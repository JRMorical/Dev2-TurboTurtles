using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class DamageBoostSkill : BaseSkill
{
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
        
        StartCoroutine(CooldownEffect());

        playerController pc = gamemanager.instance.player.GetComponent<playerController>();
        if (pc != null)
        {
            pc.bonusDamage += boostAmount;

            yield return new WaitForSeconds(duration);
            pc.bonusDamage -= boostAmount;
        }
        
        isActive = false; 
    }
}
