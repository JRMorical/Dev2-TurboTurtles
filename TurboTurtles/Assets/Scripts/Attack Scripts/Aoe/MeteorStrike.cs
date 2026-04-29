using UnityEngine;
using System.Collections;

public class MeteorStrike : MonoBehaviour
{
    [Header("Meteor Settings")]
    [SerializeField] float fallSpeed = 20f;
    [SerializeField] float warningTime = 1.5f;
    [SerializeField] float impactRadius = 5f;
    [SerializeField] int damageAmount = 100;
    [SerializeField] float impactForce = 500f;

    [Header("Effects")]
    [SerializeField] GameObject warningDecal;
    [SerializeField] ParticleSystem impactEffect;
    [SerializeField] AudioClip impactSound;

    private Vector3 impactCenter;
    private bool hasImpacted = false;

    public void SetTarget(Vector3 groundPoint)
    {
        impactCenter = groundPoint;
        transform.position = groundPoint + Vector3.up * 20f;

        Transform warningRing = transform.Find("Circle"); 
        if (warningRing != null)
            warningRing.position = new Vector3(groundPoint.x, groundPoint.y + 0.1f, groundPoint.z);

        StartCoroutine(FallDelay());
    }

    void Start() { }

    IEnumerator FallDelay()
    {
        yield return new WaitForSeconds(warningTime);
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        while (!hasImpacted)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, impactCenter, fallSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, impactCenter) < 0.5f)
            {
                hasImpacted = true;
                Impact();
            }
            yield return null;
        }
    }

    void Impact()
    {
        Collider[] hits = Physics.OverlapSphere(impactCenter, impactRadius);

        foreach (Collider hit in hits)
        {
            if (hit.isTrigger) continue;
            if (hit.CompareTag("Player")) continue;

            IDamage dmg = hit.GetComponent<IDamage>() ?? hit.GetComponentInParent<IDamage>();
            if (dmg != null)
                dmg.takeDamage(damageAmount);

            Rigidbody rb = hit.GetComponent<Rigidbody>() ?? hit.GetComponentInParent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(impactForce, impactCenter, impactRadius);
        }

        if (impactEffect != null)
        {
            impactEffect.transform.position = impactCenter;
            impactEffect.Play();
        }

        if (impactSound != null)
            AudioSource.PlayClipAtPoint(impactSound, impactCenter);

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(impactCenter, impactRadius);
    }
}