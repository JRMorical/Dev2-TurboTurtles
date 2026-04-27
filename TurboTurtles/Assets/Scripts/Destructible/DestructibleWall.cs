using UnityEngine;

public class DestructibleWall : MonoBehaviour, IDamage
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] GameObject debrisEffect;

    int currentHealth;

    void Start() => currentHealth = maxHealth;

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (debrisEffect != null)
                Instantiate(debrisEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}