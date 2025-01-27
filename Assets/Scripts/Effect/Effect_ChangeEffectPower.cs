using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_ChangeEffectPower : Effect
{
    public static string GetDescription(ChangeTypeEnum change_type, AffectedUnitsTypeEnum affected_units_type, float percentage_change) {
        if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Enemies && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnEnemy_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseAmountOnPlayer_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectAmountAdded && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnPlayer_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Enemies && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnEnemy_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseDecayOnPlayer_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else if (change_type == ChangeTypeEnum.AffectDecaySpeed && affected_units_type == AffectedUnitsTypeEnum.Player && percentage_change < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnPlayer_DescriptionSimple"), new object[] {Label.Get(affected_units_type.ToString()), Utils.GetFormattedFloat(Math.Abs(percentage_change))});
        }
        else return Label.Get("MissingLabel");
    }   

    public override string ToString()
    {
        return GetDescription(ChangeType, AffectedUnitsType, PercentageChange);
    }

    public Type AffectedEffectType;
    public enum AffectedUnitsTypeEnum {Enemies, Player};
    public enum ChangeTypeEnum {AffectAmountAdded, AffectDecaySpeed}
    public ChangeTypeEnum ChangeType;
    public AffectedUnitsTypeEnum AffectedUnitsType = AffectedUnitsTypeEnum.Enemies;
    public float PercentageChange = 0;
    public Func<bool> ConditionForEffectPowerChange = new Func<bool>(() => true);
    public Effect_ChangeEffectPower(Type effect_type, ChangeTypeEnum change_type, AffectedUnitsTypeEnum affected_units, float percentage_change, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        AffectedEffectType = effect_type;
        AffectedUnitsType = affected_units;
        ChangeType = change_type;
        PercentageChange = percentage_change;
        UsesTheFollowingEffects=new() {AffectedEffectType};
        Type = EffectType.Buff;
        HasLinearScaling = false;
    }
}
