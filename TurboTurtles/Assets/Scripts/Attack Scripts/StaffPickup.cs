using UnityEngine;

public class StaffPickup : MonoBehaviour
{
    [SerializeField] SpellStats _stats;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerController player = other.GetComponent<playerController>();
        if (player == null) return;

        player.PickupSpell(_stats);
        Destroy(gameObject);
    }
}
