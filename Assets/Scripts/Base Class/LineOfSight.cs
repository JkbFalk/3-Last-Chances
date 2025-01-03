using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour {
    public List<Unit> EnteredDuringCrowdControl = new();
    private Unit _unit;

    private void Start() {
        _unit = GetComponentInParent<Unit>();
        if(_unit != null && _unit.Actions != null) {
            _unit.Actions.LineOfSight = this;
        }
    }

    public void HandleExitCrowdControl() {
        foreach(Unit unit in EnteredDuringCrowdControl) {
            CheckUnit(unit);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_unit == null ||  _unit.Actions == null || Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.gameObject.name == "Backstep Hitbox")
        {
            return;
        }
        Unit unit_being_seen = other.GetComponentInParent<Unit>();
        if (_unit.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl) {
            EnteredDuringCrowdControl.Add(unit_being_seen);
            return;
        }
        CheckUnit(unit_being_seen);
    }

    private void CheckUnit(Unit unit_being_seen) {
        if (_unit != null && unit_being_seen != null && unit_being_seen != _unit && _unit.CheckIfHostileTowards(unit_being_seen.Faction)) {
            if (_unit.CurrentTarget == null) {
                _unit.CurrentTarget = unit_being_seen;
            }
            else if (!_unit.PotentialTargets.Contains(unit_being_seen)) {
                _unit.PotentialTargets.Add(unit_being_seen);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (_unit == null ||  _unit.Actions == null || Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.gameObject.name == "Backstep Hitbox") {
            return;
        }
        Unit unit_being_seen = other.GetComponentInParent<Unit>();
        if (_unit != null && _unit.PotentialTargets.Contains(unit_being_seen)) {
            _unit.PotentialTargets.Remove(unit_being_seen);
            EnteredDuringCrowdControl.Remove(unit_being_seen);
        }
    }
}