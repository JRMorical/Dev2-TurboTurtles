using UnityEngine;

public class PortalFX : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    Vector2 rotation;
    Renderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = new Material(renderer.material);
        rotation = renderer.material.mainTextureOffset;
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x += Time.deltaTime * speed;
        renderer.material.mainTextureOffset = rotation;
    }
}
