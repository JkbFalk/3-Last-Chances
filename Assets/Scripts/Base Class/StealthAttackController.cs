using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthAttackController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.CompareTag("Destructible"))
        {
            return;
        }
        Unit unit = other.GetComponentInParent<Unit>();
        if (unit != null && unit.IsHostile)
        {
            Player.Instance.UnitsInRangeForStealthAttack.Add(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false)
        {
            return;
        }
        Unit unit = other.GetComponentInParent<Unit>();
        if (unit != null && Player.Instance.UnitsInRangeForStealthAttack.Contains(unit))
        {
            Player.Instance.UnitsInRangeForStealthAttack.Remove(unit);
        }
    }
}
