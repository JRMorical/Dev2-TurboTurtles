using UnityEngine;

public class MeteorPickup : MonoBehaviour
{
    [SerializeField] GameObject meteorStrikePrefab;
    bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        MeteorCaster caster = other.GetComponent<MeteorCaster>();
        if (caster != null)
        {
            collected = true;
            caster.ArmMeteor(meteorStrikePrefab);
            Destroy(gameObject);
        }
    }
}