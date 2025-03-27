using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daggers_VipersFangs : Item
{
    public Daggers_VipersFangs(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.Daggers;
        SetBaseWeaponStats(60, 70, 1.35f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplyPoisonOnBA", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.05f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.05f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            damage.TargetOfDamage.AddEffect(new Effect_Poison(effect.CustomParam, new(this)));
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplyProneOnBA", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.05f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.05f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            damage.TargetOfDamage.AddEffect(new Effect_Prone(effect.CustomParam, new(this)));
        })}};
    }
}

