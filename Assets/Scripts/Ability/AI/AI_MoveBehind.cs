using System.Collections.Generic;
using UnityEngine;

public class AI_MoveBehind : AI{

    public AI_MoveBehind(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.CirclingAround;
        target = target == null ? User.CurrentTarget : target;
        float HorizontalAdjustment = target.Actions.IsFlipped ? 1f : -1f;
        Vector2 targetPosition = new Vector2(target.transform.position.x + HorizontalAdjustment, target.transform.position.y);
        User.UnitAI.WaitTimeBeforeNextAction += 50;
        User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
        User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.AlwaysFaceTargetUnit;
        if(User.UnitAI.MaxMoveRange != null && !User.UnitAI.MaxMoveRange.bounds.Contains(targetPosition) && User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.UnitAI.NavMeshAgent.SetDestination(User.UnitAI.MaxMoveRange.ClosestPoint(targetPosition));
        }
        else if(User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.UnitAI.NavMeshAgent.SetDestination(targetPosition);
        }
    }

}