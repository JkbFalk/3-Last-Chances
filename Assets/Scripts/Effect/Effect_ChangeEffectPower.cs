using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Constants;

public class Effect_ChangeEffectPower : Effect
{
    
    public override string ToString()
    {
        if (ChangeType == ChangeTypeEnum.AffectAmountAdded && AffectedUnitsType == AffectedUnitsTypeEnum.Enemies && PercentageChange > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnEnemy_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else if (ChangeType == ChangeTypeEnum.AffectAmountAdded && AffectedUnitsType == AffectedUnitsTypeEnum.Player && PercentageChange < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseAmountOnPlayer_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else if (ChangeType == ChangeTypeEnum.AffectAmountAdded && AffectedUnitsType == AffectedUnitsTypeEnum.Player && PercentageChange > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseAmountOnPlayer_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else if (ChangeType == ChangeTypeEnum.AffectDecaySpeed && AffectedUnitsType == AffectedUnitsTypeEnum.Enemies && PercentageChange < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnEnemy_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else if (ChangeType == ChangeTypeEnum.AffectDecaySpeed && AffectedUnitsType == AffectedUnitsTypeEnum.Player && PercentageChange > 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerIncreaseDecayOnPlayer_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else if (ChangeType == ChangeTypeEnum.AffectDecaySpeed && AffectedUnitsType == AffectedUnitsTypeEnum.Player && PercentageChange < 0)
        {
            return String.Format(Label.Get("Effect_ChangeEffectPowerDecreaseDecayOnPlayer_DescriptionSimple"), new object[] {Label.Get(AffectedEffectType.ToString()), Utils.GetFormattedFloat(Math.Abs(PercentageChange))});
        }
        else return Label.Get("MissingLabel");
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
