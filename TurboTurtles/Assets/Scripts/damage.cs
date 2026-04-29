using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    int DamageAmount => damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;
    bool hasHit;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 testPos = transform.position + transform.forward * 5f;
            testPos = new Vector3(testPos.x, 0f, testPos.z);

            Instantiate(hitEffect, testPos, Quaternion.identity);

            Debug.Log("Test FX spawned at: " + testPos);
        }
    }
    void Start()
    {
        if (type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);
        }
    }
    void OnParticleCollision(GameObject other)
    {
        if (type != damageType.stationary) return;
       // if (hasHit) return;

        hasHit = true;

        IDamage dmg = other.GetComponentInParent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
            gamemanager.instance.OnSuccessfulHit?.Invoke(other, gameObject);
        }

    }
    public void DealDamage(GameObject target)
    {
        if (target == null) return;

        IDamage dmg = target.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
            gamemanager.instance.OnSuccessfulHit?.Invoke(target, gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Ranged") || other.CompareTag("Melee") || other.CompareTag("Spawner")
            || other.CompareTag("Caster") || other.CompareTag("Charger"))
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);
            gamemanager.instance.OnSuccessfulHit?.Invoke(other.gameObject, gameObject);
        }

        if (type == damageType.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}