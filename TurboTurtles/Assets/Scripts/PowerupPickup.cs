using UnityEngine;

public class powerupPickup : MonoBehaviour
{
    public enum PowerupType
    {
        InstaKill
    }

    [Header("Powerup")]
    [SerializeField] PowerupType powerupType = PowerupType.InstaKill;
    [SerializeField] float duration = 10f;
    [SerializeField] float lifeTime = 15f;
    [SerializeField] bool rotate = true;
    [SerializeField] float rotateSpeed = 90f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}