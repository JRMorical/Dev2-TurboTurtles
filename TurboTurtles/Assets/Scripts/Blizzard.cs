using UnityEngine;
using System.Collections;

public class Blizzard : MonoBehaviour
{
    [Range(5, 35)][SerializeField] float radius = 5f;
    [Range(0.1f, 5)][SerializeField] float damageRate = 1f;
    [Range(0, 5)][SerializeField] int damage = 0;
    [Range(3, 8)][SerializeField] float duration = 5f;

    bool active;

    void Start()
    {
        active = true;
        StartCoroutine(DoDamage());
        Destroy(gameObject, duration);
    }

    IEnumerator DoDamage()
    {
        while (active)
        {
            ApplyDamage();
            yield return new WaitForSeconds(damageRate);
        }
    }

    void ApplyDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            IDamage dmg = hit.GetComponentInParent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }
        }
    }

    void OnDestroy()
    {
        active = false;
    }
}
