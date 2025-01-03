using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    [HideInInspector]
    public Unit Unit;
    public float MaxDistanceFromUnit = 3;
    private Vector2 _currentDestination;
    public bool IsObservingFromAfar = false;

    public void Start() {
        Unit = GetComponent<Unit>();
    }
    void FixedUpdate()
    {
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.InCutscene) {
            if(Unit.UnitAI.CurrentAIBehavior == Constants.AIBehavior.Repositioning) {
                Unit.UnitAI.CurrentAIBehavior = Constants.AIBehavior.None;
                Unit.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
                Unit.UnitAI.NavMeshAgent.isStopped = true;
            }
            return;
        }
        if(Unit.InCombat == false && Player.Instance.InCombat) {
            Unit.CurrentTarget = Player.Instance.CurrentTarget;
        }
        if(Player.Instance.InCombat) {
            return;
        }
        float distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if(distance > MaxDistanceFromUnit) {
            NavMeshHit repositionHit;
            if (NavMesh.SamplePosition((Player.Instance.transform.position + transform.position) / 2, out repositionHit, 1.5f, NavMesh.AllAreas))
            {
                if(IsObservingFromAfar && Vector2.Distance(repositionHit.position, Unit.transform.position) < 5) {
                    return;
                }
                Unit.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Repositioning;
                Unit.Actions.CurrentActionBeingPerformed = Constants.ActionType.Moving;
                _currentDestination = repositionHit.position;
                Unit.UnitAI.NavMeshAgent.SetDestination(_currentDestination);
                Unit.Actions.IsFlipped = Player.Instance.transform.position.x < Unit.transform.position.x;
            }
        }
        else if(distance < MaxDistanceFromUnit && Vector2.Distance(Unit.transform.position, _currentDestination) < 0.02f){
            Unit.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Waiting;
            Unit.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
            Unit.Actions.IsFlipped = Player.Instance.transform.position.x < Unit.transform.position.x;
        }
    }
}
