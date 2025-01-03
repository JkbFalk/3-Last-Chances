using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaggerBar : Stat {
    public Image HUDFill;
    public GameObject StaggerBars;

    public StaggerBar(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        Owner = stat_owner;
        if (Owner is Player) {
            HUDSlider = CanvasElements.UICanvas.ResourceBars.transform.Find("Stagger Bar Container/Stagger Bar").GetComponent<Slider>();
            HUDFill = HUDSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("StaggerBar/Value").GetComponent<TextMeshProUGUI>();
        }
        else if (Owner.IsBoss == false) {
            HUDSlider = Owner.transform.Find("World Space Canvas/Stagger Bar").GetComponent<Slider>();
            HUDFill = HUDSlider.transform.Find("Fill Area/Fill").GetComponent<Image>();
            StaggerBars = Owner.transform.Find("World Space Canvas/Stagger Bar/Extra Stagger Bars").gameObject;
        }
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * Utils.GetExpectedPowerForLevel(stat_owner.Level) : base_amount;
        Maximum = Base;
        CannotBeLowerThan1 = true;
        CurrentCanBeLowerThanMaximum = true;
        Current = 0;
        AddPercentageRegeneration(Constants.StatSource.Base, Owner.DefaultStaggerBarRegenPercentage);   
        AdditionalStatSpecificActionsAfterRecalculatingMaximumAmount();
    }

    public void DealStaggerDamage(Damage damage) {
        if(Current + damage.StaggerDealt > Maximum) {
            damage.StaggerDealt = Maximum - Current + 0.1f;
        }
        else {
            damage.StaggerDealt = damage.Stagger;
        }
        Current += damage.StaggerDealt;
        if(Current + damage.Stagger > Maximum && damage.Properites.Contains(Damage.DamageProperty.CannotStagger)) {
            Current = Maximum - 0.1f;
            return;
        }
        if (Current >= Maximum && Owner is Player) {
            Owner.AddEffect(new Effect_PlayerStaggered(new(damage.SourceOfDamage)), Constants.DEFAULT_PLAYER_STAGGERED_DURATION);
        }
        else if (Current >= Maximum) {
            Owner.IsStaggered = true;
            damage.Injury += (damage.Stagger - damage.StaggerDealt) / 2;
            if (Owner.CurrentStaggerBars == 1) {

                Owner.AddEffect(new Effect_HardStaggered(new(damage.SourceOfDamage)), Constants.DEFAULT_HARD_STAGGERED_DURATION);
            }
            else {
                Owner.CurrentStaggerBars--;
                Owner.StaggerBar.Maximum = Owner.StaggerBars[Owner.StaggerBars.Count - Owner.CurrentStaggerBars]* (Owner.IsHostile ? SaveFile.Instance.GlobalEnemySurvivabilityModifier : 1);
                Owner.AddEffect(new Effect_SoftStaggered(new(damage.SourceOfDamage)), Constants.DEFAULT_SOFT_STAGGERED_DURATION);
            }
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
        if(Current <= 0 && Owner is Player && Player.Instance.CheckIfUnderEffect(typeof(Effect_PlayerStaggered))) {
            Player.Instance.GetEffect(typeof(Effect_PlayerStaggered)).EndThisEffect();
        }
        else if(Current <= 0 && Owner is not Player && Owner.CheckIfUnderEffect(typeof(Effect_Staggered))) {
            Owner.GetEffect(typeof(Effect_Staggered)).EndThisEffect();
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = ((int)Current).ToString() + " / " + ((int)Maximum).ToString();
    }
}