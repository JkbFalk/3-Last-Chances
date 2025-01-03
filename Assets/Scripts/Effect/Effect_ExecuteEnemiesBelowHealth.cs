using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_ExecuteEnemiesBelowHealth : Effect
{
    public float HealthAmount = 0;
    public Constants.DamageType DamageType;
    public Effect_ExecuteEnemiesBelowHealth(Constants.DamageType damage_type, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DamageType = damage_type;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        HealthAmount = 7.5f * LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthAmount), Label.Get("WeaponCategory_" + DamageType)};
    }

    public override void OnInvokeDamageDealt( Damage damage)
    {
        if(damage.SourceOfDamage.User == TargetOfEffect && damage.AbilityDamageSource.DamageType == DamageType && damage.TargetOfDamage.Health.Current < HealthAmount)
        {
            Damage d = new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(TargetOfEffect), damage.DamagingObject) {AbilityDamageSource = new(0, 0, Constants.DamageType.None), Injury = Constants.EXECUTE_DAMAGE_AMOUNT};
            d.CalculateDamage();
            base.OnInvokeDamageDealt(damage);
        }
    }
}
