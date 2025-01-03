using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_TemporarilyIncreaseStatBelowHealthThreshold : Effect
{
    public float BonusAmount = 10;
    public Stat StatAffected;
    public float BonusDuration = 15;
    public float EffectCooldown = 30;
    public float HealthThreshold = 30;
    public float BaseIncrease = 0;

    public Effect_TemporarilyIncreaseStatBelowHealthThreshold(Stat stat_affected, float bonus_amount, float bonus_duration, float effect_cooldown, float health_threshold, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        StatAffected = stat_affected;
        BonusAmount = bonus_amount;
        BonusDuration = bonus_duration;
        EffectCooldown = effect_cooldown;
        HealthThreshold = health_threshold;
        if(stat_affected.Owner is Player) {
            EventManager.PlayerObjectReinitialized.AddListener(UpdateAffectedStat);
        }
        Listeners.Add(EventManager.DamageDealt);
    }

    public void UpdateAffectedStat() {
        StatAffected = Player.Instance.GetUnitStatCorrespondingToGivenStat(StatAffected);
    }

    public override void OnEffectValueChanged()
    {
        BonusAmount = BaseIncrease + BonusAmount * LinearEffectValue;
        if(StatAffected != null)
        {
            DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthThreshold), Utils.GetFormattedFloat(BonusAmount), "{StatLabel_" + StatAffected.GetType().ToString() + "}", Utils.GetFormattedFloat(BonusDuration), Utils.GetFormattedFloat(EffectCooldown) };
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        if(damage.TargetOfDamage.CheckIfEffectIsOnCooldown(this) == false && damage.TargetOfDamage.Health.Current < damage.TargetOfDamage.Health.Maximum * 0.3f)
        {
            damage.TargetOfDamage.AddEffect(new Effect_ChangeStat(StatAffected, SourceOfEffect) {PercentageAmount = BonusAmount}, BonusDuration);
            damage.TargetOfDamage.AddCooldown(this, EffectCooldown);
            base.OnInvokeDamageDealt(damage);
        }
    }
}
