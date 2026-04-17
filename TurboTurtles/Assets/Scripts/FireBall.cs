using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 1;
    public float speed = 80f;
    public float lifetime = 4f;
    public ParticleSystem trailEffect;
    public ParticleSystem explosionEffect;
    public Vector3 targetPoint;

    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(),
            GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>());

        Vector3 dir = (targetPoint - transform.position).normalized;
        GetComponent<Rigidbody>().linearVelocity = dir * speed;

        if (trailEffect != null)
            trailEffect.Play();

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (trailEffect != null)
            trailEffect.Stop();

        IDamage dmg = col.collider.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
            gamemanager.instance.OnSuccessfulHit?.Invoke(col.gameObject, gameObject);
        }

        if (explosionEffect != null)
        {
            explosionEffect.transform.SetParent(null);
            explosionEffect.Play();
            Destroy(explosionEffect.gameObject, explosionEffect.main.duration + 0.5f);
        }

        Destroy(gameObject);
    }
}