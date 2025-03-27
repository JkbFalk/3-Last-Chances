using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Jailer : Item
{
    public Helmet_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="DealingOrTakingDamageAppliesChainedToYou", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.5f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && (damage.SourceOfDamage.User == Player.Instance || damage.TargetOfDamage == Player.Instance)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 } };
    }
}

