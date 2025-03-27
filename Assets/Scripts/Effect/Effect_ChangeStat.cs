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
            return "+" + Utils.GetFormattedFloat(PercentageAmount) + "%" + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (FlatAmount != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (FlatAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "") + Utils.GetFormattedFloat(FlatAmount) + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (PercentageAmount != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (PercentageAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "") + Utils.GetFormattedFloat(PercentageAmount) + "%" + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (RegenerationFlatAmount != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (RegenerationFlatAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "") + Utils.GetFormattedFloat(RegenerationFlatAmount) + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
        }
        else if (RegenerationPercentageAmount != 0)
        {
            return (RemainsActiveInOtherStances ? Label.Get("RemainsActiveInOtherStances") + "\n": "") + (RegenerationPercentageAmount > 0 ? "+" : "") + (red ? "[R]" : purple ? "[P]" : "") + Utils.GetFormattedFloat(RegenerationPercentageAmount) + "%" + (red ? "[/R]" : purple ? "[/P]" : "") + Label.Get("StatLabel_" + StatAffected.ToString());
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
                    if(PercentageAmount != 0) {
                        StatAffected.AddPercentageModifier(this, PercentageAmount);
                    }
                    if(FlatAmount != 0) {
                        StatAffected.AddFlatModifier(this, FlatAmount);
                    }
                    if(RegenerationPercentageAmount != 0) {
                        StatAffected.AddPercentageRegeneration(this, RegenerationPercentageAmount);
                    }
                    if(RegenerationFlatAmount != 0) {
                        StatAffected.AddFlatRegeneration(this, RegenerationFlatAmount);
                    }
                }
                else {
                    if(PercentageAmount != 0) {
                        StatAffected.RemovePercentageModifier(this, PercentageAmount);
                    }
                    if(FlatAmount != 0) {
                        StatAffected.RemoveFlatModifier(this, FlatAmount);
                    }
                    if(RegenerationPercentageAmount != 0) {
                        StatAffected.RemovePercentageRegeneration(this, RegenerationPercentageAmount);
                    }
                    if(RegenerationFlatAmount != 0) {
                        StatAffected.RemoveFlatRegeneration(this, RegenerationFlatAmount);
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

    public bool ScaleWithEffectValue = true;
    public Stat StatAffected;
    private float _percentageAmount = 0;
    public float PercentageAmount {
        get => _percentageAmount;
        set {
            if(_percentageAmount != 0 && IsTurnedOn) {
                StatAffected.RemovePercentageModifier(this, _percentageAmount);
            }
            _percentageAmount = value;
            if(_percentageAmount != 0 && IsTurnedOn) {
                StatAffected.AddPercentageModifier(this, PercentageAmount);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _flatAmount = 0;
    public float FlatAmount {
        get => _flatAmount;
        set {
            if(_flatAmount != 0 && IsTurnedOn) {
                StatAffected.RemoveFlatModifier(this, _flatAmount);
            }
            _flatAmount = value;
            if(_flatAmount != 0 && IsTurnedOn) {
                StatAffected.AddFlatModifier(this, _flatAmount);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationPercentageAmount = 0;
    public float RegenerationPercentageAmount {
        get => _regenerationPercentageAmount;
        set {
            if(_regenerationPercentageAmount != 0 && IsTurnedOn) {
                StatAffected.RemovePercentageRegeneration(this, _regenerationPercentageAmount);
            }
            _regenerationPercentageAmount = value;
            if(_regenerationPercentageAmount != 0 && IsTurnedOn) {
                StatAffected.AddPercentageRegeneration(this, _regenerationPercentageAmount);
            }
            Type = value >= 0 ? EffectType.Buff : EffectType.Debuff;
        }
    }

    private float _regenerationFlatAmount = 0;
    public float RegenerationFlatAmount {
        get => _regenerationFlatAmount;
        set {
            if(_regenerationFlatAmount != 0 && IsTurnedOn) {
                StatAffected.RemoveFlatRegeneration(this, _regenerationFlatAmount);
            }
            _regenerationFlatAmount = value;
            if(_regenerationFlatAmount != 0 && IsTurnedOn) {
                StatAffected.AddFlatRegeneration(this, _regenerationFlatAmount);
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

    public override void OnEffectValueChanged()
    {
        if(ScaleWithEffectValue && (StatAffected is MovementSpeed || StatAffected is Control || StatAffected is Tenacity || StatAffected is CooldownReduction || StatAffected is EnergyGain || StatAffected is DamageReduction)){
            PercentageAmount *= NonLinearEffectValue;
            FlatAmount *= NonLinearEffectValue;
            RegenerationPercentageAmount *= NonLinearEffectValue;
            RegenerationFlatAmount *= NonLinearEffectValue;
        } 
        else if(ScaleWithEffectValue) {
            PercentageAmount *= LinearEffectValue;
            FlatAmount *= LinearEffectValue;
            RegenerationPercentageAmount *= LinearEffectValue;
            RegenerationFlatAmount *= LinearEffectValue;
        }
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