using System.Collections.Generic;
using UnityEngine;

public class AI_TryToFlank : AI {

    public AI_TryToFlank(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        if(User.UnitAI.NavMeshAgent.enabled == false || User?.CurrentTarget == null)
        {
            EndThisAbility();
        }
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.CirclingAround;
        target = target == null ? User.CurrentTarget : target;
        float HorizontalAdjustment = target.Actions.IsFlipped ? 2.5f : -2.5f;
        float VerticalAdjustment = UnityEngine.Random.Range(0, 100) > 50 ? 2.5f : -2.5f;
        Vector2 targetPosition = new Vector2(target.transform.position.x + HorizontalAdjustment, target.transform.position.y + VerticalAdjustment);
        User.UnitAI.WaitTimeBeforeNextAction += 50;
        User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
        User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.AlwaysFaceTargetUnit;
        if(User.UnitAI.MaxMoveRange != null && !User.UnitAI.MaxMoveRange.bounds.Contains(targetPosition) && User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.UnitAI.NavMeshAgent.SetDestination(User.UnitAI.MaxMoveRange.ClosestPoint(targetPosition));
        }
        else if(User.UnitAI.NavMeshAgent.isOnNavMesh){
            User.UnitAI.NavMeshAgent.SetDestination(targetPosition);
        }
    }
}