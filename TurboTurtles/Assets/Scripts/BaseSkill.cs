using UnityEngine;
using System.Collections;

public class BaseSkill : MonoBehaviour
{
    public float duration = 15f;
    public float cooldown = 30f;

    protected bool isActive = false;
    protected bool cooldownOn = false;

    protected IEnumerator CooldownEffect()
    {
        cooldownOn = true;
        yield return new WaitForSeconds(cooldown);
        cooldownOn = false;
    }
}
