using Camera;
using Characters.PlayerSystem.Input.Data;
using Events;
using PlayerSystem.Input.Data;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerCharacter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraOffset;
        [SerializeField] private LayerMask collisionLayer;
    
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 7f;
        [SerializeField] private float sprintSpeed = 15f;
        [SerializeField] private float crouchSpeed = 5f;
        [Space]
        [SerializeField] private float walkResponse = 25f;
        [SerializeField] private float crouchResponse = 20f;
        [Space]
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float coyoteTime = 0.2f;
        [Space]
        [SerializeField] private float pushPower = 2.0f;
        [Space]
        [SerializeField, Range(0f, 1f)] private float runStepLength = 0.5f;
        [SerializeField] private float stepInterval = 0.2f;

        [Header("Body")]
        [SerializeField] private float standHeight = 1.85f;
        [SerializeField] private float crouchHeight = 0.925f;
        [SerializeField] private float heightChangeResponse = 15f;
        [SerializeField, Range(0f, 1f)] private float standCameraTargetHeight = 0.9f;
        [SerializeField, Range(0f, 1f)] private float crouchCameraTargetHeight = 0.8f;
    
        private PlayerStats _playerStats;
        private CameraMotionController _cameraMotionController;
        private PlayerSoundFX _playerSoundFX;
    
        private Stance _stance;

        private float _verticalVelocity;
        private Vector3 _currentHorizontalVelocity;

        #region Input Variables
        
        [Header("Debug")]
        [SerializeField] private Vector3 _requestedMovement;
        private Quaternion _requestedRotation;
        private bool _requestedCrouch;
        private bool _requestedJump;
        private bool _requestedSprint;
    
        #endregion

        private float _timeSinceUngrounded;
        private float _timeSinceJumpRequest;
        private bool _ungroundedDueToJump;
        private bool _onSteepSlope;

        private float _currentSpeed;
        private float _stepCycle;
        private float _nextStep;
        private float _stepTimer;

        #region Public Properties
    
        public Transform CameraOffset => cameraOffset;
        public Vector3 CenterPosition => transform.position + characterController.center;
        // public bool IsMoving => _requestedMovement.magnitude > 0.01f;
        public bool IsMoving => characterController.velocity.sqrMagnitude > 1f && IsGrounded;
        public bool IsMovingForwardOrSideways 
        {
            get
            {
                if (!IsMoving) 
                    return false;

                var forwardAmount = Vector3.Dot(_requestedMovement.normalized, transform.forward);
                return forwardAmount >= 0f;
            }
        }
        public bool IsGrounded => characterController.isGrounded;
        public bool IsJumping { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsSprinting { get; private set; }
        public CharacterController PlayerCc => characterController;
    
        #endregion
    
        // --- Debug
        private bool _isGrounded;
        public Stance PlayerStance { get { return _stance; } }
        public bool OnSlope { get; private set; }
        public Vector3 VerticalVelocity { get; private set; }
        public Vector3 HorizontalVelocity { get; private set; }
        public Vector3 FinalMovement { get; private set; }
        public float DeltaTime { get; private set; }
        // ---

        #region Unity Methods
    
        private void OnEnable()
        {
            GameEventManager.Instance.InputEventHandler.JumpStarted += HandleJumpStarted;
            GameEventManager.Instance.InputEventHandler.JumpCanceled += HandleJumpCanceled;
        
            GameEventManager.Instance.InputEventHandler.CrouchStarted += HandleCrouchStarted;
            GameEventManager.Instance.InputEventHandler.CrouchCanceled += HandleCrouchCanceled;
        
            GameEventManager.Instance.InputEventHandler.SprintStarted += HandleSprintStarted;
            GameEventManager.Instance.InputEventHandler.SprintCanceled += HandleSprintCanceled;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.InputEventHandler.JumpStarted -= HandleJumpStarted;
            GameEventManager.Instance.InputEventHandler.JumpCanceled -= HandleJumpCanceled;
        
            GameEventManager.Instance.InputEventHandler.CrouchStarted -= HandleCrouchStarted;
            GameEventManager.Instance.InputEventHandler.CrouchCanceled -= HandleCrouchCanceled;
        
            GameEventManager.Instance.InputEventHandler.SprintStarted -= HandleSprintStarted;
            GameEventManager.Instance.InputEventHandler.SprintCanceled -= HandleSprintCanceled;
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var rigidBody = hit.collider.attachedRigidbody;
            if (rigidBody is null || rigidBody.isKinematic)
            {
                return;
            }

            // Don't push objects below
            if (hit.moveDirection.y < -0.3)
            {
                return;
            }

            // Apply the push
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rigidBody.AddForce(pushDir * pushPower, ForceMode.Impulse);
        }
    
        #endregion

        // TODO: consider move playerAnimator related in Player to PlayerCharacter
        // as playerAnimator is a child of playerCharacter
        public void Initialize(PlayerStats playerStats, CameraMotionController cameraMotionController, PlayerSoundFX playerSoundFX)
        {
            _stance = Stance.Stand;
            _currentSpeed = walkSpeed;

            _playerStats = playerStats;
            _cameraMotionController = cameraMotionController;
            _playerSoundFX = playerSoundFX;
        }

        public void UpdateMovement(MovementInput input, float deltaTime)
        {
            ProcessInput(input);
            ChangeCharacterHeight(deltaTime);
            MoveCharacter(deltaTime);
            ProgressStepCycle();
            RotateCharacter();
            UpdateHeadBob();
        }

        public void ClearRequestedInputs()
        {
            // TODO: fix bug - When pause game, player is freeze but animation is not
        }

        private void ProcessInput(MovementInput input)
        {
            _requestedRotation = input.characterRotation;

            _requestedMovement = new Vector3(input.horizontalMovement.x, 0f, input.horizontalMovement.y);
            _requestedMovement = input.characterRotation * _requestedMovement.normalized;
        }

        #region Event Handler Methods
    
        private void HandleJumpStarted()
        {
            var wasRequestedJump = _requestedJump;
            _requestedJump = true;
            if (_requestedJump && !wasRequestedJump)
            {
                _timeSinceJumpRequest = 0f;
            }
            IsJumping = true;
            _playerSoundFX.PlayJumpSoundFX();
        }

        private void HandleJumpCanceled()
        {
            IsJumping = false;
        }

        private void HandleCrouchStarted()
        {
            _requestedCrouch = !_requestedCrouch;
        }

        private void HandleCrouchCanceled()
        {
            //_requestedCrouch = false;
        }

        private void HandleSprintStarted()
        {
            _requestedSprint = true;
        }

        private void HandleSprintCanceled()
        {
            _requestedSprint = false;
        }
    
        #endregion

        #region Movement Logic Methods
        
        private void ChangeCharacterHeight(float deltaTime)
        {
            // Crouch
            if (_requestedCrouch && _stance is Stance.Stand)
            {
                _stance = Stance.Crouch;

                SetCapsuleDimensions
                (
                    radius: characterController.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
                
                IsCrouching = true;
            } 
            // Uncrouch
            else if (!_requestedCrouch && _stance is not Stance.Stand)
            {
                SetCapsuleDimensions
                (
                    radius: characterController.radius,
                    height: standHeight,
                    yOffset: standHeight * 0.5f
                );

                // Check ceiling
                var position = transform.position + Vector3.up * characterController.radius;
                float distance = 2f - characterController.radius * 2;
                if (Physics.SphereCast(position, characterController.radius, Vector3.up, out _, distance, collisionLayer))
                {
                    // Re-crouch
                    _requestedCrouch = true;

                    SetCapsuleDimensions
                    (
                        radius: characterController.radius,
                        height: crouchHeight,
                        yOffset: crouchHeight * 0.5f
                    );
                }
                else
                {
                    _stance = Stance.Stand;
                    IsCrouching = false;
                }
            }

            var currentHeight = characterController.height;
            var normalizedHeight = currentHeight / standHeight;
            var cameraTargetHeight = currentHeight * (_stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight);

            // TODO: fix crouch logic
            transform.localScale = Vector3.Lerp
            (
                a: transform.localScale,
                b: new Vector3(1f, normalizedHeight, 1f),
                t: 1f - Mathf.Exp(-heightChangeResponse * deltaTime)
            );
            cameraOffset.localPosition = Vector3.Lerp
            (
                a: cameraOffset.localPosition,
                b: new Vector3(cameraOffset.localPosition.x, cameraTargetHeight, cameraOffset.localPosition.z),
                t: 1f - Mathf.Exp(-heightChangeResponse * deltaTime)
            );
        }

        private void RotateCharacter()
        {
            var forward = Vector3.ProjectOnPlane
            (
                vector: _requestedRotation * Vector3.forward,
                planeNormal: Vector3.up
            );
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        private void UpdateHeadBob()
        {
            if (IsSprinting)
            {
                _cameraMotionController.EnableHeadBob(true, frequency: 1.5f);
            }
            else if (IsMoving && IsCrouching)
            {
                _cameraMotionController.EnableHeadBob(true, 0.5f, 0.75f);
            }
            else if (IsMoving)
            {
                _cameraMotionController.EnableHeadBob(true);
            }
            else
            {
                _cameraMotionController.EnableHeadBob(false);
            }
        }

        private void ProgressStepCycle()
        {
            if (!IsMoving)
            {
                _stepTimer = 0f;
                return;
            }
            
            float speed = characterController.velocity.magnitude;
            if (speed < 0.1f) return;

            float currentStepInterval = stepInterval * (walkSpeed / speed);
            _stepTimer += Time.deltaTime;
            if (_stepTimer >= currentStepInterval)
            {
                _stepTimer = 0f;
                _playerSoundFX.PlayWalkOrSprintSoundFX(IsCrouching ? 0.25f : 1f);
            }
        }

        private void MoveCharacter(float deltaTime)
        {
            DeltaTime = deltaTime;
        
            HandleHorizontalMovement(deltaTime);
            HandleVerticalMovement(deltaTime);
            
            HandleJumpRequest(deltaTime);

            var finalMovement = CalculateFinalMovement(deltaTime);
            characterController.Move(finalMovement);
        }

        // TODO: refactor handle sprint
        private void HandleHorizontalMovement(float deltaTime)
        {
            var movement = Vector3.ProjectOnPlane
            (
                vector: _requestedMovement,
                planeNormal: Vector3.up
            ).normalized * _requestedMovement.magnitude;

            // Handle Sprint
            if (_requestedSprint && _stance is Stance.Stand && IsMovingForwardOrSideways)
            {
                IsSprinting = _playerStats.HasStaminaForSprint(deltaTime);
                _playerStats.SpendStaminaForSprint(deltaTime);
            } 
            else
            {
                IsSprinting = false;
            }

            _currentSpeed = IsSprinting ? sprintSpeed : (_stance is Stance.Stand ? walkSpeed : crouchSpeed);
            var response = _stance is Stance.Stand ? walkResponse : crouchResponse;

            var targetMovement = movement * _currentSpeed;
            _currentHorizontalVelocity = Vector3.Lerp
            (
                a: _currentHorizontalVelocity,
                b: targetMovement,
                t: 1f - Mathf.Exp(-response * deltaTime)
            );
        }

        private void HandleVerticalMovement(float deltaTime)
        {
            // If on ground...
            if (IsGrounded)
            {
                _timeSinceUngrounded = 0f;
                _ungroundedDueToJump = false;

                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = Mathf.Lerp(_verticalVelocity, gravity, 4f * deltaTime);
                }
            }
            // else on air.
            else
            {
                _timeSinceUngrounded += deltaTime;

                if (!_ungroundedDueToJump && _timeSinceUngrounded <= deltaTime * 2f)
                {
                    _verticalVelocity = -1f;
                }

                _verticalVelocity += gravity * deltaTime;
            }
        }
    
        private void HandleJumpRequest(float deltaTime)
        {
            if (!_requestedJump) return;
        
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if ((IsGrounded || canCoyoteJump) && !_onSteepSlope)
            {
                _requestedJump = false;

                _ungroundedDueToJump = true;

                var tagetVerticalSpeed = Mathf.Max(_verticalVelocity, jumpSpeed);
                _verticalVelocity += tagetVerticalSpeed - _verticalVelocity;
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;

                var canJumpLater = _timeSinceJumpRequest < coyoteTime && !_onSteepSlope;
                _requestedJump = canJumpLater;
            }
        }

        private Vector3 CalculateFinalMovement(float deltaTime)
        {
            var verticalMovement = Vector3.up * (_verticalVelocity * deltaTime);
            VerticalVelocity = verticalMovement;
            var finalMovement = _currentHorizontalVelocity * deltaTime;
            HorizontalVelocity = finalMovement;
            finalMovement = CollideAndSlide(finalMovement, transform.position, false, 0);
            Debug.DrawRay(transform.position, finalMovement, Color.blue, 1f);
            finalMovement += CollideAndSlide(verticalMovement, transform.position + finalMovement, true, 0);
            Debug.DrawRay(transform.position, finalMovement, Color.green, 1f);
            FinalMovement = finalMovement;

            return finalMovement;
        }

        private void SetCapsuleDimensions(float radius, float height, float yOffset)
        {
            height = Mathf.Max(height, (radius * 2f) + 0.01f); // Safety to prevent invalid capsule geometries

            characterController.radius = radius;
            characterController.height = Mathf.Clamp(height, radius * 2f, height);
            characterController.center = new Vector3(0f, yOffset, 0f);
        }
    
        //TODO: Check stuck below slope (no slide slope above head)
        private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, bool gravityPass, int depth)
        {
            if (depth >= 3)
            {
                return Vector3.zero;
            }

            RaycastHit hit = CastInDirection(position, velocity);

            //early return if didn't hit anything
            if (hit.collider == null)
            {
                //convert the direction based on the surface player is standing on
                if (gravityPass)
                {
                    _isGrounded = false;
                }
                return velocity;
            }

            //cut the vector in 2: before and past the hit point
            Vector3 distanceToSurface = velocity.normalized * (hit.distance - characterController.skinWidth);
            Vector3 distancePastSurface = velocity - distanceToSurface;
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            OnSlope = slopeAngle > 0f;

            //prevent overlap when the movement would be within the skin width
            if (distanceToSurface.magnitude <= characterController.skinWidth)
            {
                //distanceToSurface = Vector3.zero;
                distanceToSurface = gravityPass ? Vector3.down * (0.05f + characterController.skinWidth) : Vector3.zero;
            }

            //gentle slope
            if (slopeAngle <= characterController.slopeLimit)
            {
                _onSteepSlope = false;

                if (gravityPass)
                {
                    _isGrounded = true;
                    return distanceToSurface;
                }
                distancePastSurface = Vector3.ProjectOnPlane(distancePastSurface, hit.normal).normalized * distancePastSurface.magnitude;
            }
            else //wall or steep slope
            {
                _onSteepSlope = true;

                if (!gravityPass && _isGrounded)
                {
                    // find the sliding direction (left/right) when walking in a wall or steep slope 
                    distancePastSurface = Vector3.ProjectOnPlane(
                        new Vector3(distancePastSurface.x, 0, distancePastSurface.z),
                        new Vector3(hit.normal.x, 0, hit.normal.z)
                    );
                }
                else if (gravityPass)
                {
                    //adjusting falling direction to slide on the steep slope
                    distancePastSurface = Vector3.ProjectOnPlane(distancePastSurface, hit.normal).normalized * distancePastSurface.magnitude;
                }
            }
            return distanceToSurface + CollideAndSlide(distancePastSurface, position + distanceToSurface, gravityPass, depth + 1);
        }

        private RaycastHit CastInDirection(Vector3 position, Vector3 velocity)
        {
            Physics.CapsuleCast(
                position + Vector3.up * (characterController.height - characterController.radius),
                position + Vector3.up * characterController.radius,
                characterController.radius - characterController.skinWidth,
                velocity.normalized,
                out RaycastHit hit,
                velocity.magnitude + characterController.skinWidth,
                collisionLayer);
            return hit;
        }
        
        #endregion
    }
}