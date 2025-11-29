using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
}
