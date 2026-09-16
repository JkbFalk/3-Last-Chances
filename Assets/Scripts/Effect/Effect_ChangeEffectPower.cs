using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_ChangeEffectPower : Effect
{
    public static string GetDescription(ChangeTypeEnum change_type, AffectedUnitsTypeEnum affected_units_type, float percentage_change, Type effect_type) {
        if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Enemies && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnEnemy_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseAmountOnPlayer_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnPlayer_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Enemies && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnEnemy_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseDecayOnPlayer_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnPlayer_Description"), new object[] {Label.Get(effect_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else return Label.Get("MissingLabel");
    }   

    public override string ToString()
    {
        return GetDescription(ChangeType, AffectedUnitsType, PercentageChange, AffectedEffectType);
    }

    public Type AffectedEffectType;
    public enum AffectedUnitsTypeEnum {Enemies, Player, Both};
    public enum ChangeTypeEnum {AffectAmountAdded, AffectDecaySpeed}
    public ChangeTypeEnum ChangeType;
    public AffectedUnitsTypeEnum AffectedUnitsType = AffectedUnitsTypeEnum.Enemies;
    public float PercentageChange = 0;
    public Func<Unit, bool> ConditionForEffectPowerChange = new Func<Unit, bool>(unit => true);
    public Effect_ChangeEffectPower(Type effect_type, ChangeTypeEnum change_type, AffectedUnitsTypeEnum affected_units, float percentage_change, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        AffectedEffectType = effect_type;
        AffectedUnitsType = affected_units;
        ChangeType = change_type;
        PercentageChange = percentage_change;
        Type = EffectType.Buff;
        HasLinearScaling = false;
    }
}
