using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_TwinSymphony : Item
{
    public Gun_TwinSymphony(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(125, 125, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("InjuryAndStaggerScaleWithEachOther")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RangedDamage")};
    }

    /*
    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="DealExtraDamageWithOtherDamage", FirstParameter = GetItemFirstEffectPB() * 0.75f, DescriptionParameters = new List<String> {"10", "[RI]", "[RS]", Utils.GetFormattedFloat(GetItemFirstEffectPB() * 0.75f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.ScalesWith == Constants.DamageType.Ranged)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.InjuryDealt > 0 && !(damage.SourceOfDamage is Ability_SourcelessDamage)) {
                        new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(damage.SourceOfDamage.User), null).SetDamageSource(0, damage.InjuryDealt * 0.1f > effect.FirstParameter ? effect.FirstParameter : damage.InjuryDealt * 0.1f, Constants.DamageType.None).CalculateDamage();
                    }
                })}, 
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="DealExtraDamageWithOtherDamage", FirstParameter = GetItemFirstEffectPB() * 0.75f, DescriptionParameters = new List<String> {"10", "[RS]", "[RI]", Utils.GetFormattedFloat(GetItemFirstEffectPB() * 0.75f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.ScalesWith == Constants.DamageType.Ranged)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.StaggerDealt > 0 && !(damage.SourceOfDamage is Ability_SourcelessDamage)) {
                        new Damage(damage.TargetOfDamage, new Ability_SourcelessDamage(damage.SourceOfDamage.User), null).SetDamageSource(damage.StaggerDealt * 0.1f > effect.FirstParameter ? effect.FirstParameter : damage.StaggerDealt * 0.1f, 0, Constants.DamageType.None).CalculateDamage();
                    }
                })}};
    }
    */
}
