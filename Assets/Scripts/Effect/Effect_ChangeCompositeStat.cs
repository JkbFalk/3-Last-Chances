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
        if (FlatModifier != 0)
        {
            return (FlatModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "") + Utils.GetFormattedFloat(FlatModifier) + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (PercentageModifier != 0)
        {
            return (PercentageModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "")  + Utils.GetFormattedFloat(PercentageModifier) + "%" + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (RegenerationFlatModifier != 0)
        {
            return (RegenerationFlatModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "")  + Utils.GetFormattedFloat(RegenerationFlatModifier) + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") +Label.Get("StatLabel_" + Stat.ToString());
        }
        else if (RegenerationPercentageModifier != 0)
        {
            return (RegenerationPercentageModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "")  + Utils.GetFormattedFloat(RegenerationPercentageModifier) + "%" + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + Stat.ToString());
        }
        else return Label.Get("MissingLabel");
    }

    public CompositeStat Stat;
    private List<Effect_ChangeStat> _statChanges = new();

    private float _percentageModifier = 0;
    public float PercentageModifier {
        get => _percentageModifier;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.PercentageModifier = value;
            }
            _percentageModifier = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _flatModifier = 0;
    public float FlatModifier {
        get => _flatModifier;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.BaseModifier = value;
            }
            _flatModifier = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationPercentageModifier = 0;
    public float RegenerationPercentageModifier {
        get => _regenerationPercentageModifier;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.RegenerationPercentageModifier = value;
            }
            _regenerationPercentageModifier = value;
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationFlatModifier = 0;
    public float RegenerationFlatModifier {
        get => _regenerationFlatModifier;
        set {
            foreach(Effect_ChangeStat e in _statChanges) {
                e.RegenerationFlatModifier = value;
            }
            _regenerationFlatModifier = value;
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
}