using UnityEngine;

public class enemyRanged : MonoBehaviour, IEnemyBehaviour
{
    [Header("-----Ranged Stats-----")]
    [Range(13, 25)][SerializeField] float rangedRange;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate = 1f;
    [SerializeField] Transform shootPos;
    float shootTimer;
    
    void Start()
    {
        if(shootPos == null)
        {
            Debug.Log("SHOOT POSTION NOT SET");
        }
    }

    public void Tick(enemyAI _ai)
    {
        float dist = _ai.PlayerDistance();
        if(dist > rangedRange)
        {
            _ai.Chase(_ai.player);
            return;
        }

        _ai.MoveStop();
        _ai.rotateToPlayer();
        shootPos.transform.LookAt(new Vector3(_ai.player.x, _ai.player.y, _ai.player.z));

        shootTimer += Time.deltaTime;

        if(shootTimer >= shootRate)
        {
            _ai.PlayProjectileAttack();
            Shoot();
            shootTimer = 0;
        }
    }
    void Shoot()
    {
        if (bullet != null)
        {
            Instantiate(bullet, shootPos.position, shootPos.rotation);
        }
        else
        {
            Debug.Log("BULLET NOT SET");
        }
    }
}
