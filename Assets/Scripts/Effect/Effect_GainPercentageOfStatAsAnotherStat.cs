using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat : Effect
{
    public float PercentageOfStatConverted = 0;
    public Stat StatToTakeIncreasesFrom;
    public Stat StatToApplyIncreasesTo;
    private Effect_ChangeStat _extraIncreases;

    public Effect_PercentageOfStatIncreasesAlsoAffectAnotherStat(Stat stat_to_take_increases_from, Stat stat_to_apply_increases_to, float percentage_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        StatToTakeIncreasesFrom = stat_to_take_increases_from;
        StatToApplyIncreasesTo = stat_to_apply_increases_to;
        PercentageOfStatConverted = percentage_amount;
        Listeners.Add(EventManager.UnitStatCurrentAmountChanged);
    }

    public override void OnStart()
    {
        base.OnStart();
        _extraIncreases = new Effect_ChangeStat(StatToApplyIncreasesTo, SourceOfEffect) {
            PercentageModifier = PercentageOfStatConverted / 100 * StatToTakeIncreasesFrom.Maximum,
        };
        TargetOfEffect.AddEffect(_extraIncreases);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _extraIncreases.EndThisEffect();
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount)
    {
        if(stat.ShouldInvoke && stat == StatToTakeIncreasesFrom)
        {
            stat.ShouldInvoke = false;
            _extraIncreases.PercentageModifier = PercentageOfStatConverted / 100 * StatToTakeIncreasesFrom.Maximum;
            stat.ShouldInvoke = true;
        }
    }
}
