using UnityEngine;

public class PortalLifetime : MonoBehaviour
{
    public enemyCaster owner;
    playerController pc;
    int playerOrigSpeed;
    bool playerInside;

    void Start()
    {
        pc = gamemanager.instance.player.GetComponent<playerController>();
        if(pc != null)
        {
            playerOrigSpeed = pc.speed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pc = other.GetComponent<playerController>();
            if (pc == null) return;
            if(!playerInside)
            {
                pc.speed /= 2;
                playerInside = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pc = other.GetComponent<playerController>();
            if (pc == null) return;
            pc.speed = playerOrigSpeed;
            playerInside = false;
        }
    }
    void OnDestroy()
    {
        if(pc != null && playerInside)
        {
            pc.speed = playerOrigSpeed;
        }

        if(owner != null)
        {
            owner.DecrementCount();    
        }
    }
}
