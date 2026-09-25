using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaggerBar : Stat {
    public Image HUDFill;
    public GameObject StaggerBars;
    public float Remaining
    {
        get => Maximum - Current;
    }

    public StaggerBar(Unit stat_owner, float base_amount) : base(stat_owner, base_amount)
    {
        Owner = stat_owner;
        if (Owner is Player)
        {
            HUDSlider = UIManager.Objects.ResourceBars.transform.Find("Stagger Bar Container/Stagger Bar").GetComponent<Slider>();
            HUDFill = HUDSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
            
            // CHANGED: Get the Slider component
            HUDBlockedSlider = HUDSlider.transform.Find("Blocked Area")?.GetComponent<Slider>();
            
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("StaggerBar/Value").GetComponent<TextMeshProUGUI>();
        }
        else if (Owner.IsBoss == false)
        {
            HUDSlider = Owner.transform.Find("World Space Canvas/Stagger Bar").GetComponent<Slider>();
            HUDFill = HUDSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
            
            // CHANGED: Get the Slider component
            HUDBlockedSlider = HUDSlider.transform.Find("Blocked Area")?.GetComponent<Slider>();
            
            StaggerBars = Owner.transform.Find("World Space Canvas/Stagger Bar/Extra Stagger Bars").gameObject;
        }
        
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * CombatMath.GetExpectedPowerForLevel(stat_owner.Level) : base_amount;
        Maximum = Base;
        CannotBeLowerThan1 = true;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0;
        AddPercentageRegeneration(Constants.StatSource.Base, Owner.DefaultStaggerBarRegenPercentage);
        AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
    }

    public void DealStaggerDamage(DamageInstance damage) {
        if(Current + damage.StaggerDealt > Maximum) {
            damage.StaggerDealt = Maximum - Current + 0.1f;
        }
        else {
            damage.StaggerDealt = damage.Stagger;
        }

        Current += damage.StaggerDealt;

        if(Current + damage.Stagger > Maximum && damage.Properties.Contains(DamageInstance.DamageProperty.CannotStagger)) {
            Current = Maximum - 0.1f;
            return;
        }

        if (Current >= Maximum && Owner is Player) {
            Owner.AddEffect(new Effect_PlayerStaggered(new(damage.SourceOfDamage)), Constants.DEFAULT_PLAYER_STAGGERED_DURATION);
        }
        else if (Current >= Maximum) {
            Owner.IsStaggered = true;
            
            // Only 1 Stagger bar now! Apply the main effect directly.
            Owner.AddEffect(new Effect_Staggered(new(damage.SourceOfDamage)));
            
            if(Owner.IsHostile) {
                Player.Instance.Energy.GenerateEnergy(Constants.EnergyGainSource.InflictedStaggered, Owner.IsBoss);
            }
        }
    }

    public override void AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount()
    {
        Owner.AdjustUIResourceBarsSize();
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        base.AdditionalStatSpecificActionsAfterCurrentValueChanged();
        if(Current <= 0 && Owner is Player && Player.Instance.CheckIfUnderEffect(typeof(Effect_Staggered))) {
            Player.Instance.GetEffect(typeof(Effect_Staggered)).EndThisEffect();
        }
        else if(Current <= 0 && Owner is not Player && Owner.CheckIfUnderEffect(typeof(Effect_Staggered))) {
            Owner.GetEffect(typeof(Effect_Staggered)).EndThisEffect();
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat((int)Current) + " / " + Utils.GetFormattedFloat((int)Maximum);
    }
}