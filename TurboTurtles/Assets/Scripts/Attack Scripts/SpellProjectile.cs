using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public ParticleSystem trailEffect;
    public ParticleSystem explosionEffect;

    int damage;
    float speed;
    float slowAmount;
    float slowDuration;
    Vector3 dir;
    bool hasHit = false;

    public void Init(Vector3 target, int dmg, float spd, float slow, float slowDur)
    {
        damage = dmg;
        speed = spd;
        slowAmount = slow;
        slowDuration = slowDur;

        Vector3 adjustedTarget = new Vector3(target.x, target.y + 1f, target.z);
        dir = (adjustedTarget - transform.position).normalized;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        if (trailEffect != null)
            trailEffect.Play();

        float lifetime = Mathf.Max(Vector3.Distance(transform.position, target) / speed, 3f);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasHit) return;

        transform.position += dir * speed * Time.deltaTime;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.2f, dir, out hit, speed * Time.deltaTime * 3f))
        {
            if (hit.collider.CompareTag("Player")) return;
            if (hit.collider.gameObject == gameObject) return;
            HitTarget(hit.collider, hit.point);
        }
    }

    void HitTarget(Collider col, Vector3 hitPoint)
    {
        if (hasHit) return;
        hasHit = true;

        if (trailEffect != null)
            trailEffect.Stop();

        IDamage dmg = col.GetComponentInParent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
            gamemanager.instance.OnSuccessfulHit?.Invoke(col.gameObject, gameObject);
        }

        enemyAI enemy = col.GetComponentInParent<enemyAI>();
        if (enemy != null)
            enemy.ApplySlow(slowAmount, slowDuration);

        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, hitPoint, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + 2f);
        }

        Destroy(gameObject);
    }
}