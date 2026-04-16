using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class DamageBoostSkill : BaseSkill
{
    public playerController player;
    public int boostAmount = 10;

    public void ActivateSkill()
    {
        if (cooldownOn || isActive) return;

        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        Debug.Log("Damage Boost Activated");

        isActive = true;

        player.shootDamage += boostAmount;

        yield return new WaitForSeconds(duration);

        player.shootDamage -= boostAmount;

        isActive = false;

        StartCoroutine(CooldownEffect());
    }
}
