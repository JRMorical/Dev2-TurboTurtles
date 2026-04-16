using UnityEngine;

public class AttackZone : MonoBehaviour
{
    public enemyAI enemy;
    void Start()
    {
        enemy = GetComponentInParent<enemyAI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemy.inMeleeAttackRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemy.inMeleeAttackRange = false;
        }
    }
}
