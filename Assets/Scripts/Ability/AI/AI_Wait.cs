using System.Collections.Generic;
using UnityEngine;

public class AI_Wait : AI {

    public AI_Wait(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        User.UnitAI.WaitTimeBeforeNextAction += 20;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Observing;
        User.Actions.EndCurrentAbility();
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.PlayAnimation("IdleInCombat");
    }
}