using System.Collections;
using Events;
using Items.ScriptableObjects;
using UnityEngine;

namespace Characters.PlayerSystem
{
    public class PlayerStats : CharacterStats
    {
        [Header("Stamina")]
        [SerializeField] private float currentStamina;
        [SerializeField] private float maxStamina;
        [Space]
        [SerializeField] private float staminaSprintDrainRate = 20f;
        [SerializeField] private float staminaBlockDrainRate = 15f;
        [SerializeField, Range(0f, 1f)] private float staminaRegenerationRate = 0.75f;
        [SerializeField] private float staminaRegenerationAmount = 2f;
        [SerializeField] private float staminaRegenerationDelay = 2f;

        private float _staminaRegenerationTimer;
        private Coroutine _staminaRegenerateCoroutine;
        private bool _isRegeneratingStamina;
    
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;

        public void Initialize(float maxStaminaValue, float maxHealthValue)
        {
            SetNewMaxStaminaValue(maxStaminaValue);
            SetNewMaxHealthValue(maxHealthValue);
        }
        
        public float GetStaminaPercent()
        {
            if (MaxStamina <= 0f)
                return 0f;

            return (CurrentStamina / MaxStamina) * 100f;
        }

        public void LoadData(float currentHealthValue, float maxHealthValue, float currentStaminaValue, float maxStaminaValue)
        {
            this.currentHealth = currentHealthValue;
            this.maxHealth = maxHealthValue;
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentHealthChanged(currentHealth, maxHealth);
        
            this.currentStamina = currentStaminaValue;
            this.maxStamina = maxStaminaValue;
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentStaminaChanged(currentStamina, maxStamina);
        }

        public void ApplyItemEffect(ItemSO itemDataToApply)
        {
            if (itemDataToApply is FoodItemSO foodItemData)
            {
                SetNewHealthValue(foodItemData.HealthRestoreAmount);
            }
        }

        public bool HasStaminaForSprint(float deltaTime)
        {
            float required = staminaSprintDrainRate * deltaTime;
            return currentStamina >= required;
        }

        public void SpendStaminaForSprint(float deltaTime)
        {
            SetNewStaminaValue(-staminaSprintDrainRate * deltaTime);
        }
        
        public bool HasStaminaForBlock(float deltaTime)
        {
            float required = staminaBlockDrainRate * deltaTime;
            return currentStamina >= required;
        }

        public void SpendStaminaForBlock()
        {
            SetNewStaminaValue(-staminaBlockDrainRate);
        }

        // TODO: consider to change to timer for smoother change but have to update per frame more
        public void RegenerateStamina(bool isPlayerSprinting, bool isPlayerBlocking, float deltaTime)
        {
            if (isPlayerSprinting || isPlayerBlocking)
            {
                _staminaRegenerationTimer = 0f;

                if (_isRegeneratingStamina)
                {
                    StopCoroutine(_staminaRegenerateCoroutine);
                    _isRegeneratingStamina = false;
                }

                return;
            }

            if (currentStamina >= maxStamina)
            {
                _staminaRegenerationTimer = 0f;
                currentStamina = maxStamina;
            }

            _staminaRegenerationTimer += deltaTime;

            if (_staminaRegenerationTimer >= staminaRegenerationDelay && !_isRegeneratingStamina)
            {
                _isRegeneratingStamina = true;
                _staminaRegenerateCoroutine = StartCoroutine(StartRegenerateStamina());
            }
        }

        private IEnumerator StartRegenerateStamina()
        {
            while (currentStamina < maxStamina)
            {
                SetNewStaminaValue(staminaRegenerationAmount);
                yield return new WaitForSeconds(1f - staminaRegenerationRate);
            }

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                _isRegeneratingStamina = false;
            }
        }

        // TODO: consider DRY
        public void SetNewMaxStaminaValue(float newMaxValue)
        {
            //SetNewMaxStatValue(ref _currentStaminaValue, ref _maxStaminaValue, CurrentStaminaChanged, newMaxValue);
            maxStamina = newMaxValue;
            currentStamina = maxStamina;
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentStaminaChanged(currentStamina, maxStamina);
        }
    
        public void SetNewMaxHealthValue(float newMaxValue)
        {
            //SetNewMaxStatValue(ref _currentHealthValue, ref _maxHealthValue, CurrentHealthChanged, newMaxValue);
            maxHealth = newMaxValue;
            currentHealth = maxHealth;
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentHealthChanged(currentHealth, maxHealth);
        }

        //private void SetNewMaxStatValue(ref float currentValue, ref float maxValue, Action<float, float> currentValueChanged, float newMaxValue)
        //{
        //    maxValue = newMaxValue;
        //   currentValue = maxValue;
        //    currentValueChanged?.Invoke(currentValue, maxValue);
        //}
    
        private void SetNewStaminaValue(float newDeltaValue)
        {
            //SetNewCurrentStatValue(ref _currentStaminaValue, ref _currentStaminaValue, CurrentStaminaChanged, newDeltaValue);
            currentStamina += newDeltaValue;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentStaminaChanged(currentStamina, maxStamina);
        }
        
        public void SetNewHealthValue(float newDeltaValue)
        {
            //SetNewCurrentStatValue(ref _currentHealthValue, ref _maxHealthValue, CurrentHealthChanged, newDeltaValue);
            currentHealth += newDeltaValue;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            GameEventManager.Instance.PlayerEventHandler.InvokeCurrentHealthChanged(currentHealth, maxHealth);
        }

        //private void SetNewCurrentStatValue(ref float currentValue, ref float maxValue, Action<float, float> currentValueChanged, float newDeltaValue)
        //{
        //    currentValue += newDeltaValue;
        //    currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
        //    currentValueChanged?.Invoke(currentValue, maxValue);
        //}
    }
}
