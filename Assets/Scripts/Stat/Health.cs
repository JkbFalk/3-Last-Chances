using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : Stat {
    public GameObject HealthBars;

    public Health(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if (Owner is Player) {
            HUDSlider = CanvasElements.UICanvas.ResourceBars.transform.Find("Health Bar Container/Health").GetComponent<Slider>();
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("Health/Value").GetComponent<TextMeshProUGUI>();
        }
        else if (Owner.IsBoss == false) {
            HUDSlider = Owner.transform.Find("World Space Canvas/Health").GetComponent<Slider>();
            HealthBars = Owner.transform.Find("World Space Canvas/Health/Extra Health Bars").gameObject;
        }
        Owner = stat_owner;
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * Utils.GetExpectedPowerForLevel(stat_owner.Level): base_amount;
        Maximum = Base;
        CannotBeLowerThan1 = true;
        CurrentCanBeLowerThanMaximum = true;
        Current = Base;
        AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        if(Owner.KnockedOut && Current > 0) {
            ChangeCurrentValueWithoutInvoking(0);
            return;
        }
        EventManager.UnitHealthChanged.Invoke(Owner);
    }

    public override void AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount()
    {
        Owner.AdjustUIResourceBarsSize();
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat((int)Current)+ " / " + Utils.GetFormattedFloat((int)Maximum);
    }
}