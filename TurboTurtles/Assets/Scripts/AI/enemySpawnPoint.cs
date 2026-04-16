using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public Transform GetSpawnTransform()
    {
        return transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}