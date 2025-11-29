using Items.Core.Interfaces;
using PlayerSystem.Input.Data;
using UI;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Physic Parameters")]
        [SerializeField] private float interactRange = 2f;
        [SerializeField] private float pickUpForce = 150f;

        private Transform _holdPoint;
        private PlayerCharacter _playerCharacter;
        private PlayerCamera _playerCamera;
        
        private CharacterController _characterController;
        
        private bool _requestedInteract;
        private bool _requestedGrab;

        private GameObject _currentHoldObject;
        private Rigidbody _currentHoldObjectRb;
    
        public bool IsInteracting { get; private set; }
        public PlayerCharacter Character => _playerCharacter;

        public void Initialize(PlayerCharacter playerCharacter, PlayerCamera playerCamera)
        {
            _playerCharacter = playerCharacter;
            _playerCamera = playerCamera;
            
            _characterController = _playerCharacter.PlayerCc;
        }

        public void UpdateInteract(InteractionInput interactInput)
        {
            ProcessInput(interactInput);
            ProcessInteract();
        }

        private void ProcessInput(InteractionInput input)
        {
            _requestedInteract = input.interactPressed;
            _requestedGrab = input.grabHeld;
        }

        private void ProcessInteract()
        {
            _holdPoint = _playerCamera.HoldObjectPoint;

            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, interactRange))
            {
                var interactableObject = hit.collider.GetComponent<IInteractable>();

                if (interactableObject != null)
                {
                    UIManager.Instance.DisplayInteractHint(interactableObject);
                
                    if (_requestedInteract)
                    {
                        StartInteract(interactableObject);
                    }
                
                    if (_requestedGrab && interactableObject is IGrabbable)
                    {
                        if (_currentHoldObject == null)
                        {
                            PickUpObject(hit.collider);
                        }
                    }
                    else
                    {
                        if (_currentHoldObject != null)
                        {
                            DropObject(hit.collider);
                        }
                    }
                }
                else
                {
                    UIManager.Instance.HideInteractHint();
                }
            }
            else
            {
                UIManager.Instance.HideInteractHint();
            }

            if (_currentHoldObject != null)
            {
                _currentHoldObjectRb.MovePosition(_holdPoint.position);
            }
        }

        private void StartInteract(IInteractable interactableObject)
        {
            interactableObject?.HandleInteract(this);
            IsInteracting = true;
        }

        private void EndInteract()
        {
            IsInteracting = false;
        }

        private void PickUpObject(Collider targetCollider)
        {
            if (targetCollider.TryGetComponent(out Rigidbody targetObjectRb))
            {
                _currentHoldObject = targetCollider.gameObject;

                _currentHoldObjectRb = targetObjectRb;
                _currentHoldObjectRb.useGravity = false;
                _currentHoldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
                _currentHoldObjectRb.isKinematic = true;

                Physics.IgnoreCollision(targetCollider, _characterController, true);
            }
        }

        private void DropObject(Collider targetCollider)
        {
            Physics.IgnoreCollision(targetCollider, _characterController, false);

            _currentHoldObjectRb.useGravity = true;
            _currentHoldObjectRb.constraints = RigidbodyConstraints.None;
            _currentHoldObjectRb.isKinematic = false;

            _currentHoldObjectRb = null;
            _currentHoldObject = null;
        }
    }
}
