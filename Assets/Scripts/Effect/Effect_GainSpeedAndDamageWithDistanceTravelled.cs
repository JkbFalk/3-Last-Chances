using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_GainSpeedAndDamageWithDistanceTravelled : Effect
{
    public float MaxRangeBuff = 0.25f;
    public float FlightSpeedBuff = 0.125f;
    public float DamageBuff = 0.2f;
    private float _originalSpeed;
    private Dictionary<Projectile, Vector2> _buffedProjectilesAndLastPositions = new();
    private Dictionary<Projectile, Vector2> _buffedProjectilesAndStartPositions = new();

    public Effect_GainSpeedAndDamageWithDistanceTravelled(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        TriggersOncePerAbility = true;
        Listeners.Add(EventManager.ProjectileCreated);
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        MaxRangeBuff *= NonLinearEffectValue;
        FlightSpeedBuff *= NonLinearEffectValue;
        DamageBuff *= LinearEffectValue;
        DescriptionParameters = new() {Utils.GetFormattedFloat(MaxRangeBuff, 1), Utils.GetFormattedFloat(FlightSpeedBuff, 1), Utils.GetFormattedFloat(DamageBuff, 1)};
    }

    public override void OnInvokeProjectileCreated(Projectile projectile)
    {
        if (projectile.SourceAbility.User != TargetOfEffect || projectile.SourceAbility.DamageType != Constants.DamageType.Ranged) {
            return;
        }
        base.OnInvokeProjectileCreated(projectile);
        _buffedProjectilesAndLastPositions.Add(projectile, projectile.transform.position);
        _buffedProjectilesAndStartPositions.Add(projectile, projectile.transform.position);
        projectile.MaxFlightDistance *= 1 + MaxRangeBuff / 100;
        projectile.FlightSpeed *= 0.5f;
        _originalSpeed = projectile.FlightSpeed;
    }

    public override void OnFixedUpdate()
    {
        foreach(Projectile proj in _buffedProjectilesAndLastPositions.Keys.ToArray()) {
            if (proj.IsDestroyed || proj.gameObject.IsDestroyed()) {
                _buffedProjectilesAndLastPositions.Remove(proj);
                _buffedProjectilesAndStartPositions.Remove(proj);
            }
            else {
                proj.FlightSpeed += _originalSpeed * Vector2.Distance(_buffedProjectilesAndLastPositions[proj], proj.transform.position) * FlightSpeedBuff / 100;
                _buffedProjectilesAndLastPositions[proj] = proj.transform.position;
            }
        }
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        base.OnInvokeHitDealt(damage);
        if(damage.SourceOfDamage.User is not Player || damage.DamagingObject == null || _buffedProjectilesAndStartPositions.Keys.Contains(damage.DamagingObject) == false) {
            return;
        }
        Projectile proj = (Projectile)damage.DamagingObject;
        damage.ExtraInjuryDealtPercentage += DamageBuff * Vector2.Distance(_buffedProjectilesAndStartPositions[proj], proj.transform.position);
        damage.ExtraStaggerDealtPercentage += DamageBuff * Vector2.Distance(_buffedProjectilesAndStartPositions[proj], proj.transform.position);
    }
}
