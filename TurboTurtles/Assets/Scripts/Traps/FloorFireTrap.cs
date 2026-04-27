using UnityEngine;
using System.Collections;

public class FloorFireTrap : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] ParticleSystem flameParticles;

    [Header("Trap Settings")]
    [SerializeField] float fireDuration = 2f;
    [SerializeField] float cooldown = 3f;
    [SerializeField] bool alwaysOn = false;
    [SerializeField] int damageAmount = 10;
    [SerializeField] float damageRate = 0.5f;

    private bool isFiring = false;
    private bool isDamaging = false;
    private float timer = 0f;

    void Start()
    {
        flameParticles.Stop();
        if (alwaysOn) StartFire();
    }

    void Update()
    {
        if (alwaysOn) return;

        timer += Time.deltaTime;

        if (!isFiring && timer >= cooldown)
            StartFire();
        else if (isFiring && timer >= fireDuration)
            StopFire();
    }

    void StartFire()
    {
        isFiring = true;
        timer = 0f;
        flameParticles.Play();
    }

    void StopFire()
    {
        isFiring = false;
        timer = 0f;
        flameParticles.Stop();
        StopAllCoroutines(); // clean stop
        isDamaging = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isFiring) return; // gate — no fire, no damage

        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && !isDamaging)
            StartCoroutine(DamageOverTime(dmg));
    }

    IEnumerator DamageOverTime(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}