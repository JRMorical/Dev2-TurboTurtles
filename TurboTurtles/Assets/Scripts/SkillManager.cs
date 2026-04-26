using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public DamageBoostSkill damageBoost;
    public DamageReductionSkill damageReduction;
    public HealOverTimeSkill healOverTime;
    public ShieldSkill magicShield;

    public void UseDamageBoost()
    {
        damageBoost.ActivateSkill();
    }

    public void UseDamageReduction()
    {
        damageReduction.ActivateSkill();
    }

    public void UseHealOverTime()
    {
        healOverTime.ActivateSkill();
    }

    public void UseMagicShield()
    {
        magicShield.ActivateSkill();
    }
}
