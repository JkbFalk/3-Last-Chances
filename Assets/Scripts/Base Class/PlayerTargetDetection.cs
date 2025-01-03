using System.Linq;
using UnityEngine;

public class PlayerTargetDetection : MonoBehaviour {

    int _counter = 0;
    public void FixedUpdate() {
        _counter++;
        if(_counter >= 50) {
            _counter = 0;
            foreach(Unit u in Player.Instance.PotentialTargets.ToList()) {
                if(Vector2.Distance(u.transform.position, Player.Instance.transform.position) > 5) {
                    Player.Instance.PotentialTargets.Remove(u);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        AddTarget(other);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        AddTarget(other);
    }

    private void AddTarget(Collider2D other)
    {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false || other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            return;
        }
        Unit enemyUnit = other.GetComponentInParent<Unit>();
        if (enemyUnit != null && enemyUnit.IsHostile && !Player.Instance.PotentialTargets.Contains(enemyUnit))
        {
            Player.Instance.PotentialTargets.Add(enemyUnit);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Unit enemyUnit = other.GetComponentInParent<Unit>();
        if (enemyUnit != null && enemyUnit.IsHostile && Player.Instance.PotentialTargets.Contains(enemyUnit))
        {
            Player.Instance.PotentialTargets.Remove(enemyUnit);
        }
    }
}