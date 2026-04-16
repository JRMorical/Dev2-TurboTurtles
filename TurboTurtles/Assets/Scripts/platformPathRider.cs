using UnityEngine;

public class pathPlatformRider : MonoBehaviour
{
    [SerializeField] pathPlatform platform;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
                platform.SetRider(cc);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
                platform.ClearRider(cc);
        }
    }
}
