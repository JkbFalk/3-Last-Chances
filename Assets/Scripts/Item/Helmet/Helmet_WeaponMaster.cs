using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_WeaponMaster : Item
{
    public Helmet_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {CustomParam =  GetFirstModifierEffectValue() * 0.9375f,EffectTypeName="WeaponTechniquesDealMoreDamage", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.9375f, 0)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && (damage.AbilityDamageSource.DamageType == Constants.DamageType.Heavy || damage.AbilityDamageSource.DamageType == Constants.DamageType.Light || damage.AbilityDamageSource.DamageType == Constants.DamageType.Ranged)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.ExtraInjuryDealtPercentage += effect.CustomParam;
                    damage.ExtraStaggerDealtPercentage += effect.CustomParam;
                })} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 10 } };
    }
}

