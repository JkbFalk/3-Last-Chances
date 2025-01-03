using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_RangedCombatReposition : AI {

    public AI_RangedCombatReposition(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        if(User.UnitAI.NavMeshAgent.enabled == false || User?.CurrentTarget == null)
        {
            EndThisAbility();
        }
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.CirclingAround;
        User.UnitAI.WaitTimeBeforeNextAction += 30;
        User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
        User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.AlwaysFaceTargetUnit;
        target = target == null ? User.CurrentTarget : target;
        Vector2 repositionLocation = new Vector2(target.transform.position.x + (User.transform.position.x > target.transform.position.x ? 5 : -5), target.transform.position.y);
        if (NavMesh.SamplePosition(repositionLocation + UnityEngine.Random.insideUnitCircle * new Vector2(3.5f, 2.5f), out NavMeshHit repositionHit, 3f, NavMesh.AllAreas))
        {
            if(User.UnitAI.MaxMoveRange != null && !User.UnitAI.MaxMoveRange.bounds.Contains(repositionLocation) && User.UnitAI.NavMeshAgent.isOnNavMesh) {
                User.UnitAI.NavMeshAgent.SetDestination(User.UnitAI.MaxMoveRange.ClosestPoint(repositionHit.position));
            }
            else if(User.UnitAI.NavMeshAgent.isOnNavMesh){
                User.UnitAI.NavMeshAgent.SetDestination(repositionHit.position);
            }
        }
    }
}