using UnityEngine;
using System.Collections;

public class DamageReductionSkill : BaseSkill
{
    public playerController player;

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

        player.damageReductionMultiplier = reductionMultiplier;

        yield return new WaitForSeconds(duration);

        player.damageReductionMultiplier = 1f;

        isActive = false;

        StartCoroutine(CooldownEffect());
    }
}
