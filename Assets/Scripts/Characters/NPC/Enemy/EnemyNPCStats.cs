using UnityEngine;

namespace Characters.NPC.Enemy
{
    public class EnemyNPCStats : CharacterStats
    {
        public void SetNewMaxHealthValue(float newMaxValue)
        {
            maxHealth = newMaxValue;
            currentHealth = maxHealth;
        }
    
        public void SetNewHealthValue(float newDeltaValue)
        {
            currentHealth += newDeltaValue;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        public void LoadData(float currentHealthValue, float maxHealthValue)
        {
            this.currentHealth = currentHealthValue;
            this.maxHealth = maxHealthValue;
        }
    }
}
