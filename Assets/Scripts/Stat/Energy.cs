using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : Stat {

    public Energy(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if (Owner is Player) {
            HUDSlider = UIManager.Objects.ResourceBars.transform.Find("Energy").GetComponent<Slider>();
            AmountDisplay = UIManager.Objects.ResourceBars.transform.Find("Energy/Amount").GetComponent<TextMeshProUGUI>();
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("Energy/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0;
    }

    public void GenerateEnergy(Constants.EnergyGainSource source, bool is_boss = true, float health_lost_amount = 0) {
        if (!(Owner is Player) || (Owner is Player && Player.Instance.IsStaggered)) {
            return;
        }
        float current_amount_before = Current;
        float BaseGain = 0;
        switch (source) {
            case Constants.EnergyGainSource.BasicAttack: {
                    Current += Constants.ENERGY_FROM_BASIC_ATTACK * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_FROM_BASIC_ATTACK * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.Riposte: {
                    Current += Constants.ENERGY_FROM_RIPOSTING * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_FROM_RIPOSTING * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.Counter:
                {
                    Current += Constants.ENERGY_FROM_COUNTERING * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_FROM_COUNTERING * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.Dodge: {
                    Current += Constants.ENERGY_FROM_DODGING * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_FROM_DODGING * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.InflictedStaggered: {
                    Current += Constants.ENERGY_FROM_INFLICTING_STAGGERED * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_FROM_INFLICTING_STAGGERED * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.Block: {
                    Current += Constants.ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING * (1 + Player.Instance.EnergyGain.Current / 100) * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = Constants.ENERGY_PER_STAGGER_PERCENTAGE_LOST_FROM_BLOCKING * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
            case Constants.EnergyGainSource.HealthLost: {
                    float percentage = health_lost_amount / Owner.Health.Maximum * 100;
                    Current += percentage * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * (1 + Player.Instance.EnergyGain.Current / 100)  * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    BaseGain = percentage * Constants.ENERGY_PER_HEALTH_PERCENTAGE_LOST * (is_boss ? Constants.ENERGY_GAIN_MULTIPLIER_VERSUS_BOSSES : 1);
                    break;
                }
        }
        EventManager.GeneratedEnergy.Invoke(BaseGain, Current - current_amount_before, source);
    }

    public void GenerateEnergy(float amount, bool affected_by_energy_gain = true) {
        if (Owner is Player) {
            Current += amount * (affected_by_energy_gain ? (1 + Player.Instance.EnergyGain.Current / 100) : 1);
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
            }
            else if(ability.AbilityGraphic != null){
                float cost = Ability.GetEnergyCost(ability.Type);
                if (cost > 0)
                {
                    ability.AbilityGraphic.transform.Find("Stroke").GetComponent<Image>().color = cost <= Player.Instance.Energy.Current ? Colors.AbilityEnoughEnergyBorderColor : Colors.AbilityNotEnoughEnergyBorderColor;
                }
            }
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat((int)Current) + " / " + Utils.GetFormattedFloat((int)Maximum);
    }
}