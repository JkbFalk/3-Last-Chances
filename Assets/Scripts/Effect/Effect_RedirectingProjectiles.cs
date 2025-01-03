using UnityEngine;

public class Effect_RedirectingProjectiles : Effect {

    public Effect_RedirectingProjectiles(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners.Add(EventManager.HitDealt);
        Type = EffectType.Buff;
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if (damage.TargetOfDamage == TargetOfEffect && damage.DamagingObject != null && (damage.DamagingObject is Projectile || damage.DamagingObject.GetType().IsSubclassOf(typeof(Projectile))) && damage.DamagingObject.CanBeRiposted) {
            damage.DestroyProjectileAfterDamageCalcuation = true;
            Utils.SendProjectileBackTowardsSource(damage, TargetOfEffect, SourceOfEffect.SourceAbility, true);
            damage.DamagingObject.SourceAbility = SourceOfEffect.SourceAbility;
            base.OnInvokeHitDealt(damage);
        }
    }
}