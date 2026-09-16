using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour {
    private GameObject _topmostParent;
    private UnitAI _unitAI;
    private Unit _unit;

    private void Start() {
        Type AbilityType = AbilityTypeRegistry.GetByName(gameObject.name);
        if (AbilityType == null) {
            throw new System.Exception("Could not find ability with name: " + gameObject.name);
        }
        _topmostParent = transform.root.gameObject;
        _unitAI = GetComponentInParent<UnitAI>();
        _unit = GetComponentInParent<Unit>();
    }

	private void OnTriggerStay2D(Collider2D other) {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.gameObject.name == "Backstep Hitbox") {
            return;
        }
        Unit enemyUnit = other.GetComponentInParent<Unit>();
        if (_unit != null && !_unitAI.EnemiesInRangeForAbility.ContainsKey(gameObject.name) && enemyUnit != null && _unit.CheckIfHostileTowards(enemyUnit.Faction)) {
            _unitAI.EnemiesInRangeForAbility.Add(gameObject.name, enemyUnit);
        }
        if (_unit != null && enemyUnit is Player && _unit.CheckIfHostileTowards(enemyUnit.Faction)) {
            ((Player)enemyUnit).InCombatTimer = Constants.DEFAULT_FIXED_FRAMES_UNTIL_EXITING_COMBAT;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.gameObject.name == "Backstep Hitbox") {
            return;
        }
        Unit enemyUnit = other.GetComponentInParent<Unit>();
        if (enemyUnit != null) {
            _unitAI.EnemiesInRangeForAbility.Remove(gameObject.name);
        }
    }
}