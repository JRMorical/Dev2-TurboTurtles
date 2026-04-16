using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class enemyAI : MonoBehaviour, IDamage
{
    enum MovementState
    {
        CHASING,
        ATTACKING
    }
    enum AttackType
    {
        IDLE,
        MELEE,
        RANGED
    }
  
    [Header("Enemy Stats")]
    [SerializeField] int HP;
    [SerializeField] int enemyRotateSpeed = 5;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate = 1f;
    [SerializeField] Transform shootPos;

    float shootTimer;

    [SerializeField] enemyAnimator anim;
    Color colorOrig;

    MovementState currentState;
    AttackType currentType;
    [Header("Attacking")]
    public bool inRangedAttackRange;
    public bool inMeleeAttackRange;
    public bool canAttack;
    float attackCooldown;

    Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        anim = GetComponent<enemyAnimator>();
    }
    void Update()
    {
        EnemyTypeActions();
    }
    void EnemyTypeActions()
    {
        agent.SetDestination(gamemanager.instance.player.transform.position);
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        attackCooldown -= Time.deltaTime;
        if (inMeleeAttackRange && attackCooldown <= 0)
        {
            currentState = MovementState.ATTACKING;
            currentType = AttackType.MELEE;
            Melee();
        }
        else if (inRangedAttackRange)
        {
            currentState = MovementState.ATTACKING;
            currentType = AttackType.RANGED;
            Attack();
        }
        else
        {
            currentState = MovementState.CHASING;
            currentType = AttackType.IDLE;
            Chase();
        }
    }
    void Chase()
    {
        agent.SetDestination(gamemanager.instance.player.transform.position);
        if(agent.isOnOffMeshLink)
        {
            StartCoroutine(LinkJump());
        }
    }
    void Attack()
    {
        agent.SetDestination(transform.position);

        rotateToPlayer();

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;
            Shoot();
        }
    }
   void Melee()
    { 
        if (anim == null || agent == null) return;
  
        attackCooldown = .5f;
        rotateToPlayer();
        anim.PlayAttack();
        agent.SetDestination(transform.position);
    }
    void Shoot()
    {
        if (bullet != null)
        {
            Instantiate(bullet, shootPos.position, shootPos.rotation);
        }
    }
    void rotateToPlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z).normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot , Time.deltaTime * enemyRotateSpeed);
    }
    IEnumerator LinkJump()
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;

        agent.isStopped = true;
        agent.updateRotation = false;
        Vector3 start = transform.position;
        Vector3 end = data.endPos + Vector3.up * agent.baseOffset;

        float time = 0f;
        float duration = 0.6f;
        while(time < duration)
        {
            float t = time / duration;
            float height = Mathf.Sin(t * Mathf.PI) * 2f;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        agent.CompleteOffMeshLink();
        agent.isStopped = false;
        agent.updateRotation = true;
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
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
}