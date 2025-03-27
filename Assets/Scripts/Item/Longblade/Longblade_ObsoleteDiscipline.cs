using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_ObsoleteDiscipline : Item
{
    public Longblade_ObsoleteDiscipline(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        
        SetBaseWeaponStats(100, 150, 0.7f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ApplySelfChainedToEnemies", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 1.5f).ToString()}, CustomParam = GetFirstModifierEffectValue(false) * 0.25f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
            damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
            damage.TargetOfDamage.AddEffect(new Effect_Chained(playersChained.DecayingAmount * effect.CustomParam / 100, new(this)));
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="GainChainedOnHeavyDamage", DescriptionParameters=new List<String>{(GetSecondModifierEffectValue() * 0.5f).ToString()}, CustomParam = GetSecondModifierEffectValue() * 0.5f, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false && damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Heavy),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Chained(effect.CustomParam, new(this)));
        })}};
    }
}
