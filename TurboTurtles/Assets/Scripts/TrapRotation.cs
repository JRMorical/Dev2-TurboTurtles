using UnityEngine;

public class rotateObject : MonoBehaviour
{
    [SerializeField] Transform model;
    [SerializeField] int rotateSpeed;
    [SerializeField] ParticleSystem flameParticles; // drag in Inspector

    private float currentXRotation = 0f;
    private float direction = 1f;
    private const float maxAngle = 45f;

    void Update()
    {
        currentXRotation += direction * rotateSpeed * Time.deltaTime;

        if (currentXRotation >= maxAngle)
        {
            currentXRotation = maxAngle;
            direction = -1f;
        }
        else if (currentXRotation <= -maxAngle)
        {
            currentXRotation = -maxAngle;
            direction = 1f;
        }

        model.localRotation = Quaternion.Euler(currentXRotation, model.localEulerAngles.y, 0f);
    }
}