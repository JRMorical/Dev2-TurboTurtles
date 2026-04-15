using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;

    [Header("Detection")]
    [SerializeField] float detectionRange = 15f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float fieldOfView = 120f;
    [SerializeField] LayerMask sightBlockMask;

    [Header("Movement")]
    [SerializeField] float repathRate = 0.2f;
    [SerializeField] float stuckCheckTime = 1f;
    [SerializeField] float stuckDistanceThreshold = 0.2f;
    [SerializeField] float nudgeDistance = 2f;

    float repathTimer;
    float stuckTimer;
    Vector3 lastPosition;

    public Transform Target => target;
    public bool HasTarget => target != null;
    public bool InAttackRange => target != null && Vector3.Distance(transform.position, target.position) <= attackRange;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (target == null || target == transform)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (target == null || agent == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        // For now: chase by distance only
        if (distance <= detectionRange)
        {
            HandleChase();
        }
        else
        {
            agent.ResetPath();
        }

        HandleStuckCheck();
    }

    void HandleChase()
    {
        repathTimer += Time.deltaTime;

        if (repathTimer >= repathRate)
        {
            repathTimer = 0f;
            agent.SetDestination(target.position);
        }

        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }
    }

    public bool CanSeeTarget()
    {
        if (target == null)
            return false;

        Vector3 rayStart = transform.position + Vector3.up * 1.2f;
        Vector3 rayEnd = target.position + Vector3.up * 1f;
        Vector3 rayDir = (rayEnd - rayStart).normalized;
        float rayDistance = Vector3.Distance(rayStart, rayEnd);

        float angle = Vector3.Angle(transform.forward, rayDir);
        if (angle > fieldOfView * 0.5f)
            return false;

        if (Physics.Raycast(rayStart, rayDir, out RaycastHit hit, rayDistance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void HandleStuckCheck()
    {
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckCheckTime)
        {
            float movedDistance = Vector3.Distance(transform.position, lastPosition);

            if (agent.hasPath && movedDistance < stuckDistanceThreshold)
            {
                Vector3 randomNudge = new Vector3(
                    Random.Range(-nudgeDistance, nudgeDistance),
                    0,
                    Random.Range(-nudgeDistance, nudgeDistance)
                );

                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position + randomNudge, out hit, 3f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }

            lastPosition = transform.position;
            stuckTimer = 0f;
        }
    }

    public void StopMoving()
    {
        if (agent != null)
            agent.ResetPath();
    }

    public void SetStoppingDistance(float value)
    {
        if (agent != null)
            agent.stoppingDistance = value;
    }
}