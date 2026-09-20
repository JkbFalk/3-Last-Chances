using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : Stat 
{
    public const float MAX_ENERGY_CAP = 10f;
    private Slider[] _energyOrbs;

    public Energy(Unit stat_owner, float base_amount) : base(stat_owner, MAX_ENERGY_CAP) 
    {
        if (Owner is Player) {
            Transform energyContainer = UIManager.Objects.ResourceBars.transform.Find("Energy");
            if (energyContainer != null)
            {
                _energyOrbs = new Slider[10];
                for (int i = 0; i < 10; i++)
                {
                    _energyOrbs[i] = energyContainer.Find("Orbs/" + (i + 1).ToString())?.GetComponent<Slider>();
                }
            }
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("Energy/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = MAX_ENERGY_CAP;
        Maximum = MAX_ENERGY_CAP;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0;
    }

    public void GenerateEnergy(Constants.EnergyGainSource source, bool is_boss = true, float health_lost_amount = 0) 
    {
        if (!(Owner is Player) || (Owner is Player && Player.Instance.IsStaggered)) 
            return;

        float current_amount_before = Current;
        float baseGain = 0;
        float bossMult = is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1f;
        float egMultiplier = (1f + Player.Instance.EnergyGain.Current / 100f);

        switch (source) 
        {
            case Constants.EnergyGainSource.BasicAttack:
                baseGain = Constants.ENERGY_FROM_BASIC_ATTACK * bossMult;
                break;
            case Constants.EnergyGainSource.Riposte:
                baseGain = Constants.ENERGY_FROM_RIPOSTING * bossMult;
                break;
            case Constants.EnergyGainSource.Counter:
                baseGain = Constants.ENERGY_FROM_COUNTERING * bossMult;
                break;
            case Constants.EnergyGainSource.Dodge:
                baseGain = Constants.ENERGY_FROM_DODGING * bossMult;
                break;
            case Constants.EnergyGainSource.InflictedStaggered:
                baseGain = Constants.ENERGY_FROM_INFLICTING_STAGGERED * bossMult;
                break;
            case Constants.EnergyGainSource.Block:
                baseGain = Constants.ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING * bossMult;
                break;
            case Constants.EnergyGainSource.HealthLost:
                float hpPercent = (health_lost_amount / Owner.Health.Maximum) * 100f;
                baseGain = (hpPercent / 10f) * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * bossMult;
                break;
        }

        Current += baseGain * egMultiplier;
        EventManager.GeneratedEnergy.Invoke(baseGain, Current - current_amount_before, source);
    }

    public void GenerateEnergy(float amount, bool affected_by_energy_gain = true) 
    {
        if (Owner is Player) 
        {
            float mult = affected_by_energy_gain ? (1f + Player.Instance.EnergyGain.Current / 100f) : 1f;
            Current += amount * mult;
        }
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        base.AdditionalStatSpecificActionsAfterCurrentValueChanged();
        MarkAbilitiesWithNotEnoughEnergy();
        UpdateEnergyOrbsDisplay();
    }

    // Inside Assets/Scripts/Stat/Energy.cs

    public static void MarkAbilitiesWithNotEnoughEnergy()
    {
        if (Player.Instance?.CurrentStance?.Abilities == null) return;

        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
        {
            if (ability?.AbilityGraphic == null) continue;
            var stroke = ability.AbilityGraphic.transform.Find("Stroke")?.GetComponent<UnityEngine.UI.Image>();
            if (stroke == null) continue;

            if (ability.Type == null)
            {
                stroke.color = Colors.AbilityEnoughEnergyBorderColor;
            }
            else if (Player.Instance.PreparingForUltimate)
            {
                // While preparing an ultimate, locked families get the red border
                Ability.AbilityFamily family = Ability.GetFamily(ability.Type);
                bool isUnlocked = SaveFile.Instance.IsUltimateFamilyUnlocked(family);

                stroke.color = isUnlocked 
                    ? Colors.AbilityEnoughEnergyBorderColor 
                    : Colors.AbilityNotEnoughEnergyBorderColor; // Red border
            }
            else
            {
                float cost = Ability.GetEnergyCost(ability.Type);
                bool hasEnough = Player.Instance.Energy.Current >= cost;
                stroke.color = hasEnough 
                    ? Colors.AbilityEnoughEnergyBorderColor 
                    : Colors.AbilityNotEnoughEnergyBorderColor;
            }
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat((int)Current) + " / " + Utils.GetFormattedFloat((int)Maximum);
    }

    public void UpdateEnergyOrbsDisplay()
    {
        if (_energyOrbs == null || _energyOrbs.Length == 0)
        {
            return;
        }
        float energyPerOrb = Maximum / 10f;
        if (energyPerOrb <= 0f) energyPerOrb = 1f;
        for (int i = 0; i < 10; i++)
        {
            if (i >= _energyOrbs.Length || _energyOrbs[i] == null)
            {
                continue;
            }
            float orbFloor = i * energyPerOrb;
            float fillRatio = Mathf.Clamp01((Current - orbFloor) / energyPerOrb);
            _energyOrbs[i].value = Mathf.Lerp(_energyOrbs[i].minValue, _energyOrbs[i].maxValue, fillRatio);
        }
    }
}