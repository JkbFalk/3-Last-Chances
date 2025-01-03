using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Void : Item
{
    public Gloves_Void(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Void;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Void)},EffectTypeName="DecreaseStaggerBarOnAbilityHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 1.5f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 1.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsTechnique)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Void(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(this)) { PercentageAmount = 1} };
    }
}
