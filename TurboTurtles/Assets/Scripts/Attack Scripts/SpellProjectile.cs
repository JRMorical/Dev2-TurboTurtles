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
    float spawnTimer = 0f;

    public void Init(Vector3 target, int dmg, float spd, float slow, float slowDur)
    {
        damage = dmg;
        speed = spd;
        slowAmount = slow;
        slowDuration = slowDur;
        dir = (target - transform.position).normalized;

        GetComponent<Rigidbody>().linearVelocity = dir * speed;

        if (trailEffect != null)
            trailEffect.Play();

        float lifetime = Mathf.Max(Vector3.Distance(transform.position, target) / speed, 3f);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasHit) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < 0.2f) return;

        // Raycast ahead
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, speed * Time.deltaTime * 6f))
        {
            if (!hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            {
                HitTarget(hit.collider);
                return;
            }
        }

        // OverlapSphere as backup
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player")) continue;
            if (col.gameObject == gameObject) continue;

            HitTarget(col);
            return;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (hasHit) return;
        if (spawnTimer < 0.2f) return;
        if (col.collider.CompareTag("Player")) return;

        HitTarget(col.collider);
    }

    void HitTarget(Collider col)
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
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + 2f);
        }

        Destroy(gameObject);
    }
}