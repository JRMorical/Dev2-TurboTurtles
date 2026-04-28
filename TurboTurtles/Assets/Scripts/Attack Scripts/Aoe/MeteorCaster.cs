using UnityEngine;

public class MeteorCaster : MonoBehaviour
{
    [SerializeField] AudioClip castSound;
    [SerializeField] GameObject meteorUI;

    private GameObject meteorPrefab;
    private bool isArmed = false;

    void Start()
    {
        if (meteorUI != null)
            meteorUI.SetActive(false);
    }

    public void ArmMeteor(GameObject prefab)
    {
        meteorPrefab = prefab;
        isArmed = true;

        if (meteorUI != null)
            meteorUI.SetActive(true);
    }

    void Update()
    {
        if (!isArmed) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~LayerMask.GetMask("Player")))
            {
                GameObject meteor = Instantiate(meteorPrefab, hit.point, Quaternion.identity);
                meteor.GetComponent<MeteorStrike>().SetTarget(hit.point);

                if (castSound != null)
                    AudioSource.PlayClipAtPoint(castSound, hit.point);

                isArmed = false;

                if (meteorUI != null)
                    meteorUI.SetActive(false);
            }
        }
    }
}