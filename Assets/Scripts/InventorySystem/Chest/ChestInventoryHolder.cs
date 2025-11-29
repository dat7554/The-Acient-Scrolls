using System.Collections;
using System.Collections.Generic;
using Audio;
using Characters.PlayerSystem;
using Events;
using InventorySystem.Core;
using Items.Core.Interfaces;
using SaveLoadSystem;
using UI;
using UnityEngine;
 
namespace InventorySystem.Chest
{
    [RequireComponent(typeof(ChestDataHandler), typeof(UniqueIdGenerator))]
    public class ChestInventoryHolder : InventoryHolder, IInteractable
    {
        [Header("Prompt")]
        [SerializeField] private string interactionPrompt;
        
        [Header("References")]
        [SerializeField] private GameObject chestLid;
        [SerializeField] private float lidRotateSpeed = 3f;
        
        [Header("Initial Chest Items")]
        [SerializeField] private InitialChestItem[] initialItems;
 
        // public UnityAction<IInteractable> OnInteractComplete {get; private set;}
        
        private ChestSoundFX _chestSoundFX;
        private Coroutine _toggleLidCoroutine;
        private bool _isLidOpen;

        protected override void Awake()
        {
            base.Awake();

            foreach (var entry in initialItems)
            {
                if (entry.itemData == null) continue;
                container.AddItemToContainer(entry.itemData, entry.quantity);
            }
        }

        private void Start()
        {
            _chestSoundFX = GetComponent<ChestSoundFX>();
        }

        private void OnEnable()
        {
            GameEventManager.Instance.InputEventHandler.CloseActiveUI += CloseLid;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.InputEventHandler.CloseActiveUI -= CloseLid;
        }

        public string GetInteractionPrompt() => interactionPrompt;

        public void HandleInteract(PlayerInteract playerInteract)
        {
            _chestSoundFX.PlayOpenChestSoundFX();
            OpenLid();
            UIManager.Instance.DisplayChestUI(container);
        }

        private void OpenLid()
        {
            float toAngle = -90f;
            if (_toggleLidCoroutine is not null) StopCoroutine(_toggleLidCoroutine);
            _toggleLidCoroutine = StartCoroutine(RotateLidCoroutine(toAngle));
            _isLidOpen = true;
        }

        private void CloseLid()
        {
            if (!_isLidOpen) return;
            
            float toAngle = 0f;
            if (_toggleLidCoroutine is not null) StopCoroutine(_toggleLidCoroutine);
            _toggleLidCoroutine = StartCoroutine(RotateLidCoroutine(toAngle));
            _isLidOpen = false;
        }

        private IEnumerator RotateLidCoroutine(float toAngle)
        {
            float elapsedTime = 0f;
            var fromQuaternion = chestLid.transform.localRotation;
            var toQuaternion = Quaternion.Euler(toAngle, 0f, 0f);

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * lidRotateSpeed;
                chestLid.transform.localRotation = Quaternion.Slerp(fromQuaternion, toQuaternion, elapsedTime);
                yield return null;
            }
            
            chestLid.transform.localRotation = toQuaternion;
        }
    }
}
