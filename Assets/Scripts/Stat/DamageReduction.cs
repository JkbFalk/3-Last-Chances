using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageReduction : Stat {
    public TextMeshProUGUI DamageReductionLabel;
    public Image DamageReductionDisplay;

    public DamageReduction(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("DamageReduction/Value").GetComponent<TextMeshProUGUI>();
        }
        else if (Owner.IsBoss == false) {
            DamageReductionDisplay = Owner.transform.Find("World Space Canvas/Health/DamageReduction").GetComponent<Image>();
            DamageReductionLabel = Owner.transform.Find("World Space Canvas/Health/DamageReduction/Label").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        if(Owner is not Player && Current != 1 && DamageReductionLabel != null) {
            DamageReductionLabel.GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedFloat((Current - 1) * 100, 0);
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 1 ? "" : "+") + Utils.GetFormattedFloat((Current - 1) * 100) + "%" + " (" + Utils.GetFormattedFloat(Current == 0 ? 0.00001f : 1/Current, 2) + ")";
    }
}