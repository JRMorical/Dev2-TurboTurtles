using System.Collections;
using UnityEngine;

public class ShieldSkill : BaseSkill
{
    public float damageReductionMultiplier = 0;
    public GameObject shieldPrefab;

    public void ActivateSkill()
    {
        if (cooldownOn || isActive) return;

        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        Debug.Log("Magic Shield Activate");

        isActive = true;

        GameObject shield = Instantiate(shieldPrefab, gamemanager.instance.player.transform.position, Quaternion.identity);

        shield.transform.SetParent(gamemanager.instance.player.transform);

        shield.transform.localPosition = Vector3.zero;

        gamemanager.instance.player.GetComponent<playerController>().damageReductionMultiplier = damageReductionMultiplier;

        yield return new WaitForSeconds(duration);

        gamemanager.instance.player.GetComponent<playerController>().damageReductionMultiplier = 1;

        Destroy(shield);

        isActive = false;

        StartCoroutine(CooldownEffect());
    }
}
