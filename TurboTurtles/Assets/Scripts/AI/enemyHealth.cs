using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] int maxHP = 3;

    [Header("Flash")]
    [SerializeField] Renderer model;
    [SerializeField] float flashTime = 0.1f;

    EnemyRoundSpawner spawner;

    int currentHP;
    Color originalColor;

    void Start()
    {
        currentHP = maxHP;

        if (model != null)
        {
            originalColor = model.material.color;
        }
    }

    public void takeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
        else if (model != null)
        {
            StartCoroutine(FlashRed());
        }
    }

    void Die()
    {
        if (spawner != null)
        {
            spawner.NotifyEnemyDied();
        }

        gamemanager.instance.exp++;
        gamemanager.instance.AddScore();

        Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        model.material.color = originalColor;
    }

    public void SetSpawner(EnemyRoundSpawner roundSpawner)
    {
        spawner = roundSpawner;
    }
}