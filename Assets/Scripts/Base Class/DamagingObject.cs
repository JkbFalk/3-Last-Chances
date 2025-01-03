using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using System.Linq;
using System;
using System.Reflection;
using Unity.VisualScripting;

public class DamagingObject : TemporaryObject {
    [HideInInspector]
    public Unit Owner;
    public bool CanBeRiposted = true;
    public bool DealingDamage = true;
    public bool WasStopped = false;

    private void OnTriggerStay2D(Collider2D other) {
        HandleCollision(other);
    }

    public void HandleCollision(Collider2D other) {
        if (GameController.Instance.GameplayMode != Constants.GameplayMode.Regular || !DealingDamage || SourceAbility == null || SourceAbility.User == null || SourceAbility.User.KnockedOut || SourceAbility.User.gameObject.activeSelf == false || other.gameObject.activeSelf == false)
        {
            return;
        }
        else if (other.gameObject.name == "Backstep Hitbox" && (SourceAbility == null || SourceAbility.AbilityModifiers.Contains(Constants.AbilityModifier.CounteredByBackstep) == false))
        {
            return;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Environment") && other.gameObject.name != "Environment Collision" && Owner.Faction != Constants.Faction.DuelingEachOther)
        {
            HandleDestructibleHit(other);
        }
        else if (other != null && other.IsDestroyed() == false && other.GetComponent<Projectile>() != null && other.GetComponent<Projectile>().SourceAbility?.User != Owner && Owner != null && Owner.Actions.CurrentAbilityBeingPerformed != null && Owner.Actions.CurrentAbilityBeingPerformed.IsBasicAttack && other.GetComponent<Projectile>().DealingDamage && other.GetComponent<Projectile>().CanBeRiposted && ((BasicAttack)Owner.Actions.CurrentAbilityBeingPerformed).DealingDamage) {
            Projectile projectile = other.GetComponent<Projectile>();
            Type riposteType = System.Type.GetType(Owner.CurrentWeaponClass.ToString() + "_Riposte");
            Riposte riposte = (Riposte)Activator.CreateInstance(riposteType, new object[] { Owner });
            riposte.Target = projectile.SourceAbility.User;
            if (Owner.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
            {
                Owner.Energy.GenerateEnergy(Constants.EnergyGainSource.Riposte, projectile.SourceAbility.User.IsBoss);
            }
            Damage attemptedDamage = new(Owner, projectile.SourceAbility, projectile);
            Utils.SendProjectileBackTowardsSource(attemptedDamage, Owner, riposte, true);
        }
        else if(other.CompareTag("Hitbox") == true && (other is CapsuleCollider2D || other is BoxCollider2D)){
            Unit unit_being_attacked = other.GetComponentInParent<Unit>();
            if (unit_being_attacked != null && unit_being_attacked != SourceAbility.User && SourceAbility.User.CheckIfHostileTowards(unit_being_attacked.Faction)) {
                SourceAbility.HandleEnemyHit(unit_being_attacked, this, other);
            }   
        }
    }

    protected virtual void AdditionalActionsOnCollision() {}
    protected virtual void AdditionalActionsOnDestructibleHit() {}

    private void HandleDestructibleHit(Collider2D other)
    {
        DestructibleEnvironment dest = other.GetComponent<DestructibleEnvironment>();
        if(dest != null && !string.IsNullOrWhiteSpace(dest.ClassAndMethodCheckIfDestructible)) {
            MethodInfo method = Type.GetType(dest.ClassAndMethodCheckIfDestructible.Split(".")[0]).GetMethod(dest.ClassAndMethodCheckIfDestructible.Split(".")[1], BindingFlags.Public | BindingFlags.Static);
            if((bool)method.Invoke(null, new object[] {}) == false) {
                return;
            }
        }
        if(dest != null && (dest.LastTimeHitSoundWasPlayed == null || (DateTime.Now - dest.LastTimeHitSoundWasPlayed).TotalSeconds > 0.35f)) {
            Utils.PlaySoundEffect(SourceAbility.User.AudioSource, "Hit/" + dest.AnimationType + "_Hit" + Utils.GetRandomSoundNumber( dest.AnimationType + "_Hit"), 0.5f);
            dest.LastTimeHitSoundWasPlayed = DateTime.Now;
        } 
        if(dest != null && dest.LevelRequiredToDealDamage < 101 && SourceAbility.User is Player && dest.LevelRequiredToDealDamage > SaveFile.Instance.Level + 10) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="NotHighEnoughLevelForDestructibleNotification"});
        }
        else if(dest != null && dest.LevelRequiredToDealDamage < 101 && SourceAbility.User is Player && dest.LevelRequiredToDealDamage > SaveFile.Instance.Level) {
            NotificationController.ShowCustomizedDialogueNotification(new() {Id="SlightlyNotHighEnoughLevelForDestructibleNotification"});
        }
        else if (dest != null && dest.LevelRequiredToDealDamage <= SaveFile.Instance.Level && !SourceAbility.AffectedDestructibles.ContainsKey(dest))
        {
            SourceAbility.UpdateAffectedDestructibleList(dest, this);
            DamageSource source = SourceAbility.GetDamageSource(gameObject.name);
            float damage = (SourceAbility.GetInjuryStatForDamageSource(source) == null ? 100 : SourceAbility.GetInjuryStatForDamageSource(source).Current) * source.InjuryScaling / 100 + (SourceAbility.GetStaggerStatForDamageSource(source) == null ? 100 : SourceAbility.GetStaggerStatForDamageSource(source).Current) * source.StaggerScaling / 100;
            Utils.CreateAuditLog("Destructible (" + dest.gameObject + ") is taking " + damage + " damage from ability " + SourceAbility.ToString() + " and source " + source.DamageType);
            dest.HitPoints -= damage;
            AdditionalActionsOnDestructibleHit();
            if (dest.HitPoints <= 0)
            {
                dest.DestroyObject();
                EventManager.DestructibleDestroyed.Invoke(dest);
            }
            else
            {
                dest.HandleObjectHit();
            }
        }
        else if(other.gameObject.name != "Environment Collision" && this is Projectile) {
            ((Projectile)this).HandleWallHit();
        }
    }
}