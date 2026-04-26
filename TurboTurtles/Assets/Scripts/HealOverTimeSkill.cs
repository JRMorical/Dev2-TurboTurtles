using System.Collections;
using UnityEngine;

public class HealOverTimeSkill : BaseSkill
{
    public void ActivateSkill()
    {
        if (cooldownOn || isActive) return;

        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        Debug.Log("Heal Over Time Activated");

        isActive = true;

        StartCoroutine(CooldownEffect());

        for (int i = 0; i < 15; i++)
        {
            if (gamemanager.instance.player.GetComponent<playerController>().HP >= gamemanager.instance.player.GetComponent<playerController>().HPOrig)
                yield break;

            gamemanager.instance.player.GetComponent<playerController>().Heal(1);

            yield return new WaitForSeconds(1.0f);
        }

        isActive = false;
    }
}
