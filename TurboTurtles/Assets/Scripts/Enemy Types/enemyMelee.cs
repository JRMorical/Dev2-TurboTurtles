using UnityEngine;

public class enemyMelee : MonoBehaviour, IEnemyBehaviour
{
    [Range(1, 3)][SerializeField] float range = 2f;
    float cooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Tick(enemyAI _ai)
    {
        float dist = Vector3.Distance(_ai.transform.position, _ai.player);
        if(dist > range)
        {
            _ai.Chase(_ai.player);
            return;
        }
        _ai.MoveStop();
        _ai.rotateToPlayer();
        cooldown -= Time.deltaTime;
        if(cooldown <= 0)
        {
            _ai.PlayMeleeAttack();
            cooldown = 0.5f;
        }
    }
}
