using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EnemyBrain brain;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bulletPrefab;

    [Header("Attack")]
    [SerializeField] float fireRate = 1f;
    [SerializeField] float attackRange = 10f;

    float fireTimer;

    void Start()
    {
        if (brain == null)
            brain = GetComponent<EnemyBrain>();

        brain.SetStoppingDistance(attackRange - 1f);
    }

    void Update()
    {
        if (!brain.HasTarget)
            return;

        fireTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, brain.Target.position);

        if (distance <= attackRange)
        {
            brain.StopMoving();

            Vector3 lookDir = brain.Target.position - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 8f);
            }

            if (fireTimer >= fireRate)
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        fireTimer = 0f;

        if (bulletPrefab != null && shootPos != null)
        {
            Vector3 dir = (brain.Target.position + Vector3.up) - shootPos.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            Instantiate(bulletPrefab, shootPos.position, rot);
        }
    }
}