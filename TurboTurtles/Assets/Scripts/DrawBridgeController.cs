using UnityEngine;
using System.Collections;

public class DrawBridgeController : MonoBehaviour
{
    [SerializeField] Vector3 closedRotation;
    [SerializeField] Vector3 openRotation;
    [SerializeField] float rotateSpeed = 60f;

    Coroutine rotateRoutine;

    void Start()
    {
        transform.rotation = Quaternion.Euler(closedRotation);
    }

    public void LowerBridge()
    {
        RotateTo(openRotation);
    }

    public void CloseBridge()
    {
        RotateTo(closedRotation);
    }

    void RotateTo(Vector3 targetEuler)
    {
        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(RotateRoutine(targetEuler));
    }

    IEnumerator RotateRoutine(Vector3 targetEuler)
    {
        Quaternion targetRot = Quaternion.Euler(targetEuler);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRot;
        rotateRoutine = null;
    }
}