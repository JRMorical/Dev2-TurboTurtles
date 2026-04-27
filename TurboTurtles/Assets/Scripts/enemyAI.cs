 using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class enemyAI : MonoBehaviour, IDamage
{
    public enum EnemyRole
    {
        MELEE_ONLY,
        RANGED_ONLY,
        CASTER_ONLY,
        SPAWNER
    }

    [Header("-----Role-----")]
    [SerializeField] EnemyRole role;

    [Header("-----Enemy Stats-----")]
    [SerializeField] int HP;
    [SerializeField] int enemyRotateSpeed = 5;

    [Header("-----Enemy Brain-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] MonoBehaviour enemyBehavior;
    [Range(0.2f, 1f)][SerializeField] float enemyLinkJumpSpeed = 0.6f;
    IEnemyBehaviour behavior;
    enemyAnimator anim;
    bool isTravesingOffMeshLink;
    Vector3 Player;
    public Vector3 player => Player;
    enemySpawner spawner;
    Color colorOrig;

    // Slowed
    bool isSlowed = false;
    float originalSpeed;

    void Awake()
    {
        behavior = enemyBehavior as IEnemyBehaviour;
        if (behavior == null)
        {
            Debug.LogError("No IEnemyBehaviour found on " + gameObject.name);
        }
    }
    void Start()
    {
        StartCalls();
    }
    void Update()
    {
        UpdateCalls();
    }
    public void SetSpawner(enemySpawner _spawner)
    {
        spawner = _spawner;
    }
    void UpdateCalls()
    {
        Player = gamemanager.instance.player.transform.position;
        behavior?.Tick(this);
    }
    void StartCalls()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        anim = GetComponent<enemyAnimator>();
        SetEnemyRole();

        //og speed before slowed
        originalSpeed = agent.speed;
    }
    void SetEnemyRole()
    {
        if (CompareTag("Melee"))
        {
            role = EnemyRole.MELEE_ONLY;
        }
        else if (CompareTag("Ranged"))
        { 
            role = EnemyRole.RANGED_ONLY; 
        }
        else if (CompareTag("Caster"))
        {
            role = EnemyRole.CASTER_ONLY;
        }
        else if (CompareTag("Spawner"))
        {
            role = EnemyRole.SPAWNER;
        }          
    }
    public void Chase(Vector3 _target)
    {
        MoveTo(_target);
        rotateToPlayer();
    }
    public float PlayerDistance()
    {
        return Vector3.Distance(Player, transform.position);
    }
    public void MoveTo(Vector3 _target)
    {
        agent.isStopped = false;
        agent.SetDestination(_target);
    }
    public void MoveStop()
    {
        agent.isStopped = true;
    }
    public void SetStoppingDistance(float _dist)
    {
        agent.stoppingDistance = _dist;
    }
    public void rotateToPlayer()
    {
        Vector3 playerDir = player - transform.position;
        playerDir.y = 0;
        if (playerDir.sqrMagnitude < 0.01f) return;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z).normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot , Time.deltaTime * enemyRotateSpeed);
    }
    public void PlayMeleeAttack()
    {
        anim.PlayAttack();
    }
    public void PlayProjectileAttack()
    {
        anim.PlayProjectile();
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        
        if (HP <= 0)
        {          
            if (spawner != null)
            {
                spawner.SpawnDeath();
                spawner.ResetTimer();
            }             
            gamemanager.instance.updateGameGoal(-1);
            gamemanager.instance.AddScore();
            gamemanager.instance.exp++;
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void ApplySlow(float amount, float duration)
    {
        if (isSlowed) return;
        StartCoroutine(SlowRoutine(amount, duration));
    }

    IEnumerator SlowRoutine(float amount, float duration)
    {
        isSlowed = true;
        agent.speed = originalSpeed * (1f - amount);
        model.material.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;
        model.material.color = colorOrig;
        isSlowed = false;
    }
}