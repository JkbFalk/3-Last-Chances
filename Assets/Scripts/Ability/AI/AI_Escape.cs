using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Escape : AI {

    public AI_Escape(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        if (User.Health.Current < User.Health.Maximum * 0.25f)
        {
            User.UnitAI.WaitTimeBeforeNextAction += 80;
            User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Escaping;
            User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
            User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.FaceDirectionOfCurrentMovement;
            if (target != null)
            {
                Vector2 escapePosition = Utils.GetPositionGivenDistanceAwayBasedOnTwoPoints(target.transform.position, User.transform.position, UnityEngine.Random.Range(10, 13));
                if (NavMesh.SamplePosition(escapePosition, out NavMeshHit escapePositionHit, 10f, NavMesh.AllAreas))
                {
                    if(User.UnitAI.MaxMoveRange != null && !User.UnitAI.MaxMoveRange.bounds.Contains(escapePosition) && User.UnitAI.NavMeshAgent.isOnNavMesh) {
                        User.UnitAI.NavMeshAgent.SetDestination(User.UnitAI.MaxMoveRange.ClosestPoint(escapePosition));
                    }
                    else if(User.UnitAI.NavMeshAgent.isOnNavMesh){
                        User.UnitAI.NavMeshAgent.SetDestination(escapePosition);
                    }
                }
            }
        }
    }
}