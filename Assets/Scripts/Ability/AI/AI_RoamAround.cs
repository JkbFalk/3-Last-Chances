using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_RoamAround : AI {

    private bool _reRoamQueued = false;
    public AI_RoamAround(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.UnitAI.NavMeshAgent.enabled = true;
        User.UnitAI.WaitTimeBeforeNextAction += 200;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Repositioning;
        User.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
        User.UnitAI.CurrentDirectionType = UnitAI.DirectionType.FaceDirectionOfCurrentMovement;
        User.PlayAnimation("Run");
        Vector3 intended_position = new Vector2(User.transform.position.x, User.transform.position.y) + UnityEngine.Random.insideUnitCircle * new Vector2(5f, 5f);
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(intended_position, out closestHit, 500, 1))
        {
            User.UnitAI.NavMeshAgent.SetDestination(intended_position);
        }
        PerformActionAfterIntervals(10, 0.2f);
    }

    public override void ActionToPerformAfterIntervals()
    {
        if(_reRoamQueued == false && AbilityEnded == false && User.UnitAI.NavMeshAgent.remainingDistance < 0.1f && User.InCombat == false) {
            _reRoamQueued = true;
            User.PlayAnimation("SearchingForEnemies");
            GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(1f, 2), ReRoam);
        }
    }

    public void ReRoam() {
        User.Actions.UseAbility(typeof(AI_RoamAround));
    }
}