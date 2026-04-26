using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public DamageBoostSkill damageBoost;
    public DamageReductionSkill damageReduction;
    public HealOverTimeSkill healOverTime;

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
}
