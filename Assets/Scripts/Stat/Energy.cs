using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : Stat {

    public Energy(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if (Owner is Player) {
            HUDSlider = CanvasElements.UICanvas.ResourceBars.transform.Find("Energy").GetComponent<Slider>();
            AmountDisplay = CanvasElements.UICanvas.ResourceBars.transform.Find("Energy/Amount").GetComponent<TextMeshProUGUI>();
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("Energy/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0;
    }

    public void GenerateEnergy(Constants.EnergyGainSource source, bool is_boss = true, float health_lost_amount = 0) {
        if (!(Owner is Player)) {
            return;
        }
        float current_amount_before = Current;
        float BaseGain = 0;
        switch (source) {
            case Constants.EnergyGainSource.BasicAttack: {
                    Current += Constants.ENERGY_FROM_BASIC_ATTACK * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_BASIC_ATTACK * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.Riposte: {
                    Current += Constants.ENERGY_FROM_RIPOSTING * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_RIPOSTING * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.Counter:
                {
                    Current += Constants.ENERGY_FROM_COUNTERING * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_COUNTERING * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.Dodge: {
                    Current += Constants.ENERGY_FROM_DODGING * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_DODGING * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.InflictedStaggered: {
                    Current += Constants.ENERGY_FROM_INFLICTING_STAGGERED * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_INFLICTING_STAGGERED * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.Block: {
                    Current += Constants.ENERGY_FROM_BLOCKING * Player.Instance.EnergyGain.Current * (is_boss ? 2 : 1);
                    BaseGain = Constants.ENERGY_FROM_BLOCKING * (is_boss ? 2 : 1);
                    break;
                }
            case Constants.EnergyGainSource.HealthLost: {
                    float percentage = health_lost_amount / Owner.Health.Maximum * 100;
                    Current += percentage * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * Player.Instance.EnergyGain.Current  * (is_boss ? 2 : 1);
                    BaseGain = percentage * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * (is_boss ? 2 : 1);
                    break;
                }
        }
        EventManager.GeneratedEnergy.Invoke(BaseGain, Current - current_amount_before, source);
    }

    public void GenerateEnergy(float amount) {
        if (Owner is Player) {
            Current += amount;
        }
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        base.AdditionalStatSpecificActionsAfterCurrentValueChanged();
        MarkAbilitiesWithNotEnoughEnergy();
    }

    public static void MarkAbilitiesWithNotEnoughEnergy()
    {
        if (Player.Instance == null || Player.Instance.CurrentStance == null || Player.Instance.CurrentStance.Abilities == null)
        {
            return;
        }
        foreach (Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities)
        {
            if(ability.AbilityGraphic != null && (ability == null || ability.Type == null)) {
                    ability.AbilityGraphic.transform.Find("Stroke").GetComponent<Image>().color = Colors.AbilityEnoughEnergyBorderColor;
                    ability.AbilityGraphic.transform.Find("Corners").GetComponent<Image>().color = Colors.AbilityEnoughEnergyBorderColor;
            }
            else if(ability.AbilityGraphic != null){
                float cost = Ability.GetEnergyCost(ability.Type);
                if (cost > 0)
                {
                    ability.AbilityGraphic.transform.Find("Stroke").GetComponent<Image>().color = cost <= Player.Instance.Energy.Current ? Colors.AbilityEnoughEnergyBorderColor : Colors.AbilityNotEnoughEnergyBorderColor;
                    ability.AbilityGraphic.transform.Find("Corners").GetComponent<Image>().color = cost <= Player.Instance.Energy.Current ? Colors.AbilityEnoughEnergyBorderColor : Colors.AbilityNotEnoughEnergyBorderColor;
                }
            }
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = ((int)Current).ToString() + " / " + ((int)Maximum).ToString();
    }
}