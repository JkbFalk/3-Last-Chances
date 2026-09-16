using System;
using TMPro;
using UnityEngine;

public class MovementSpeed : Stat {

    public MovementSpeed(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("MovementSpeed/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }
    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        Owner.Animator.SetFloat("Movement Speed",  1 + Current / 100);
        if (Owner.UnitAI != null && Owner.UnitAI.NavMeshAgent != null) {
            Owner.UnitAI.NavMeshAgent.speed = (Owner.IsHostile ? Constants.DEFAULT_ENEMY_SPEED : Constants.DEFAULT_ALLY_SPEED) * (1 + Current / 100);
        }
        if(ShouldInvoke && Current < -80f) {
            ChangeCurrentValueWithoutInvoking(-80f);
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 0 ? "" : "+") + Utils.GetFormattedFloat(Current);
    }
}