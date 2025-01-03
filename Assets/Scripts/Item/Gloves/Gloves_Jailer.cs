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
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Chained)},EffectTypeName="ApplyChainedOnBAHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.25f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.25f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Control, new(this)) { PercentageAmount = 0.5f } };
    }
}
