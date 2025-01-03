using System.Collections.Generic;
using UnityEngine;

public class AI_Chase : AI {

    Vector3 _chasePosition;

    public AI_Chase(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        if(User.UnitAI.NavMeshAgent.enabled == false || User?.CurrentTarget == null)
        {
            EndThisAbility();
        }
        User.UnitAI.WaitTimeBeforeNextAction += 30;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.ChasingCurrentTarget;
        User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
        User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.AlwaysFaceTargetUnit;
        if (target != null)
        {
            User.CurrentTarget = target;
        }
        float horizontalAdjustment = User.CurrentTarget.transform.position.x > User.transform.position.x ? -User.DistanceAwayFromChaseTarget : User.DistanceAwayFromChaseTarget;
        _chasePosition = User.CurrentTarget.transform.localPosition + new Vector3(horizontalAdjustment, 0);
        if(User.UnitAI.MaxMoveRange != null && !User.UnitAI.MaxMoveRange.bounds.Contains(_chasePosition) && User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.UnitAI.NavMeshAgent.SetDestination(User.UnitAI.MaxMoveRange.ClosestPoint(_chasePosition));
        }
        else if(User.UnitAI.MaxMoveRange != null && User.UnitAI.NavMeshAgent.isOnNavMesh) {
            User.UnitAI.NavMeshAgent.SetDestination(_chasePosition);
        }
    }

    public override void AdditionalActionsOnUpdate()
    {
        if (Vector2.Distance(User.transform.position, _chasePosition) < 0.25f)
        {
            User.UnitAI.DecideOnNextAction();
        }
    }
}