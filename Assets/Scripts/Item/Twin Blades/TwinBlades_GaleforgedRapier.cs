using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_GaleforgedRapier : Item
{
    public TwinBlades_GaleforgedRapier(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(20, 135, 1.05f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableEffectOnEvent(new(this)) {DescriptionParameters = new List<String> {Utils.GetFormattedFloat(0.6f + 0.015f * GetFirstModifierEffectValue(), 1)}, EffectTypeName="GainInvincibleOnRiposteOrCounter", FlatAmount = 0.6f + 0.015f * GetFirstModifierEffectValue(), ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Counter) || ability.Is(Ability.AbilityProperty.Riposte)))), ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Invincible(new(this)) {ShowsInUI = true}, ability.Is(Ability.AbilityProperty.Counter) ? effect.FlatAmount * 2 : effect.FlatAmount);
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { CustomParam =  GetSecondModifierEffectValue() * 1.5f,EffectTypeName="RiposteAndCounterDamage", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 1.5f, 0)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.ExtraInjuryDealtPercentage += effect.CustomParam;
                    damage.ExtraStaggerDealtPercentage += effect.CustomParam;
                })} };
    }
}

