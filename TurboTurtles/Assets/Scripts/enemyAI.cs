using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour, IDamage
{
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
    [Range(13, 25)][SerializeField] float rangedRange;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("Projectile")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate = 1f;
    [SerializeField] Transform shootPos;
    float shootTimer;

    enemyAnimator anim;
    IEnemyBehaviour behavior;

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
    float spawnTimer;
    bool isTravesingOffMeshLink;

    [Header("Player Position")]
    public Vector3 player;

    void Start()
    {
        behavior = GetComponent<IEnemyBehaviour>();

        if (behavior == null)
        {
            Debug.LogError("No IEnemyBehaviour found on " + gameObject.name);
        }
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        anim = GetComponent<enemyAnimator>();
        SetEnemyRole();
    }
    void Update()
    {
        //shootTimer += Time.deltaTime;
        //spawnTimer += Time.deltaTime;
        player = gamemanager.instance.player.transform.position;
        HandleNavJump();
        behavior?.Tick(this);
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

    public void Chase(Vector3 _target)
    {
        MoveTo(_target);
        rotateToPlayer();
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
    public void MoveTo(Vector3 _target)
    {
        agent.isStopped = false;
        agent.SetDestination(_target);
    }
    public void MoveStop()
    {
        agent.isStopped = true;
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
    void HandleNavJump()
    {
        if (isTravesingOffMeshLink) return;
        if(agent.isOnOffMeshLink)
        {
            StartCoroutine(LinkJump());
        }
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