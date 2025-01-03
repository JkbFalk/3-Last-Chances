using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_SlicingGale : Item
{
    public TwinBlades_SlicingGale(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(80, 80, 1.4f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Analysis)},EffectTypeName="ApplyAnalyzedOnBAs", CustomParam = GetFirstModifierEffectValue() * 0.1875f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.1875f)}, ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack && damage.AbilityDamageSource.DamageType == Constants.DamageType.Light)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Analysis(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType.Light, 1, 1, new(this))};
    }
}

