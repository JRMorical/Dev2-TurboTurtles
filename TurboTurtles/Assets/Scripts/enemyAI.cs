using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour, IDamage
{
    enum AttackType
    {
        IDLE,
        MELEE,
        RANGED
    }
    public enum EnemyRole
    {
        MELEE_ONLY,
        RANGED_ONLY,
        SPAWNER
    }

    [Header("Role")]
    [SerializeField] EnemyRole role;

    [Header("Enemy Stats")]
    [SerializeField] int HP;
    [SerializeField] int enemyRotateSpeed = 5;
    [Range(1, 3)][SerializeField] float meleeRange;
    [Range(13, 25)][SerializeField] float rangedRange;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate = 1f;
    [SerializeField] Transform shootPos;
    float shootTimer;

    enemyAnimator anim;
    Color colorOrig;

    [Header("Spawner")]
    [SerializeField] GameObject meleePrefab;
    [SerializeField] GameObject rangedPrefab;
    [SerializeField] float spawnCooldown = 5f;
    [SerializeField] float spawnRadius = 8f;
    [SerializeField] int maxSpawned = 3;
    int currentSpawned;
    enemyAI spawner;

    [Header("Attacking")]
    AttackType currentType;
    float spawnTimer;
    bool isTravesingOffMeshLink;
    float attackCooldown;

    [Header("Player Position")]
    Vector3 playerDir;
    Vector3 player;

    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        anim = GetComponent<enemyAnimator>();
        SetEnemyRole();
    }
    void Update()
    {
        shootTimer += Time.deltaTime;
        attackCooldown -= Time.deltaTime;
        spawnTimer += Time.deltaTime;
        EnemyTypeActions();
    }
    void SetEnemyRole()
    {
        if (CompareTag("Melee"))
            role = EnemyRole.MELEE_ONLY;

        else if (CompareTag("Ranged"))
            role = EnemyRole.RANGED_ONLY;

        else if (CompareTag("Spawner"))
            role = EnemyRole.SPAWNER;
    }
    void EnemyTypeActions()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        player = gamemanager.instance.player.transform.position;

        float dist = Vector3.Distance(player, transform.position);

        switch (role)
        {
            case EnemyRole.MELEE_ONLY:
                currentType = dist <= meleeRange ? AttackType.MELEE : AttackType.IDLE;
                break;

            case EnemyRole.RANGED_ONLY:
                currentType = dist <= rangedRange ? AttackType.RANGED : AttackType.IDLE;            
                break;

            case EnemyRole.SPAWNER:
                TrySpawn();
                break;
        }
        ExecuteStateActions();
    }
    void ExecuteStateActions()
    {
        if (isTravesingOffMeshLink) return;
        switch(currentType)
        {
            case AttackType.MELEE:
                if(attackCooldown <= 0)
                {
                    Melee();
                    attackCooldown = 0.5f;
                }
                break;

            case AttackType.RANGED:
                Attack();
                break;

            case AttackType.IDLE:
                Chase();
                break;

            default:
                break;
        }
    }
    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player);
        rotateToPlayer();
        if(agent.isOnOffMeshLink)
        {
            StartCoroutine(LinkJump());
        }
    }
    void Attack()
    {
        agent.isStopped = true;
        shootPos.transform.LookAt(new Vector3(player.x, player.y, player.z));
        rotateToPlayer();
        anim.PlayProjectile();

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;
            Shoot();
        }
    }
   void Melee()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.x, player.y, player.z));

        rotateToPlayer();
        anim.PlayAttack();
    }
    void Shoot()
    {
        if (bullet != null)
        {
            Instantiate(bullet, shootPos.position, shootPos.rotation);
        }
    }
    void TrySpawn()
    {
        if (spawnTimer < spawnCooldown) return;
        if (currentSpawned >= maxSpawned) return;

        spawnTimer = 0;
        currentSpawned++;

        GameObject prefabToSpawn;

        int roll = Random.Range(0, 2);

        if (roll == 0)
            prefabToSpawn = meleePrefab;
        else
            prefabToSpawn = rangedPrefab;

        Vector3 offset = Random.insideUnitSphere * spawnRadius;
        offset.y = 0;

        Vector3 pos = transform.position + offset + Vector3.right * Random.Range(-2f, 2f); ;

        GameObject enemy = Instantiate(prefabToSpawn, pos, transform.rotation);
        enemyAI ai = enemy.GetComponent<enemyAI>();
        ai.SetSpawner(this);
    }
    public void SetSpawner(enemyAI s)
    {
        spawner = s;
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
        isTravesingOffMeshLink = true;

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

        agent.Warp(end);
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.CompleteOffMeshLink();

        isTravesingOffMeshLink = false;
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        
  
        if (HP <= 0)
        {
            //if (role == EnemyRole.SPAWNER)
            //{
            //    currentSpawned--;
            //}             
            if (spawner != null)
                spawner.currentSpawned--;
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