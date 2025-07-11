using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : Stat {
    public GameObject HealthBar;
    public Slider FollowUpHealthBarSlider;
    public float FollowUpHealthBarFreezeTimer = Constants.FOLLOW_UP_HEALTH_BAR_FREEZE_TIME;

    public Health(Unit stat_owner, float base_amount) : base(stat_owner, base_amount)
    {
        if (Owner is Player)
        {
            HUDSlider = UIManager.Objects.ResourceBars.transform.Find("Health Bar Container/Health").GetComponent<Slider>();
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("Health/Value").GetComponent<TextMeshProUGUI>();
            FollowUpHealthBarSlider = UIManager.Objects.ResourceBars.transform.Find("Health Bar Container/Follow-up Health").GetComponent<Slider>();
        }
        else if (Owner.IsBoss == false)
        {
            HUDSlider = Owner.transform.Find("World Space Canvas/Health").GetComponent<Slider>();
            HealthBar = Owner.transform.Find("World Space Canvas/Health/Extra Health Bars").gameObject;
            FollowUpHealthBarSlider = Owner.transform.Find("World Space Canvas/Follow-up Health").GetComponent<Slider>();
        }
        Owner = stat_owner;
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * Utils.GetExpectedPowerForLevel(stat_owner.Level) : base_amount;
        Maximum = Base;
        CannotBeLowerThan1 = true;
        CurrentCanBeLowerThanMaximum = true;
        Current = Base;
        AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        if(FollowUpHealthBarSlider == null || HUDSlider == null) {
            return;
        }
        if(FollowUpHealthBarSlider.value < HUDSlider.value) {
            FollowUpHealthBarSlider.value = HUDSlider.value;
        }
        FollowUpHealthBarFreezeTimer = Constants.FOLLOW_UP_HEALTH_BAR_FREEZE_TIME;
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