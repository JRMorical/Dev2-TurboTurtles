using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public DamageBoostSkill damageBoost;
    public DamageReductionSkill damageReduction;

    public void UseDamageBoost()
    {
        damageBoost.ActivateSkill();
    }

    public void UseDamageReduction()
    {
        damageReduction.ActivateSkill();
    }
}
