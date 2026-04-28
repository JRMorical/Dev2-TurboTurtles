using System;
using System.Collections;
using UnityEngine;

public class enemyCaster : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Caster Stats-----")]
    [SerializeField] GameObject AOE_portal;
    [Range(2f, 8f)][SerializeField] float portalCooldown = 5f;
    [Range(8, 15)][SerializeField] int stoppingDistance = 8;
    [SerializeField] ParticleSystem damageEffect;
    [Range(1, 3)][SerializeField] int maxSpawned = 1;
    CharacterController controller;

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

    }
    void HandleSpawns()
    {
        if (spawned >= maxSpawned) return;

        if (!spawning)
        {
            portalTimer += Time.deltaTime;

            if (portalTimer >= portalCooldown)
            {
                spawning = true;
                spawned = 0;
                spawnDelayTimer = 0;
            }
        }
        else
        {
            spawnDelayTimer += Time.deltaTime;

            if (spawnDelayTimer >= spawnDelayCooldown)
            {
                if (controller.isGrounded)
                {
                    lockedPosition = controller.transform.position - Vector3.up * ((controller.height / 2f) + 0.07f);
                    StartCoroutine(SpawnLocationDelay(lockedPosition));
                    spawnDelayTimer = 0;
                }

                if (spawned >= maxSpawned)
                {
                    spawning = false;
                    portalTimer = 0;
                }
            }
        }
    }

    void TrySpawnPortal(enemyAI _ai)
    {
        if (controller == null) return;

        _ai.SetStoppingDistance(stoppingDistance);

        float dist = _ai.PlayerDistance();

        if (dist > stoppingDistance)
        {
            _ai.Chase(_ai.player);
            return;
        }

        _ai.Chase(_ai.player);

        if (wasFirstSpawned)
        {
            HandleSpawns();
        }
        else
        {
            if (!controller.isGrounded) return;
            lockedPosition = controller.transform.position - Vector3.up * ((controller.height / 2f) + 0.07f);
            StartCoroutine(SpawnLocationDelay(lockedPosition));
            spawning = true;
            wasFirstSpawned = true;
        }
    }
    void SpawnPortal(Vector3 _pos)
    {
        spawned++;

        GameObject AOE_instance = Instantiate(AOE_portal, _pos, Quaternion.identity);

        PortalLifetime portal = AOE_instance.GetComponent<PortalLifetime>();
        if (portal != null)
        {
            portal.owner = this;
        }
        ParticleSystem effects = Instantiate(damageEffect, AOE_instance.transform.position, Quaternion.identity);
    
        Destroy(AOE_instance, portalTimeEnd);

        if (effects != null)
        {
            Destroy(effects, portalTimeEnd);
        }
    }
    IEnumerator SpawnLocationDelay(Vector3 _loc)
    {
        yield return new WaitForSeconds(0.5f);
        SpawnPortal(_loc);
    }
    public void DecrementCount()
    {
        spawned = Mathf.Max(0, spawned - 1);
    }
    public void Tick(enemyAI _ai)
    {
        TrySpawnPortal(_ai);
    }
}