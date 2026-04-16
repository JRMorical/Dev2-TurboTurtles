using UnityEngine;
using System.Collections;

public class WispManager : MonoBehaviour
{
    public static WispManager instance;

    public enum WispType
    {
        Flame,
        Frost,
        Gale,
        Thunder
    }

    public WispType currentWisp;

    int galeHitCount;
    bool galeBuffActive;

    Coroutine frostRoutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gamemanager.instance.OnSuccessfulHit += HandleHit;

        frostRoutine = StartCoroutine(FrostLoop());
    }

    void HandleHit(GameObject target, GameObject source)
    {
        switch (currentWisp)
        {
            case WispType.Flame:
                FlameWisp(target);
                break;

            case WispType.Gale:
                GaleWisp();
                break;

            case WispType.Thunder:
                StartCoroutine(ThunderWisp(target));
                break;
        }
    }

    // Flame Wisp
    void FlameWisp(GameObject target)
    {
        if (Random.value < 0.2f)
        {
            playerController pc = gamemanager.instance.player.GetComponent<playerController>();
            if (pc != null)
            {
                pc.Heal(2);
            }
        }
    }

    // Gale Wisp
    void GaleWisp()
    {
        galeHitCount++;

        if (galeHitCount >= 3 && !galeBuffActive)
        {
            StartCoroutine(GaleBuff());
            galeHitCount = 0;
        }
    }

    IEnumerator GaleBuff()
    {
        galeBuffActive = true;

        float timer = 10f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        galeBuffActive = false;
    }

    // Thunder Wisp
    IEnumerator ThunderWisp(GameObject target)
    {
        EnemyBrain brain = target.GetComponent<EnemyBrain>();

        if (brain != null)
        {
            brain.SetStoppingDistance(0.2f);

            yield return new WaitForSeconds(6f);

            yield return new WaitForSeconds(1.5f);

            brain.SetStoppingDistance(2f);
        }
    }

    // Frost Wisp Loop
    IEnumerator FrostLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (currentWisp != WispType.Frost)
                continue;

            Vector3 origin = gamemanager.instance.player.transform.position;

            origin += gamemanager.instance.player.transform.forward * 1.5f;
            origin += Vector3.up * 2f;

            RaycastHit hit;

            if (Physics.Raycast(origin, Vector3.down, out hit, 10f))
            {
                if (hit.collider.CompareTag("Player"))
                    yield break;

                GameObject ice = GameObject.CreatePrimitive(PrimitiveType.Cube);

                ice.transform.position = hit.point + Vector3.up * 0.01f;
                ice.transform.localScale = new Vector3(3, 0.1f, 3);

                Destroy(ice.GetComponent<Collider>());
                Destroy(ice, 15f);
            }
        }
    }
}
