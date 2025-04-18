using System;
using System.Linq;
using UnityEngine;

public class Effect_ChangeStat : Effect {

    public bool ShouldInvokeOnStatChange = true;
    public override string ToString()
    {
        bool red = StatAffected is Health || StatAffected is Injury;
        bool purple = StatAffected is StaggerBar || StatAffected is Stagger;
        if (StatAffected is AttackSpeed speed) {
            return "+" + Utils.GetFormattedFloat(PercentageModifier) + "%" + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (BaseModifier != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (BaseModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "") + Utils.GetFormattedFloat(BaseModifier) + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (PercentageModifier != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (PercentageModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "") + Utils.GetFormattedFloat(PercentageModifier) + "%" + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (RegenerationFlatModifier != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (RegenerationFlatModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "") + Utils.GetFormattedFloat(RegenerationFlatModifier) + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (RegenerationPercentageModifier != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (RegenerationPercentageModifier > 0 ? "+" : "") + (red ? "[RED]" : purple ? "[PURPLE]" : "") + Utils.GetFormattedFloat(RegenerationPercentageModifier) + "%" + (red ? "[/RED]" : purple ? "[/PURPLE]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else return Label.Get("MissingLabel");
    }

    private bool _isTurnedOn = false;
    public bool IsTurnedOn {
        get => _isTurnedOn;
        set {
            if(_isTurnedOn != value) {
                _isTurnedOn = value;
                if(_isTurnedOn) {
                    if(PercentageModifier != 0) {
                        StatAffected.AddPercentageModifier(this, PercentageModifier);
                    }
                    if(BaseModifier != 0) {
                        StatAffected.AddFlatModifier(this, BaseModifier);
                    }
                    if(RegenerationPercentageModifier != 0) {
                        StatAffected.AddPercentageRegeneration(this, RegenerationPercentageModifier);
                    }
                    if(RegenerationFlatModifier != 0) {
                        StatAffected.AddFlatRegeneration(this, RegenerationFlatModifier);
                    }
                }
                else {
                    if(PercentageModifier != 0) {
                        StatAffected.RemovePercentageModifier(this, PercentageModifier);
                    }
                    if(BaseModifier != 0) {
                        StatAffected.RemoveFlatModifier(this, BaseModifier);
                    }
                    if(RegenerationPercentageModifier != 0) {
                        StatAffected.RemovePercentageRegeneration(this, RegenerationPercentageModifier);
                    }
                    if(RegenerationFlatModifier != 0) {
                        StatAffected.RemoveFlatRegeneration(this, RegenerationFlatModifier);
                    }
                }
            }
            else {
                _isTurnedOn = value;
            }
        }

    }

    public enum DependenceOnCombatStatusEnum { WorksRegardlessOfCombatStatus, OnlyWorksInCombat, OnlyWorksOutOfCombat};
    private DependenceOnCombatStatusEnum _dependenceOnCombatStatus = DependenceOnCombatStatusEnum.WorksRegardlessOfCombatStatus;
    public DependenceOnCombatStatusEnum DependenceOnCombatStatus
    {
        get
        {
            return _dependenceOnCombatStatus;
        }
        set
        {
            _dependenceOnCombatStatus = value;
            if(_dependenceOnCombatStatus != DependenceOnCombatStatusEnum.WorksRegardlessOfCombatStatus)
            {
                EventManager.EnterCombat.AddListener(ActivateOnEnterCombat);
                EventManager.ExitCombat.AddListener(ActivateOnExitCombat);
            }
            if((DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksInCombat && StatAffected.Owner.InCombat == false) || (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksOutOfCombat && StatAffected.Owner.InCombat))
            {
                IsTurnedOn = false;
            }
        }
    }

    public void ActivateOnEnterCombat(Unit unit)
    {
        if(unit != TargetOfEffect) {
            return;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.WorksRegardlessOfCombatStatus)
        {
            return;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksInCombat)
        {
            IsTurnedOn = true;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksOutOfCombat)
        {
            IsTurnedOn = false;
        }

    }

    public void ActivateOnExitCombat(Unit unit) {
        if(unit != TargetOfEffect) {
            return;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.WorksRegardlessOfCombatStatus)
        {
            return;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksInCombat)
        {
            IsTurnedOn = false;
        }
        if (DependenceOnCombatStatus == DependenceOnCombatStatusEnum.OnlyWorksOutOfCombat)
        {
            IsTurnedOn = true;
        }
    }

    public Stat StatAffected;
    private float _percentageModifier = 0;
    public float PercentageModifier {
        get => _percentageModifier;
        set {
            if(_percentageModifier != 0 && IsTurnedOn) {
                StatAffected.RemovePercentageModifier(this, _percentageModifier);
            }
            _percentageModifier = value;
            if(_percentageModifier != 0 && IsTurnedOn) {
                StatAffected.AddPercentageModifier(this, PercentageModifier);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _baseModifier = 0;
    public float BaseModifier {
        get => _baseModifier;
        set {
            if(_baseModifier != 0 && IsTurnedOn) {
                StatAffected.RemoveFlatModifier(this, _baseModifier);
            }
            _baseModifier = value;
            if(_baseModifier != 0 && IsTurnedOn) {
                StatAffected.AddFlatModifier(this, _baseModifier);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationPercentageModifier = 0;
    public float RegenerationPercentageModifier {
        get => _regenerationPercentageModifier;
        set {
            if(_regenerationPercentageModifier != 0 && IsTurnedOn) {
                StatAffected.RemovePercentageRegeneration(this, _regenerationPercentageModifier);
            }
            _regenerationPercentageModifier = value;
            if(_regenerationPercentageModifier != 0 && IsTurnedOn) {
                StatAffected.AddPercentageRegeneration(this, _regenerationPercentageModifier);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationFlatModifier = 0;
    public float RegenerationFlatModifier {
        get => _regenerationFlatModifier;
        set {
            if(_regenerationFlatModifier != 0 && IsTurnedOn) {
                StatAffected.RemoveFlatRegeneration(this, _regenerationFlatModifier);
            }
            _regenerationFlatModifier = value;
            if(_regenerationFlatModifier != 0 && IsTurnedOn) {
                StatAffected.AddFlatRegeneration(this, _regenerationFlatModifier);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    public Effect_ChangeStat(Stat stat_affected, SourceOfEffect source_of_effect) : base(source_of_effect) {
        StatAffected = stat_affected;
        PathToEffectGraphic = "UI/" + stat_affected.ToString();
        if(StatAffected?.Owner is Player) {
            EventManager.PlayerObjectReinitialized.AddListener(UpdateAffectedStat);
        }
    }

    public void UpdateAffectedStat() {
        StatAffected = Player.Instance.GetUnitStatCorrespondingToGivenStat(StatAffected);
    }

    public override void OnStart()
    {
        base.OnStart();
        IsTurnedOn = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        IsTurnedOn = false;
        EventManager.EnterCombat.RemoveListener(ActivateOnEnterCombat);
        EventManager.ExitCombat.RemoveListener(ActivateOnExitCombat);
    }
}