using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_IronRule : Item
{
    public Greatsword_IronRule(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(40, 160, 0.6f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="ReducedFlatDamageDuringBasicAttacks", DescriptionParameters = new List<String> { (GetFirstModifierEffectValue() * 6.25f / 2).ToString(), (GetFirstModifierEffectValue() * 6.25f * 2).ToString()}, CustomParam = GetFirstModifierEffectValue() * 6.25f, ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null && Player.Instance.Actions.CurrentAbilityBeingPerformed.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury -= effect.CustomParam / 2;
                    damage.Stagger -= effect.CustomParam * 2;
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) { HasLinearScaling=false, PercentageAmount = -0.125f} , new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount=0.625f} };
    }
}
