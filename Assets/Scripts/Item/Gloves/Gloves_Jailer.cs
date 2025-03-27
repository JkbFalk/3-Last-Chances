using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Jailer : Item
{
    public Gloves_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="NoDescription", FlatAmount= GetFirstModifierEffectValue() * 0.2f, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
            ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter))), 
            ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, new(this)));
                })},
            new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="DodgingAndCounteringAppliesChained", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.2f, 1)}, TriggersOncePerAbility=true, FlatAmount = GetFirstModifierEffectValue() * 0.2f, ConditionCheckForDamageWasDodged = new Func<Damage, bool>((damage) => 
                damage.TargetOfDamage == Player.Instance), ActionOnDamageWasDodged = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount, new(this)));
            })}
                };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, GetFirstModifierEffectValue(false) * -0.5f, new(this)) };
    }
}
