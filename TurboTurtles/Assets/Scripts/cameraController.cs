using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens; //Sensitivity
    [SerializeField] int lockVertMin, lockVertMax; //Prevents the camera from going too far over the top and bottom.
    [SerializeField] bool invertY; //Allows for inverted controls.
    [SerializeField] Transform player;

    float camRotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

        player.transform.Rotate(Vector3.up * mouseX);
    }
}
