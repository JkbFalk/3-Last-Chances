using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Effect_Chained : Effect
{
    public Effect_ChangeCompositeStat DamageBuff;
    public Effect_ChangeStat DamageReductionDebuff;
    public Effect_ChangeStat EnergyGainDebuff;
    public Effect_ChangeCompositeStat AttackSpeedDebuff;
    public Effect_Chained(float chained, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        _initialDecayingAmount = chained;
        ShowsInUI = true;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDecayingAmount;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void ExtraBehaviourOnDecayingAmountChange()
    {
        EffectIndicatorText = Utils.GetFormattedFloat(DecayingAmount);
        foreach(Effect e in new List<Effect>{DamageBuff, EnergyGainDebuff, DamageReductionDebuff, AttackSpeedDebuff}) {
            if(e != null && e.EffectEnded == false) {
                PropertyInfo propertyInfo = e.GetType().GetProperty("PercentageAmount");
                propertyInfo.SetValue(e, e == DamageBuff ? DecayingAmount : -DecayingAmount / 10);
            }
        }
    }

    public override void OnStart() {
        base.OnStart();
        BaseDuration = Constants.DEFAULT_STACKING_EFFECT_BASE_DURATION_IN_SECONDS;
        DamageBuff = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.Damage, SourceOfEffect) {PercentageAmount = DecayingAmount};
        DamageReductionDebuff = new Effect_ChangeStat(TargetOfEffect.DamageReduction, SourceOfEffect) {PercentageAmount = -DecayingAmount / 10};
        AttackSpeedDebuff = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {PercentageAmount = -DecayingAmount / 10};
        if(TargetOfEffect is Player) {
            EnergyGainDebuff = new Effect_ChangeStat(Player.Instance.EnergyGain, SourceOfEffect) {PercentageAmount = -DecayingAmount / 10};
            TargetOfEffect.AddEffect(DamageBuff);
            TargetOfEffect.AddEffect(EnergyGainDebuff);
        }
        else {
            EnergyGainDebuff = new Effect_ChangeStat(TargetOfEffect.CooldownReduction, SourceOfEffect) {PercentageAmount = -DecayingAmount / 10};
            TargetOfEffect.AddEffect(EnergyGainDebuff);
        }
        TargetOfEffect.AddEffect(DamageReductionDebuff);
        TargetOfEffect.AddEffect(AttackSpeedDebuff);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        foreach(Effect e in new List<Effect>{DamageBuff, EnergyGainDebuff, DamageReductionDebuff, AttackSpeedDebuff}) {
            if(e != null && e.EffectEnded == false) {
                e.EndThisEffect();
            }
        }
    }
}
