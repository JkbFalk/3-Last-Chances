using System;
using TMPro;

public class MovementSpeed : Stat {

    public MovementSpeed(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find("MovementSpeed/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        Owner.Animator.SetFloat("Movement Speed", Current);
        if (Owner.UnitAI != null && Owner.UnitAI.NavMeshAgent != null) {
            Owner.UnitAI.NavMeshAgent.speed = (Owner.IsHostile ? 3 : 5) * Current;
        }
        if(ShouldInvoke && Current < 0.2f) {
            ChangeCurrentValueWithoutInvoking(0.2f);
        }
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 1 ? "" : "+") + Math.Round((Current - 1) * 100, 0).ToString() + "%";
    }
}