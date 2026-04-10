using UnityEngine;

public class collectible : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] bool rotate;
    [SerializeField] float rotateSpeed = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gamemanager.instance != null)
        {
            for (int i = 0; i < value; i++)
            {
                gamemanager.instance.registerCollectible();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < value; i++)
            {
                gamemanager.instance.addCollectible();
            }

            Destroy(gameObject);
        }
    }
}
