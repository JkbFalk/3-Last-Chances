using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Effect_ChangeCompositeStat;

public class Effect_ChangeCompositeStat : Effect {

    public override string ToString()
    {
        bool red = Stat is CompositeStat.Injury;
        bool purple = Stat is CompositeStat.Stagger;
        if (FlatAmount != 0)
        {
            return (FlatAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "") + Utils.GetFormattedFloat(FlatAmount) + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (PercentageAmount != 0)
        {
            return (PercentageAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "")  + Utils.GetFormattedFloat(PercentageAmount) + "%" + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (RegenerationFlatAmount != 0)
        {
            return (RegenerationFlatAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "")  + Utils.GetFormattedFloat(RegenerationFlatAmount) + (red ? "[/R]" : purple ? "[/P]" : "") +Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (RegenerationPercentageAmount != 0)
        {
            return (RegenerationPercentageAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "")  + Utils.GetFormattedFloat(RegenerationPercentageAmount) + "%" + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else return Label.Get("MissingLabel");
    }

    public CompositeStat Stat;
    private List<Effect_ChangeStat> _statChanges = new();

    private float _percentageAmount = 0;
    public float PercentageAmount {
        get => _percentageAmount;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.PercentageAmount = value;
            }
            _percentageAmount = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _flatAmount = 0;
    public float FlatAmount {
        get => _flatAmount;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.FlatAmount = value;
            }
            _flatAmount = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationPercentageAmount = 0;
    public float RegenerationPercentageAmount {
        get => _regenerationPercentageAmount;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.RegenerationPercentageAmount = value;
            }
            _regenerationPercentageAmount = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationFlatAmount = 0;
    public float RegenerationFlatAmount {
        get => _regenerationFlatAmount;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.RegenerationFlatAmount = value;
            }
            _regenerationFlatAmount = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    public enum CompositeStat { AttackSpeed, Injury, Stagger, Damage };

    public Effect_ChangeCompositeStat(Unit unit, CompositeStat composite_stat, SourceOfEffect source_of_effect) : base(source_of_effect) {
        TargetOfEffect = unit;
        Stat = composite_stat;
        PathToEffectGraphic = "UI/" + composite_stat.ToString();
        if (composite_stat == CompositeStat.AttackSpeed)
        {
            _statChanges = new List<Effect_ChangeStat>() {new Effect_ChangeStat(unit.HeavyAttackSpeed, SourceOfEffect), new Effect_ChangeStat(unit.LightAttackSpeed, SourceOfEffect), new Effect_ChangeStat(unit.RangedAttackSpeed, SourceOfEffect), new Effect_ChangeStat(unit.MagicAttackSpeed, SourceOfEffect)};
        }
        else if (composite_stat == CompositeStat.Injury || composite_stat == CompositeStat.Damage)
        {
            _statChanges = _statChanges.Concat(new List<Effect_ChangeStat> {new Effect_ChangeStat(unit.HeavyInjury, SourceOfEffect), new Effect_ChangeStat(unit.LightInjury, SourceOfEffect), new Effect_ChangeStat(unit.RangedInjury, SourceOfEffect), new Effect_ChangeStat(unit.MagicInjury, SourceOfEffect)}).ToList();
        }
        if (composite_stat == CompositeStat.Stagger || composite_stat == CompositeStat.Damage)
        {
            _statChanges = _statChanges.Concat(new List<Effect_ChangeStat> {new Effect_ChangeStat(unit.HeavyStagger, SourceOfEffect), new Effect_ChangeStat(unit.LightStagger, SourceOfEffect), new Effect_ChangeStat(unit.RangedStagger, SourceOfEffect), new Effect_ChangeStat(unit.MagicStagger, SourceOfEffect)}).ToList();
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        foreach(Effect_ChangeStat e in _statChanges) {
            e.IsRemovable = IsRemovable;
            e.ShowsInMenu = false;
            e.CountsAsSeparateEffect = false;
            TargetOfEffect.AddEffect(e);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        foreach(Effect_ChangeStat e in _statChanges) {
            e.EndThisEffect();
        }
    }

    public override void OnEffectValueChanged()
    {
        if(Stat is CompositeStat.AttackSpeed) {
            PercentageAmount *= NonLinearEffectValue;
            FlatAmount *= NonLinearEffectValue;
            RegenerationPercentageAmount *= NonLinearEffectValue;
            RegenerationFlatAmount *= NonLinearEffectValue;
        }
        else {
            PercentageAmount *= LinearEffectValue;
            FlatAmount *= LinearEffectValue;
            RegenerationPercentageAmount *= LinearEffectValue;
            RegenerationFlatAmount *= LinearEffectValue;
        }
    }
}