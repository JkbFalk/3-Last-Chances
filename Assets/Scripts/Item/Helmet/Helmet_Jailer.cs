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
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Chained)},EffectTypeName="ApplyChainedOnWeaponAbilityHit", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.5f).ToString()}, CustomParam = GetFirstModifierEffectValue() * 0.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.IsTechnique && damage.SourceOfDamage.DamageType != Constants.DamageType.Magic && damage.SourceOfDamage.DamageType != Constants.DamageType.None)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 } };
    }
}

