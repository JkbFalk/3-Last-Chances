using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_Ambience : Item
{
    public TwinBlades_Ambience(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(20, 135, 1.05f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableEffectOnEvent(new(this)) { UsesTheFollowingEffects=new() {typeof(Effect_Invincible)},DescriptionParameters = new List<String> {Utils.GetFormattedFloat(0.6f + 0.015f * GetFirstModifierEffectValue(), 1)}, EffectTypeName="GainInvincibleOnRiposteOrCounter", FlatAmount = 0.6f + 0.015f * GetFirstModifierEffectValue(), ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User == Player.Instance && (ability.IsCounter || ability.IsRiposte))), ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Invincible(new(this)) {DisplayEffectIndicator = true}, ability.IsCounter ? effect.FlatAmount * 2 : effect.FlatAmount);
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { CustomParam =  GetSecondModifierEffectValue() * 1.5f,EffectTypeName="RiposteAndCounterDamage", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 1.5f, 0)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.IsRiposte || damage.SourceOfDamage.IsCounter)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.ExtraInjuryDealtPercentage += effect.CustomParam;
                    damage.ExtraStaggerDealtPercentage += effect.CustomParam;
                })} };
    }
}

