using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushOrPullUnits : MonoBehaviour
{
    public enum Mode { Push, Pull};
    /// <summary>
    /// If higher than 1, units farther away will be affected more strongly. If lower than 1, units farther way be affected less strongly. For example, setting this value to 0.5 will make units at the edge only be affected half as strongly.
    /// </summary>
    public float AffectedByDistance = 1;
    public float HighestPossibleDistance = 1;
    public float Force = 0;
    public bool OncePerUnit = true;
    /// <summary>
    /// If this is enabled, then the vector is not normalized, thus affecting enemies outside with enough force to pull them into center of the object.
    /// </summary>
    public bool PullIntoSpot = false;
    public float DisableNSecondsAfterStart = 0;
    private int _disableTimer = 0;
    private AreaOfEffect _parentAreaOfEffect;
    public Mode CurrentMode = PushOrPullUnits.Mode.Push;
    public List<Unit> AffectedUnits = new List<Unit>();
    public bool AffectOnlyEnemies = true;

    public void Awake()
    {
        _parentAreaOfEffect = GetComponent<AreaOfEffect>();
        if(DisableNSecondsAfterStart != 0)
        {
            _disableTimer = (int)(DisableNSecondsAfterStart * 50);
        }
    }

    public void Update()
    {
        if(_disableTimer > 0)
        {
            _disableTimer--;
            if(_disableTimer == 0)
            {
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActiveAndEnabled)
        {
            AffectUnit(other);
        } 
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isActiveAndEnabled)
        {
            AffectUnit(other);
        }
    }

    private void AffectUnit(Collider2D other)
    {
        if (Utils.CheckIfColliderIsValidUnitHitbox(other) == false || _parentAreaOfEffect?.SourceAbility == null || AffectedUnits == null)
        {
            return;
        }
        Rigidbody2D rigidbody = other.GetComponentInParent<Rigidbody2D>();
        Unit unit_being_affected = other.GetComponentInParent<Unit>();
        if(unit_being_affected == null || rigidbody == null) {
            return;
        }
        if ((AffectOnlyEnemies == false || (unit_being_affected != _parentAreaOfEffect.SourceAbility.User && unit_being_affected.CheckIfHostileTowards(_parentAreaOfEffect.SourceAbility.User.Faction))) && (!AffectedUnits.Contains(unit_being_affected) || OncePerUnit == false) && unit_being_affected != null && rigidbody != null)
        {
            Vector2 vector = Vector2.zero;
            if (CurrentMode == Mode.Push)
            {
                vector = unit_being_affected.transform.position - transform.position;
            }
            else if (CurrentMode == Mode.Pull)
            {
                vector = transform.position - unit_being_affected.transform.position;
            }
            rigidbody.AddForce((PullIntoSpot ? vector : vector.normalized) * Force * (OncePerUnit ? 1 : Time.deltaTime * 100) * GetDistanceModifier(unit_being_affected.transform.position), ForceMode2D.Force);
            if(!AffectedUnits.Contains(unit_being_affected))
            {
                AffectedUnits.Add(unit_being_affected);
            }
        }
    }

    private float GetDistanceModifier(Vector2 target_position)
    {
        if (AffectedByDistance != 1)
        {
            float distance = Vector2.Distance(transform.position, target_position);
            return Math.Abs(AffectedByDistance - 1) / (distance < 0.5f ? 0.5f : distance);
        }
        else return AffectedByDistance;
    }
}
