using System.Collections;
using Audio;
using Camera;
using Characters.PlayerSystem.Animators;
using Characters.PlayerSystem.Input;
using Interfaces;
using UI;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class Player : Character, IDamageable
    {
        [SerializeField] private ArmsPlayerAnimator armsAnimator;
        [SerializeField] private ShadowPlayerAnimator shadowAnimator;
        [Space]
        [SerializeField] private CameraMotionController cameraMotionController;
        
        [Header("Status")]
        public bool isDead;
        
        // TODO: consider to be public but hide in inspector -> to not need to use initialize
        private PlayerCharacter _playerCharacter;
        private PlayerCamera _playerCamera;
        private PlayerStats _playerStats;
        private PlayerInputHandler _playerInputHandler;
        private PlayerInventoryHolder _playerInventoryHolder;
        private PlayerEquipment _playerEquipment;
        private PlayerInteract _playerInteract;
        private PlayerCombat _playerCombat;
        private PlayerSoundFX _playerSoundFX;

        public override FactionType Faction => FactionType.Player;
        public override Vector3 ForwardTransform => _playerCharacter.transform.forward;
        public Transform PlayerTransform => _playerCharacter.transform;

        private void Awake()
        {
            _playerCharacter = GetComponentInChildren<PlayerCharacter>();
            _playerCamera = GetComponentInChildren<PlayerCamera>();
            _playerStats = GetComponent<PlayerStats>();
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _playerInventoryHolder = GetComponent<PlayerInventoryHolder>();
            _playerEquipment = GetComponent<PlayerEquipment>();
            _playerInteract = GetComponent<PlayerInteract>();
            _playerCombat = GetComponent<PlayerCombat>();
            _playerSoundFX = GetComponent<PlayerSoundFX>();
        }

        private void Start()
        {
            _playerCharacter.Initialize(_playerStats, cameraMotionController, _playerSoundFX);
            _playerCamera.Initialize(_playerCharacter.CameraOffset);
            
            armsAnimator.Initialize(this, _playerCharacter, _playerEquipment, _playerCombat);
            shadowAnimator.Initialize(this, _playerCharacter, _playerEquipment, _playerCombat);
            
            _playerStats.Initialize(100f, 100f);
            _playerInputHandler.Initialize(_playerCharacter, _playerCamera, _playerInteract, armsAnimator, shadowAnimator);
            _playerInventoryHolder.Initialize(_playerEquipment, _playerStats, _playerSoundFX);
            _playerInteract.Initialize(_playerCharacter, _playerCamera);
            _playerCombat.Initialize(_playerEquipment, _playerStats);
        }
        
        private void Update()
        {
            var deltaTime = Time.deltaTime;

            _playerStats.RegenerateStamina(_playerCharacter.IsSprinting, _playerCombat.IsBlocking, deltaTime);
            _playerSoundFX.PlayBreathSoundFX(_playerStats.GetStaminaPercent());
            _playerInputHandler.UpdateInputs();
            _playerCombat.UpdateCombat(deltaTime);
        }

        private void LateUpdate()
        {
            _playerCamera.MoveCameraPosition(_playerCharacter.CameraOffset);
        }

        public void TakeDamage(float damage, float? hitAngle = null)
        {
            _playerStats.SetNewHealthValue(-damage);
            
            // VFX
            cameraMotionController.PlayTemporaryNoise(cameraMotionController.ShakeNoise);
            UIManager.Instance.DisplayDamageOverlay();
            
            // SFX
            PlayDamageSoundFX();
            PlayDamageGruntSoundFX();
            
            if (_playerStats.CurrentHealth == 0f)
            {
                Die();
            }
        }

        public override void HandleSuccessfulBlock()
        {
            PlayBlockImpactSoundFX();
            
            armsAnimator.HandleBlockImpact();
            shadowAnimator.HandleBlockImpact();
            
            _playerStats.SpendStaminaForBlock();
        }

        public override void HandleAttackHit()
        {
            cameraMotionController.PlayTemporaryNoise(cameraMotionController.ShakeNoise, amplitude: 1.5f, frequency: 0.2f);
            
            //armsAnimator.HandleAttackHit();
            //shadowAnimator.HandleAttackHit();
        }
        
        // TODO: consider move this elsewhere
        public void PlayWeaponWhooshSoundFX()
        {
            _playerSoundFX.PlayWeaponWhooshSoundFX();
        }
        
        public void PlayLandSoundFX()
        {
            _playerSoundFX.PlayLandSoundFX();
        }

        private void PlayDamageSoundFX()
        {
            AudioClip audioClip = AudioManager.Instance.GetRandomElement(AudioManager.Instance.physicalDamageSFX);
            _playerSoundFX.PlaySoundFX(audioClip, volume: 0.75f);
        }
        
        private void PlayDamageGruntSoundFX()
        {
            _playerSoundFX.PlayDamageGruntSoundFX();
        }

        private void PlayBlockImpactSoundFX()
        {
            _playerSoundFX.PlayBlockImpactSoundFX();
        }
        
        private void Die()
        {
            StartCoroutine(WaitBeforeDisplayRoutine());
        }

        private IEnumerator WaitBeforeDisplayRoutine()
        {
            // Wait to let others vfx and sfx process before display game over screen
            yield return new WaitForSeconds(0.5f);
            
            UIManager.Instance.DisplayGameOverScreen();
        }
    }
}