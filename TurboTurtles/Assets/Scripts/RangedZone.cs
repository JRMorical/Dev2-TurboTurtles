using UnityEngine;

public class RangedZone : MonoBehaviour
{
    public enemyAI enemy;
    void Start()
    {
        enemy = GetComponentInParent<enemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            enemy.inRangedAttackRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        enemy.inRangedAttackRange= false;
    }
}
