using UnityEngine;
using System.Collections;

public class DamageReductionSkill : BaseSkill
{

    public float reductionMultiplier = 0.5f;

  public void ActivateSkill()
    {
        if (cooldownOn || isActive) return;

        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        Debug.Log("Damage Reduction Activated");

        isActive = true;

        StartCoroutine(CooldownEffect());

        gamemanager.instance.player.GetComponent<playerController>().damageReductionMultiplier = reductionMultiplier;

        yield return new WaitForSeconds(duration);

        gamemanager.instance.player.GetComponent<playerController>().damageReductionMultiplier = 1f;

        isActive = false;
    }
}
