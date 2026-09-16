using System;
using TMPro;
using UnityEngine;

public class Tenacity : Stat {

    public Tenacity(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("Tenacity/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = stat_owner != null && stat_owner.ScaleStatsWithLevel ? base_amount * CombatMath.GetExpectedControlLevel(stat_owner.Level) : base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged()
    {
        Owner.Animator.SetFloat("Tenacity", Current);
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 0 ? "" : "+") + Utils.GetFormattedFloat(Current);
    }
}