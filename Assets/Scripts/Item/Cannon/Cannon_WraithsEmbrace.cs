using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_WraithsEmbrace : Item
{
    public Cannon_WraithsEmbrace(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="TakeStaggerOnBasicAttackButDealMoreDamage", CustomParam = GetFirstModifierEffectValue() * 1.5625f, DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 1.5625f), Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 1.5625f * 2), Utils.GetFormattedFloat( GetFirstModifierEffectValue() * 1.5625f)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury += effect.CustomParam;
                    damage.Stagger += 2 * effect.CustomParam;
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 10 }};
    }
}
