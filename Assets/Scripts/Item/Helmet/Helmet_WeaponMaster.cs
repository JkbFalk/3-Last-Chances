using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_WeaponMaster : Item
{
    public Helmet_WeaponMaster(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.WeaponMaster;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableDamageChange(new(this)) {CustomParam =  GetFirstModifierEffectValue() * 0.9375f,EffectTypeName="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa", DescriptionParameters = new List<String> {"5", Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.9375f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                Player.Instance.AddEffect(
                    new Effect_CustomizableDamageChange(new(this)) {CustomParam = effect.CustomParam, ShowsInUI=true, PathToEffectGraphic="UI/AllWeapons", Identifier="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredBasicAttack", EffectIndicatorText=effect.CustomParam + "%", 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage2, effect2) => damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)), BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier,
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                        damage3.ExtraInjuryDealtPercentage += effect3.CustomParam;
                        damage3.ExtraStaggerDealtPercentage += effect3.CustomParam;
                        effect3.EndThisEffect();
                    })}, 5);
                })},
            new Effect_CustomizableDamageChange(new(this)) {CustomParam =  GetFirstModifierEffectValue() * 0.9375f, EffectTypeName="NoDescription", ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                Player.Instance.AddEffect(
                    new Effect_CustomizableDamageChange(new(this)) {CustomParam = effect.CustomParam, ShowsInUI=true, PathToEffectGraphic="UI/Technique", Identifier="WeaponTechniquesEmpowerNextBasicAttackAndViceVersa - EmpoweredWeaponTechnique", EffectIndicatorText=effect.CustomParam + "%", 
                    ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage2, effect2) => damage2.SourceOfDamage.User == Player.Instance && damage2.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage2.IsWeaponDamage), BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, 
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage3, effect3) =>  {
                        damage3.ExtraInjuryDealtPercentage += effect3.CustomParam;
                        damage3.ExtraStaggerDealtPercentage += effect3.CustomParam;
                        effect3.EndThisEffect();
                    })}, 5);
                })}
        };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {             
            new Effect_ChangeStat(Player.Instance.HeavyInjury, new(this)) {EffectTypeName="WeaponDamage", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetSecondModifierEffectValue())}, PercentageAmount = 1},
            new Effect_ChangeStat(Player.Instance.HeavyStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 1},
            new Effect_ChangeStat(Player.Instance.LightInjury, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 1},
            new Effect_ChangeStat(Player.Instance.LightStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 1},
            new Effect_ChangeStat(Player.Instance.RangedInjury, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 1},
            new Effect_ChangeStat(Player.Instance.RangedStagger, new(this)) {EffectTypeName="NoDescription", PercentageAmount = 1},
        };
    }
}

