using UnityEngine;
using System.Collections;

public class DrawBridgeController : MonoBehaviour
{
    [SerializeField] Vector3 closedRotation;
    [SerializeField] Vector3 openRotation;
    [SerializeField] float rotateSpeed = 60f;

    bool isOpening = false;

    public void LowerBridge()
    {
        if (!isOpening)
            StartCoroutine(LowerRoutine());
    }

    IEnumerator LowerRoutine()
    {
        isOpening = true;

        Quaternion targetRot = Quaternion.Euler(openRotation);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRot;
        isOpening = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Quaternion.Euler(closedRotation);   
    }
}
