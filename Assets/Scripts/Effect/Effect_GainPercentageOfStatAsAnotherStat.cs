using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_GainPercentageOfStatAsAnotherStat : Effect
{
    public float PercentageOfStatConverted = 0;
    public Stat StatToTakePercentageFrom;
    public Stat StatToConvertInto;
    public string StatToTakePercentageFromColor = "";
    public string StatToConvertIntoColor = "";
    public float CurrentBonus;
    public float MaxAmount = 0;

    public Effect_GainPercentageOfStatAsAnotherStat(Stat stat_to_take_percentage_from, Stat stat_to_convert_into, float percentage_amount, float max_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        StatToTakePercentageFrom = stat_to_take_percentage_from;
        StatToConvertInto = stat_to_convert_into;
        MaxAmount = max_amount;
        PercentageOfStatConverted = percentage_amount;
    }

    public override void OnEffectValueChanged()
    {
        MaxAmount *= LinearEffectValue;
        if(StatToTakePercentageFrom != null && StatToConvertInto != null)
        {
            DescriptionParameters = new List<string> { Utils.GetFormattedFloat(PercentageOfStatConverted), "{StatLabel_" + StatToTakePercentageFrom.GetType().ToString() + "}", "{StatLabel_" + StatToConvertInto.GetType().ToString() + "}", Utils.GetFormattedFloat(MaxAmount), StatToTakePercentageFromColor, StatToConvertIntoColor };
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        CurrentBonus = StatToTakePercentageFrom.Maximum * PercentageOfStatConverted / 100 > MaxAmount ? MaxAmount: StatToTakePercentageFrom.Maximum * PercentageOfStatConverted / 100;
        StatToConvertInto.AddFlatModifier(this, CurrentBonus);
        EventManager.UnitStatCurrentAmountChanged.AddListener(RecalculateBonus);
    }

    public void RecalculateBonus(Stat stat, float amount)
    {
        if(stat.ShouldInvoke && stat == StatToTakePercentageFrom)
        {
            stat.ShouldInvoke = false;
            StatToConvertInto.RemoveFlatModifier(this);
            CurrentBonus = StatToTakePercentageFrom.Maximum * PercentageOfStatConverted / 100 > MaxAmount ? MaxAmount : StatToTakePercentageFrom.Maximum * PercentageOfStatConverted / 100;
            StatToConvertInto.AddFlatModifier(this, CurrentBonus);
            stat.ShouldInvoke = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        StatToConvertInto.RemoveFlatModifier(this);
        EventManager.UnitStatCurrentAmountChanged.RemoveListener(RecalculateBonus);
    }
}
