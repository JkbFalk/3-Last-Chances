using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_GainProjectileSpeedAndDamageWithDistanceTravelled : Effect
{
    public float MaxRangeInMeters = 15;
    public float MaxDistanceBuffPercentage;
    public float FlightSpeedBuff = 0.125f;
    public float DamageBuffPercentage;
    private float _originalSpeed;
    private Dictionary<Projectile, Vector2> _buffedProjectilesAndLastPositions = new();
    private Dictionary<Projectile, Vector2> _buffedProjectilesAndStartPositions = new();

    public Effect_GainProjectileSpeedAndDamageWithDistanceTravelled(float damage_increase_at_max_range, float distance_increase, SourceOfEffect source_of_effect) : base(source_of_effect) {
        DamageBuffPercentage = damage_increase_at_max_range;
        MaxDistanceBuffPercentage = distance_increase;
        Type = EffectType.Buff;
        DescriptionParameters = new List<string>() { Utils.GetFormattedFloat(DamageBuffPercentage), Utils.GetFormattedFloat(MaxRangeInMeters), Utils.GetFormattedFloat(MaxDistanceBuffPercentage) };
        Listeners.Add(EventManager.ProjectileCreated);
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeProjectileCreated(Projectile projectile)
    {
        if (projectile.SourceAbility.User != TargetOfEffect || projectile.SourceAbility.ScalesWith != Constants.DamageType.Ranged) {
            return;
        }
        base.OnInvokeProjectileCreated(projectile);
        _buffedProjectilesAndLastPositions.Add(projectile, projectile.transform.position);
        _buffedProjectilesAndStartPositions.Add(projectile, projectile.transform.position);
        projectile.MaxFlightDistance *= 1 + MaxDistanceBuffPercentage / 100;
        projectile.FlightSpeed *= 0.5f;
        _originalSpeed = projectile.FlightSpeed;
    }

    public override void OnFixedUpdate()
    {
        foreach(Projectile proj in _buffedProjectilesAndLastPositions.Keys.ToArray()) {
            if (proj.IsDestroyed || proj.gameObject== null) {
                _buffedProjectilesAndLastPositions.Remove(proj);
                _buffedProjectilesAndStartPositions.Remove(proj);
            }
            else {
                proj.FlightSpeed += _originalSpeed * Vector2.Distance(_buffedProjectilesAndLastPositions[proj], proj.transform.position) * FlightSpeedBuff / 100;
                _buffedProjectilesAndLastPositions[proj] = proj.transform.position;
            }
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        base.OnInvokeHitDealt(damage);
        if (damage.SourceOfDamage.User is not Player || damage.DamagingObject == null || _buffedProjectilesAndStartPositions.Keys.Contains(damage.DamagingObject) == false)
        {
            return;
        }
        Projectile proj = (Projectile)damage.DamagingObject;
        damage.DamageDealtPercentageModifier += DamageBuffPercentage * Vector2.Distance(_buffedProjectilesAndStartPositions[proj], proj.transform.position);
        Effect armorPen = Player.Instance.GetEffect(new System.Func<Effect, bool>(effect => effect.Id == "ArmorPenetrationBasedOnFlightTime"));
        if (armorPen != null)
        {
            damage.ArmorPenetrationModifier += armorPen.PercentageAmount;
        }
    }
}
