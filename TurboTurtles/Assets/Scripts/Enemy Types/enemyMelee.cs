using UnityEngine;

public class enemyMelee : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField] float range = 2f;
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
            _ai.moveTo(_ai.player);
            return;
        }
        _ai.moveStop();
        _ai.rotateToPlayer();
        cooldown -= Time.deltaTime;
        if(cooldown <= 0)
        {
            _ai.playMeleeAttack();
            cooldown = 0.5f;
        }
    }
}
