using UnityEngine;

public class GravityLift : MonoBehaviour
{
    public enum LiftMode
    {
        Vertical,       
        Angled          
    }

    [Header("Lift Settings")]
    [SerializeField] LiftMode mode = LiftMode.Vertical;
    [SerializeField] float launchForce = 18f;
    [SerializeField] float angleDirection = 0f; 

    [Header("Feel")]
    [SerializeField] float cancelGravityDuration = 0.8f; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerController player = other.GetComponent<playerController>();
        CharacterController cc = other.GetComponent<CharacterController>();

        if (player == null || cc == null) return;

        StartCoroutine(LaunchPlayer(player, cc));
    }

    private System.Collections.IEnumerator LaunchPlayer(playerController player, CharacterController cc)
    {
        Vector3 launchVelocity = CalculateLaunchVelocity();

        float elapsed = 0f;

        while (elapsed < cancelGravityDuration)
        {
          
            float t = elapsed / cancelGravityDuration;
            Vector3 frameMove = Vector3.Lerp(launchVelocity, Vector3.zero, t) * Time.deltaTime;
            cc.Move(frameMove);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private Vector3 CalculateLaunchVelocity()
    {
        switch (mode)
        {
            case LiftMode.Angled:
                
                Quaternion horizontalRot = Quaternion.Euler(0f, angleDirection, 0f);
                Vector3 baseDir = horizontalRot * Vector3.forward;
               
                return (baseDir + Vector3.up).normalized * launchForce;

            case LiftMode.Vertical:
            default:
                return Vector3.up * launchForce;
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = mode == LiftMode.Vertical ? Color.cyan : Color.yellow;
        Gizmos.DrawRay(transform.position, CalculateLaunchVelocity().normalized * 3f);
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}