using System;
using System.Collections;
using System.Collections.Generic;
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
            new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Sharp)},EffectTypeName="GainSharpOnBAAndCounter", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.125f).ToString(), (GetFirstModifierEffectValue() * 0.2f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.125f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Sharp(effect.CustomParam, new(this)));
                })},
            new Effect_CustomizableEffectOnEvent(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Onslaught)},EffectTypeName="NoDescription", FlatAmount= GetFirstModifierEffectValue() * 0.2f, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
            ability.User is Player && (ability.IsRiposte || ability.IsCounter)), 
            ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Onslaught(effect.FlatAmount, new(this)));
                })},
                };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.5f } };
    }
}
