using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Effect;

public class Effect_RipostingSpecificAbility : Effect
{
    public int MaxRipostes = 5;
    public List<Type> AbilitiesToRiposte = new List<Type>();
    public Effect_RipostingSpecificAbility(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        if(damage?.TargetOfDamage != TargetOfEffect || TargetOfEffect.EffectCooldowns.FirstOrDefault(e => e.Type == GetType()) != null || damage.DamagingObject == null || damage.DamagingObject is not Projectile) {
            return;
        }
        if(UnityEngine.Random.Range(0, 100) < 99 && (AbilitiesToRiposte.Contains(damage.SourceOfDamage.GetType()) || (damage.SourceOfDamage.OriginalRipostedAbility != null && AbilitiesToRiposte.Contains(damage.SourceOfDamage.OriginalRipostedAbility.GetType())))) {
            if(damage.SourceOfDamage.OriginalRipostedAbility.RipostedCount >= MaxRipostes) {
                return;
            }
            TargetOfEffect.Actions.EndCurrentAbility();
            TargetOfEffect.PlayAnimation(TargetOfEffect.DamageCategory == Constants.DamageType.Heavy ? "Greatsword_Riposte" : TargetOfEffect.DamageCategory == Constants.DamageType.Light ? "TwinBlades_Riposte" : TargetOfEffect.DamageCategory == Constants.DamageType.Ranged ? "Gun_Riposte" : "Gun_Riposte");
            damage.DestroyProjectileAfterDamageCalcuation = true;
            TargetOfEffect.AddCooldown(this, 1.5f);
            Projectile proj = Utils.SendProjectileBackTowardsSource(damage, TargetOfEffect, damage.DamagingObject.SourceAbility, true);
            proj.SourceAbility = damage.DamagingObject.SourceAbility.OriginalRipostedAbility;
            base.OnInvokeDamageDealt(damage);
            GameController.Instance.WaitAndRunMethod(0.5f, PerformOtherAction, damage.TargetOfDamage);
        }
    }

    public void PerformOtherAction(Unit u) {
        u.UnitAI.DecideOnNextAction();
    }
}
