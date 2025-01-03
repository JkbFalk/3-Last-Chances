using System.Collections.Generic;
using UnityEngine;

public class AI_ShieldAlly : AI {

    public AI_ShieldAlly(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Repositioning;
        target = target == null ? User.CurrentTarget : target;
        if (target.MostRecentEnemyHit != User)
        {
            User.UnitAI.WaitTimeBeforeNextAction += 100;
            User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
            User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.AlwaysFaceTargetUnit;
            if(User.UnitAI.NavMeshAgent.isOnNavMesh) {
                User.UnitAI.NavMeshAgent.SetDestination(User.CurrentTarget.transform.localPosition);
            }
        }
    }

}