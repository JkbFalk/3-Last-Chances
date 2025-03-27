using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Gloves_Duelist : Item
{
    public Gloves_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {
            new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="GainSharpOnBasicAttacksAndOnslaughtOnCounter", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.125f).ToString(), (GetFirstModifierEffectValue() * 0.2f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.125f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Sharp(effect.CustomParam, new(this)));
                })},
            new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="NoDescription", FlatAmount= GetFirstModifierEffectValue() * 0.2f, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
            ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))), 
            ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Onslaught(effect.FlatAmount, new(this)));
                })},
                };
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="RestoreHealthOnCounter", DescriptionParameters=new List<String>{Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 1.25f)}, CustomParam = GetSecondModifierEffectValue() * 1.25f, TriggersOncePerAbility = false, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += effect.CustomParam;
                })}};
    }
    /*public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_GainEffectWhileUnderAnotherEffect(typeof(Effect_Sharp), new(this)) { PercentageAmount=GetSecondModifierEffectValue() * 0.5f, EffectTypeName="SharpDamageReduction", DescriptionParameters=new List<String>{(GetSecondModifierEffectValue() * 0.5f).ToString()},
                CreateEffect = new Func<Effect_GainEffectWhileUnderAnotherEffect, Effect> ((effect) =>  {
                    return new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount=effect.PercentageAmount};
                })
        }};
    }*/
}
