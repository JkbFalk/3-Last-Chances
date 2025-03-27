using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_WeaponMaster : Item
{
    public Gloves_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="RestoreHealthOnWeaponDamage", DescriptionParameters=new List<String>{Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.3125f)}, CustomParam = GetFirstModifierEffectValue() * 0.3125f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.IsWeaponDamage),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += effect.CustomParam;
                })}};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, new(this)) {PercentageAmount = 0.625f}, new Effect_ChangeStat(Player.Instance.LightAttackSpeed, new(this)) {PercentageAmount = 0.625f}, new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(this)) {PercentageAmount = 0.625f}  };
    }
}
