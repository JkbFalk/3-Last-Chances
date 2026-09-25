using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Stat {
    protected float PreviousCurrentAmount;
    protected bool CurrentCanBeLowerThanMaximum = false;
    protected TextMeshProUGUI AmountDisplay;
    public Slider HUDSlider;
    public TextMeshProUGUI MenuStatDisplay;
    public Unit Owner;
    public string Id;
    public List<StatModifier> BaseModifiers = new List<StatModifier>();
    public List<StatModifier> PercentageModifiers = new List<StatModifier>();

    public float Regeneration { get; protected set; } = 0;
    public List<StatModifier> FlatRegeneration = new List<StatModifier>();
    public List<StatModifier> PercentageRegeneration = new List<StatModifier>();

    public bool CannotBeLowerThan1 = false;
    protected float _current = 0;
    public float MaximumValue = 1000000;

    public bool ShouldInvoke = true;
    public Slider HUDBlockedSlider; 

    public float Current {
        get => _current;
        set {
            PreviousCurrentAmount = _current;
            _current = (value > Maximum ? Maximum : (value < 0 ? 0 : value));
            
            if(_current != Maximum && CurrentCanBeLowerThanMaximum == false)
            {
                _current = Maximum;
            }
            
            if(PreviousCurrentAmount != _current || this is AttackSpeed) {
                AdditionalStatSpecificActionsAfterCurrentValueChanged();
                if(ShouldInvoke) {
                    EventManager.UnitStatCurrentAmountChanged.Invoke(this, _current - PreviousCurrentAmount);
                }
            }
            
            // Scale main slider relative to Base if debuffed
            if (HUDSlider != null) {
                float visualMax = Maximum < Base ? Base : Maximum;
                if (visualMax <= 0) visualMax = 1f; 
                HUDSlider.value = _current / visualMax;
            }

            if (AmountDisplay != null) {
                AmountDisplay.text = ((int)_current).ToString();
            }
            if(Owner is Player && MenuStatDisplay != null) {
                UpdateMenuStatDisplayValue();
            }
            ShouldInvoke = true;
        }
    }

    public void RecalculateMaximumAmount(System.Object source) {
        float calculated_maximum = Base;
        foreach (StatModifier modifier in BaseModifiers)
        {
            calculated_maximum += modifier.Amount;
        }
        
        float total_added = 0;
        foreach (StatModifier modifier in PercentageModifiers) {
            total_added += calculated_maximum * modifier.Amount / 100;
        }
        
        Maximum = calculated_maximum + total_added;
        
        if (this is Energy == false && (CurrentCanBeLowerThanMaximum == false || Owner == null || Owner.InCombat == false)) {
            Current = this is StaggerBar ? 0 : Maximum;
        }
        else
        {
            Current = Current; 
        }
        
        float visualMax = Maximum < Base ? Base : Maximum;
        if (visualMax <= 0) visualMax = 1f;
        
        // CHANGED: Update the Slider value instead of Image fillAmount
        if (HUDBlockedSlider != null) {
            HUDBlockedSlider.value = Mathf.Clamp01(1f - (Maximum / visualMax));
        }

        RecalculateRegeneration();
        AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
    }

    
    public void ChangeCurrentValueWithoutInvoking(float amount) {
        ShouldInvoke = false;
        Current = amount;
    }

    private float _base = 0;

    public float Base {
        get => _base;
        set {
            _base = value > MaximumValue ? MaximumValue : value;
            RecalculateMaximumAmount(null);
        }
    }

    public virtual void UpdateMenuStatDisplayValue() {

    }

    private float _maximum = 0;

    public float Maximum {
        get => _maximum;
        set {
            _maximum = value > MaximumValue ? MaximumValue : value;
            if(value < 1 && CannotBeLowerThan1)
            {
                _maximum = 1;
            }
            if (Current > _maximum) {
                Current = _maximum;
            }
            AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
        }
    }
    
    public float Missing
    {
        get => _maximum - _current;
    }

    public float CurrentPercentage
    {
        get => _current / _maximum * 100;
    }

    public Stat(Unit stat_owner, float base_amount)
    {
        Owner = stat_owner;
        _base = base_amount > MaximumValue ? MaximumValue : base_amount;
        _maximum = _base;
        _current = _base;
        Id = Owner?.name + "-" + GetType() + "-" + Guid.NewGuid().ToString();
    }

    public void AddFlatModifier(System.Object source, float amount) {
        BaseModifiers.Add(new StatModifier(amount, source));
        RecalculateMaximumAmount(source);
    }

    public string GetCalculatedIncrease(float percentage_increase = 0, float flat_increase = 0) {
        float calculated_maximum = Base;
        float total_added = 0;
        float total_percentages = 0;
        float total_flats = 0;
        float modified_flat_increase = flat_increase;
        foreach (StatModifier modifier in BaseModifiers)
        {
            total_flats += modifier.Amount;
            calculated_maximum += modifier.Amount;
        }
        foreach (StatModifier modifier in PercentageModifiers) {
            total_percentages += modifier.Amount;
            total_added += calculated_maximum * modifier.Amount / 100;
            modified_flat_increase += modified_flat_increase * modifier.Amount / 100;
        }
        float pre_increase_percentage_total = calculated_maximum + total_added;
        total_added += calculated_maximum * percentage_increase / 100;
        calculated_maximum += total_added;
        if(flat_increase == 0) {
            return Utils.GetFormattedFloat(pre_increase_percentage_total) + " -> " + Utils.GetFormattedFloat(calculated_maximum) + "     (" + Utils.GetFormattedFloat(total_percentages) + "% -> " + Utils.GetFormattedFloat(total_percentages + percentage_increase) + "%)";
        }
        else {
            return Utils.GetFormattedFloat(calculated_maximum) + " -> " + Utils.GetFormattedFloat(calculated_maximum + modified_flat_increase) + "      (" + Utils.GetFormattedFloat(Base + total_flats) + " -> " + Utils.GetFormattedFloat(Base + total_flats + flat_increase) + ")";
        }
    }

    public void RemoveFlatModifier(System.Object source, float amount = 0) {
        StatModifier modifierToRemove = BaseModifiers.FirstOrDefault(statModifier => statModifier.Source == source && (amount == 0 || statModifier.Amount == amount));
        if (modifierToRemove == null) {
            Debug.LogWarning("Flat Modifier to remove (" + source + ": " + amount + ") for stat " + GetType().Name + " not found");
            return;
        }
        BaseModifiers.Remove(modifierToRemove);
        RecalculateMaximumAmount(source);
    }

    public void AddPercentageModifier(System.Object source, float amount) {
        PercentageModifiers.Add(new StatModifier(amount, source));
        RecalculateMaximumAmount(source);
    }

    public void RemovePercentageModifier(System.Object source, float amount = 0) {
        StatModifier modifierToRemove = PercentageModifiers.FirstOrDefault(statModifier => statModifier.Source == source && (amount == 0 || statModifier.Amount == amount));
        if (modifierToRemove == null)
        {
            Debug.LogWarning("Percentage Modifier to remove (" + source.GetType() + ": " + amount + ") for stat " + GetType().Name + " not found");
            return;
        }
        PercentageModifiers.Remove(modifierToRemove);
        RecalculateMaximumAmount(source);
    }

    public bool CheckIfContainsFlatRegeneration(System.Object source, float amount) {
        return FlatRegeneration.FirstOrDefault(item => item.Amount == amount && item.Source == source) != null;
    }
    public bool CheckIfContainsPercentageRegeneration(System.Object source, float amount)
    {
        return PercentageRegeneration.FirstOrDefault(item => item.Amount == amount && item.Source == source) != null;
    }

    public void AddFlatRegeneration(System.Object source, float amount) {
        FlatRegeneration.Add(new StatModifier(amount, source));
        RecalculateRegeneration();
    }

    public void RemoveFlatRegeneration(System.Object source, float amount = 0) {
        StatModifier modifierToRemove = FlatRegeneration.FirstOrDefault(statModifier => statModifier.Source == source);
        if (modifierToRemove == null) {
            Debug.LogWarning("Flat Regeneration to remove (" + source + ": " + amount + ") for stat " + GetType().Name + " not found");
            return;
        }
        FlatRegeneration.Remove(modifierToRemove);
        RecalculateRegeneration();
    }

    public void AddPercentageRegeneration(System.Object source, float amount) {
        PercentageRegeneration.Add(new StatModifier(amount, source));
        RecalculateRegeneration();
    }

    public void RemovePercentageRegeneration(System.Object source, float amount = 0) {
        StatModifier modifierToRemove = PercentageRegeneration.FirstOrDefault(statModifier => statModifier.Source == source);
        if (modifierToRemove == null) {
            Debug.LogWarning("Percentage Regeneration to remove (" + source + ": " + amount + ") for stat " + GetType().Name + " not found");
            return;
        }
        PercentageRegeneration.Remove(modifierToRemove);
        RecalculateRegeneration();
    }

    public float GetRegenerationTotalWithoutItems()
    {
        float total = 0;
        foreach (StatModifier modifier in FlatRegeneration)
        {
            if (modifier.Source is Constants.StatSource && (((Constants.StatSource)modifier.Source) == Constants.StatSource.Base || ((Constants.StatSource)modifier.Source) == Constants.StatSource.PowerUp))
            {
                total += modifier.Amount;
            }
        }
        foreach (StatModifier modifier in PercentageRegeneration)
        {
            if (modifier.Source is Constants.StatSource && (((Constants.StatSource)modifier.Source) == Constants.StatSource.Base || ((Constants.StatSource)modifier.Source) == Constants.StatSource.PowerUp)) {
                total += total * modifier.Amount / 100;
            }
        }
        return total;
    }

    private void RecalculateRegeneration() {
        float sum = 0;
        foreach (StatModifier modifier in FlatRegeneration) {
            sum += modifier.Amount;
        }
        foreach (StatModifier modifier in PercentageRegeneration) {
            sum += Maximum * (modifier.Amount / 100);
        }
        Regeneration = sum;
    }

    public virtual void AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount() {
    }

    public virtual void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
    }

    public class StatModifier {
        public float Amount;
        public System.Object Source;

        public StatModifier(float amount, System.Object source) {
            Amount = amount;
            Source = source;
        }
    }

    public void ClearAllModifiers()
    {
        BaseModifiers.Clear();
        PercentageModifiers.Clear();
        FlatRegeneration.Clear();
        PercentageRegeneration.Clear();
    }
}