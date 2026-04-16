using UnityEngine;
using System.Collections;

public class EnemyMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EnemyBrain brain;

    [Header("Attack")]
    [SerializeField] int damage = 1;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackRange = 2f;

    bool canAttack = true;

    void Start()
    {
        if (brain == null)
            brain = GetComponent<EnemyBrain>();

        brain.SetStoppingDistance(attackRange - 0.2f);
    }

    void Update()
    {
        if (!brain.HasTarget)
            return;

        if (brain.InAttackRange && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        brain.StopMoving();

        IDamage dmg = brain.Target.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);

            gamemanager.instance.OnSuccessfulHit?.Invoke(brain.Target.gameObject, gameObject);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}