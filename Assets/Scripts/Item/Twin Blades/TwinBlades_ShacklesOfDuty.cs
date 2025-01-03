using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_ShacklesOfDuty : Item
{
    public TwinBlades_ShacklesOfDuty(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(120, 40, 1.25f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Chained)},EffectTypeName="ApplyChainedOnBAHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.625f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.625f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsBasicAttack)),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                damage.TargetOfDamage.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
            })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableEffectOnEvent(new(this)) {DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.2f, 1)}, TriggersOncePerAbility=true, EffectTypeName="ApplyChainedOnDodge", FlatAmount = GetFirstModifierEffectValue() * 0.2f, ConditionCheckForDamageWasDodged = new Func<Damage, bool>((damage) => 
                damage.TargetOfDamage == Player.Instance), ActionOnDamageWasDodged = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                damage.SourceOfDamage.User.AddEffect(new Effect_Chained(effect.FlatAmount, new(this)));
            })}};
    }
}

