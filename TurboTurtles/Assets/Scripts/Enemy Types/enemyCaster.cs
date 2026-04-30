using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class enemyCaster : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Caster Stats-----")]
    [SerializeField] PortalLifetime AOE_portal;
    [SerializeField] Blizzard blizzardPrefab;
    CharacterController controller;
    [Range(2f, 8f)][SerializeField] float portalCooldown = 5f;
    [Range(8, 30)][SerializeField] int stoppingDistance = 15;
    [Range(1, 3)][SerializeField] int maxSpawned = 1;
    [Range(1, 3)][SerializeField] float abilityCooldown = 1f;

    float abilityTimer;
    bool wasFirstSpawned;
    bool spawning;
    int spawned;
    float portalTimer;
    float spawnDelayCooldown = 1f;
    float spawnDelayTimer;
    float portalTimeEnd = 5f;
    Vector3 lockedPosition;

    void Start()
    {
        controller = gamemanager.instance.player.GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.Log("CHARACTER CONTROLLER IS NULL");
        }
        if (AOE_portal == null)
        {
            Debug.Log("PORTAL OBJECT HAS NOT BEEN SET");
        }
        if(blizzardPrefab == null)
        {
            Debug.Log("BLIZZARD PREFAB HAS NOT BEEN SET");
        }
    }
    void CastBlizzard(enemyAI _ai)
    {
        Vector3 location = gamemanager.instance.player.transform.position;
        location += Vector3.up * 8f;
        StartCoroutine(BlizzardDelay(location));
    }
    void HandleSpawns()
    {
        if (!spawning)
        {
            portalTimer += Time.deltaTime;
        
            if (portalTimer < portalCooldown) return;

            spawning = true;
            spawned = 0;
            spawnDelayTimer = 0;
        }
        else
        {
            spawnDelayTimer += Time.deltaTime;
            if (spawnDelayTimer < spawnDelayCooldown) return;
            if (!controller.isGrounded) return;

            lockedPosition = controller.transform.position - Vector3.up * ((controller.height / 2f) + 0.07f);
       
            StartCoroutine(SpawnLocationDelay(lockedPosition));
            spawnDelayTimer = 0;
        }
    }
    void TrySpawnPortal(enemyAI _ai)
    {
        if (controller == null) return;

        if (wasFirstSpawned)
        {
            HandleSpawns();
        }
        else
        {
            if (!controller.isGrounded) return;
            if (spawning) return;

            lockedPosition = controller.transform.position - Vector3.up * ((controller.height / 2f) + 0.07f);
            StartCoroutine(SpawnLocationDelay(lockedPosition));  
            wasFirstSpawned = true;
        }
    }
    IEnumerator SpawnLocationDelay(Vector3 _loc)
    {
        spawning = true;
        yield return new WaitForSeconds(0.5f);
        PortalLifetime portal = Instantiate(AOE_portal, _loc, Quaternion.identity);
        spawned++;

        if (spawned >= maxSpawned)
        {
            spawning = false;
            portalTimer = 0;
        }

        if (portal != null)
        {
            portal.owner = this;
        }
        Destroy(portal.gameObject, portalTimeEnd);     
    }
    IEnumerator BlizzardDelay(Vector3 _loc)
    {
        yield return new WaitForSeconds(1f);
        Instantiate(blizzardPrefab, _loc, Quaternion.identity);
    }
    public void DecrementCount()
    {
        spawned = Mathf.Max(0, spawned - 1);
    }
    public  void Tick(enemyAI _ai)
    {
        _ai.Chase(_ai.player);

        float dist = _ai.PlayerDistance();
        if (dist > stoppingDistance) return;

        TrySpawnPortal(_ai);
        abilityTimer += Time.deltaTime;

        if (abilityTimer < abilityCooldown) return;

        abilityTimer = 0;

        int roll = UnityEngine.Random.Range(0, 3);

        if (roll == 1)
        {
            CastBlizzard(_ai);
        }
    }
}