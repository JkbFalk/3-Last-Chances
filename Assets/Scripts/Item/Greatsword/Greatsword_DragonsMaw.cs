using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_DragonsMaw : Item
{
    public Greatsword_DragonsMaw(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(100, 160, 0.7f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { UsesTheFollowingEffects=new() {typeof(Effect_Burn)},RemainsActiveInOtherStances = true, CustomParam =  GetFirstModifierEffectValue() * 0.16f,EffectTypeName="HeavyDamageAppliesBurn", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.16f, 0)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Heavy),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Burn(effect.CustomParam, new(this)));
                })}, new Effect_BlazingShadowWatchesYourBack(new(this)) {BurningInflicted = GetFirstModifierEffectValue() * 0.1f, DecreasedBackstabDamageTaken = GetFirstModifierEffectValue() * 0.5f} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, GetSecondModifierEffectValue() * 1.25f, new(this)) {EffectTypeName="BurnPowerWhileBelowNHealth", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 1.25f, 0), "50"}, ConditionForEffectPowerChange = new Func<bool>(() => Player.Instance.Health.Current < Player.Instance.Health.Maximum * 0.5f)} };
    }
}
