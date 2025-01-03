using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageReduction : Stat {

    public DamageReduction(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("DamageReduction/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 1 ? "" : "+") + Math.Round((Current - 1) * 100, 0).ToString() + "%" + " (" + Utils.GetFormattedFloat(1/Current, 2, true) + ")";
    }
}