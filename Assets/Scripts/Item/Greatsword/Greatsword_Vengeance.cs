using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Vengeance : Item
{
    public Greatsword_Vengeance(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(115, 115, 0.8f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplyOnslaughtOnBAs", CustomParam = GetFirstModifierEffectValue() * 0.25f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.25f)}, ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && damage.AbilityDamageSource.DamageType == Constants.DamageType.Heavy)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Onslaught(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(this)) {RemainsActiveInOtherStances = true, PercentageAmount = 0.25f}};
    }
}
