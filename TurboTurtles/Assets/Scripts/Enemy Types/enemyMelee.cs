using UnityEngine;

public class enemyMelee : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Melee Stats-----")]
    [Range(1, 3)][SerializeField] float range = 2f;
    float cooldown;

    public void Tick(enemyAI _ai)
    {
        float dist = _ai.PlayerDistance(); ;
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
