using UnityEngine;
using System.Collections;

public class enemyCharger : MonoBehaviour, IEnemyBehaviour
{
    enum AttackState
    {
        IDLE,
        WINDING_UP,
        CHARGING
    }

    [Header("-----Charger Stats-----")]
    AttackState state;

    Vector3 chargeDirection;

    float chargeSpeed = 20f;
    float chargeDuration = 1.2f;
    float chargeTime;

    float chargeCooldown = 3f;
    float chargeTimer;

    float chargeRange = 10f;
    float attackRange = 2.5f;

    bool canCharge = true;
    bool isHitting;


    public void Tick(enemyAI _ai)
    {
        switch (state)
        {
            case AttackState.WINDING_UP:
                _ai.rotateToPlayer();
                return;

            case AttackState.CHARGING:
                UpdateCharge(_ai);
                return;
        }

        _ai.Chase(_ai.player);

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeCooldown && canCharge)
        {
            if (_ai.PlayerDistance() < chargeRange)
            {
                chargeTimer = 0;
                canCharge = false;
                StartCoroutine(Windup(_ai));
            }
        }
    }
    IEnumerator Windup(enemyAI _ai)
    {
        state = AttackState.WINDING_UP;

        _ai.MoveStop();
        _ai.rotateToPlayer();

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));

        chargeDirection = (_ai.player - _ai.transform.position).normalized;

        chargeTime = 0;
        state = AttackState.CHARGING;
    }
    void UpdateCharge(enemyAI _ai)
    {
        chargeTime += Time.deltaTime;

        _ai.transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

        if (!isHitting && _ai.PlayerDistance() <= attackRange)
        {
            StartCoroutine(HitDuringCharge(_ai));
        }

        if (chargeTime >= chargeDuration)
        {
            EndCharge();
        }
    }
    IEnumerator HitDuringCharge(enemyAI _ai)
    {
        isHitting = true;

        _ai.PlayMeleeAttack();

        yield return new WaitForSeconds(0.4f); 

        isHitting = false;
    }
    void EndCharge()
    {
        chargeTime = 0;
        chargeTimer = 0;
        canCharge = true;
        state = AttackState.IDLE;
    }
}
