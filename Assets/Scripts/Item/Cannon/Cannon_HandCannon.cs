using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_HandCannon : Item
{
    public Cannon_HandCannon(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="TakeStaggerOnBasicAttackButDealMoreDamage", CustomParam = GetFirstModifierEffectValue() * 1.5625f, DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 1.5625f), Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 1.5625f * 2), Utils.GetFormattedFloat( GetFirstModifierEffectValue() * 1.5625f)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury += effect.CustomParam;
                    damage.Stagger += 2 * effect.CustomParam;
                })},
            new Effect_CustomizableEffectOnEvent(new(this)) { EffectTypeName="NoDescription", FlatAmount = GetFirstModifierEffectValue() * 1.5625f, ConditionCheckForProjectileCreated = new Func<Projectile, bool>((proj) => 
                    (proj.SourceAbility.User == Player.Instance && proj.SourceAbility.IsBasicAttack)), ActionOnProjectileCreated = new Action<Projectile, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Ability damageAbility = new Ability_SourcelessDamage(Player.Instance);
                    new Damage(Player.Instance, damageAbility, null).SetDamageSource(0, effect.FlatAmount, Constants.DamageType.None).CalculateDamage();
                    Player.Instance.ApplyForce(Player.Instance.Actions.IsFlipped ? Vector2.right * 450 : Vector2.left * 450, damageAbility);
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 10 }};
    }
}
