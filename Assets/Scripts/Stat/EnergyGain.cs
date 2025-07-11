using System;
using TMPro;
using UnityEngine;

public class EnergyGain : Stat {

    public EnergyGain(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("EnergyGain/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 0 ? "" : "+") + Utils.GetFormattedFloat(Current);
    }
}