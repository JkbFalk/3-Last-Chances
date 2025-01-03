using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Effect;

public class Effect_StrongBAsDealStaggerBasedOnEnemyStaggerBar : Effect
{
    public float[] MinSB = {500, 850, 1250, 1750, 2500};
    public float[] MaxSB = {2000, 3400, 5000, 7000, 10000};
    float StaggerAmount = 0;
    public int Grade = 0;
    public Effect_StrongBAsDealStaggerBasedOnEnemyStaggerBar(float stagger_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        StaggerAmount = stagger_amount;
        Listeners.Add(EventManager.HitDealt);
    }


    public override void OnEffectValueChanged()
    {
        StaggerAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(StaggerAmount), Utils.GetFormattedFloat(StaggerAmount * 4), MinSB[Grade].ToString(), MaxSB[Grade].ToString()};
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsStrongBasicAttack)
        {
            float actualStagger = Utils.GetValueBasedOnMinAndMax(damage.TargetOfDamage.Health.Maximum, MinSB[Grade], MaxSB[Grade], StaggerAmount, StaggerAmount*4);
            damage.Stagger += actualStagger;
            base.OnInvokeDamageDealt(damage);
        }
    }
}
 