using UnityEngine;

public class enemyCaster : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Caster Stats-----")]
    [SerializeField] GameObject AOE_portal;
    [Range(5f, 8f)][SerializeField] float portalCooldown = 5f;
    [Range(8, 15)][SerializeField] int stoppingDistance = 8;
    [SerializeField] ParticleSystem damageEffect;
    CharacterController playerController;
    int spawned;
    float portalTimer;
    float portalUpTime = 5f;
    float portalTimeEnd = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = gamemanager.instance.player.GetComponent<CharacterController>();
        if(playerController == null)
        {
            Debug.Log("PLAYER CONTROLLER IS NULL");
        }
        if(AOE_portal == null)
        {
            Debug.Log("PORTAL OBJECT HAS NOT BEEN SET");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnPortal(enemyAI _ai)
    {
        if (playerController == null) return;
        if (spawned == 0)
        {
            _ai.MoveStop();
            Vector3 playersFeet = playerController.transform.position - Vector3.up * (playerController.height/2f);

            GameObject AOE_instance = Instantiate(AOE_portal, playersFeet, Quaternion.identity);
            if (AOE_instance == null) return;
            BoxCollider col = AOE_portal.GetComponent<BoxCollider>();
            if (col == null) return;
            if(col.isTrigger)
            {
                ParticleSystem effects = Instantiate(damageEffect, AOE_instance.transform.position, Quaternion.identity);
                ++spawned;

                Destroy(AOE_instance, portalTimeEnd);
                if(effects != null)
                {
                    Destroy(effects, portalTimeEnd);
                }
                --spawned;

                portalTimer = 0;
            }
        }
    }
    public void Tick(enemyAI _ai)
    { 
        portalTimer += Time.deltaTime;
        float dist = _ai.PlayerDistance(); ;
        if (dist > stoppingDistance)
        {
            _ai.Chase(_ai.player);
            return;
        }
        if (portalTimer >= portalCooldown)
        {
            SpawnPortal(_ai);
        }

    }
}
