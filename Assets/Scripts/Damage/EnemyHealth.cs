using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Takedamage(int damageTaken, int damageMultiplier)
    {
        int totalDamage = damageTaken * damageMultiplier;
        currentHealth -= totalDamage;

        Debug.Log($"{gameObject.name} took {totalDamage} damage. Remaining: {currentHealth}");

        // Check for death here instead of Update()
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death effects, sounds, or loot drops here
        Debug.Log($"{gameObject.name} has been defeated!");
        Destroy(gameObject); 
    }
}
