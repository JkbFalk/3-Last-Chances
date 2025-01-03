using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Void : Item
{
    public Helmet_Void(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Void;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Void)},EffectTypeName="DecreaseStaggerBarOnBAHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.625f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.625f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Void(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 10 } };
    }
}

