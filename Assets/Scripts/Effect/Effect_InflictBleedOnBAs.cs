using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_InflictBleedOnBAs : Effect
{
    public float BleedAmount = 0;
    public Effect_InflictBleedOnBAs(float bleed_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.DamageDealt);
        BleedAmount = bleed_amount;
    }

    public override void OnEffectValueChanged()
    {
        BleedAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BleedAmount, 1)};
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsBasicAttack && damage.IsDamageOverTime == false)
        {
            damage.TargetOfDamage.AddEffect(new Effect_Bleed(damage.SourceOfDamage.IsStrongBasicAttack ? BleedAmount * 2 : BleedAmount, SourceOfEffect));
            base.OnInvokeDamageDealt(damage);
        }
    }
}
