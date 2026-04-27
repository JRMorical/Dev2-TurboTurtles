using UnityEngine;

public class PortalLifetime : MonoBehaviour
{
    public enemyCaster owner;
    playerController playerController;
    int playerOrigSpeed;

    void Start()
    {
        playerController = gamemanager.instance.player.GetComponent<playerController>();
        if(playerController != null)
        {
            playerOrigSpeed = playerController.speed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerController pc = other.GetComponent<playerController>();
            if (pc != null)
            {
                pc.speed = 1;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerController pc = other.GetComponent<playerController>();
            if (pc != null)
            {
                pc.speed = playerOrigSpeed;
            }
        }
    }
    void OnDestroy()
    {
        if(owner != null)
        {
            owner.DecrementCount();
        }
    }
}
