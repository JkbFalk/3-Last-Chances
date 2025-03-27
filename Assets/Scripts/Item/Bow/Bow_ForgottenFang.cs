using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_ForgottenFang : Item
{
    public Bow_ForgottenFang(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(80, 160, 0.6f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="RangedDamageAppliesSleep", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.125f)}, CustomParam = GetFirstModifierEffectValue(false) * 0.125f, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.AbilityDamageSource.DamageType == Constants.DamageType.Ranged && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && damage.DamagingObject != null && damage.DamagingObject is Projectile)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.DamageDealtMultiplier += effect.CustomParam * Vector2.Distance(damage.DamagingObject.transform.position, ((Projectile)damage.DamagingObject).StartLocation);
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Control, new(this)) {PercentageAmount = 0.5f}};
    }
}
