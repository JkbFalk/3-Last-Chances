using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class Effect_CustomizableEffectOnEvent : Effect
{
    public float CustomParam = 0;
    public float PercentageAmount = 0;
    public float FlatAmount = 0;
    public Func<Item, Item, bool> ConditionCheckForItemEquipped;
    public Action<Item, Item, Effect_CustomizableEffectOnEvent> ActionOnItemEquipped;
    public Func<Ability, bool> ConditionCheckForAbilityUsed;
    public Action<Ability, Effect_CustomizableEffectOnEvent> ActionOnAbilityUsed;
    public Func<Ability, bool> ConditionCheckForAbilityEnded;
    public Action<Ability, Effect_CustomizableEffectOnEvent> ActionOnAbilityEnded;
    public Func<Damage, bool> ConditionCheckForDamageWasDodged;
    public Action<Damage, Effect_CustomizableEffectOnEvent> ActionOnDamageWasDodged;
    public Func<Projectile, bool> ConditionCheckForProjectileCreated;
    public Action<Projectile, Effect_CustomizableEffectOnEvent> ActionOnProjectileCreated;
    public Func<Effect, bool> ConditionCheckForEffectStarted;
    public Action<Effect, Effect_CustomizableEffectOnEvent> ActionOnEffectStarted;
    public Func<Effect, bool> ConditionCheckForEffectEnded;
    public Action<Effect, Effect_CustomizableEffectOnEvent> ActionOnEffectEnded;
    public Func<Effect, Effect, bool> ConditionCheckForEffectEmpowered;
    public Action<Effect, Effect, Effect_CustomizableEffectOnEvent> ActionOnEffectEmpowered;
    public Func<Damage, bool> ConditionCheckForUnitKnockedOut;
    public Action<Damage, Effect_CustomizableEffectOnEvent> ActionOnUnitKnockedOut;
    public Func<Damage, bool> ConditionCheckForHealthBarBroken;
    public Action<Damage, Effect_CustomizableEffectOnEvent> ActionOnHealthBarBroken;
    public Func<bool> ConditionCheckForAmmoAmountChanged;
    public Action<Effect_CustomizableEffectOnEvent> ActionOnAmmoAmountChanged;
    public Func<Stat, float, bool> ConditionCheckForUnitStatCurrentAmountChanged;
    public Action<Stat, float, Effect_CustomizableEffectOnEvent> ActionOnUnitStatCurrentAmountChanged;
    public Func<Cooldown, bool> ConditionCheckForCooldownAdded;
    public Action<Cooldown, Effect_CustomizableEffectOnEvent> ActionOnCooldownAdded;
    public Func<Ability, float, bool> ConditionCheckForAbilityEnergyConsumed;
    public Action<Ability, float, Effect_CustomizableEffectOnEvent> ActionOnAbilityEnergyConsumed;
    public Func<bool> ConditionCheckForOneFifthSecondElapsedNotRealtime;
    public Action<Effect_CustomizableEffectOnEvent> ActionOnOneFifthSecondElapsedNotRealtime;

    public Effect_CustomizableEffectOnEvent(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }

    public override void OnStart()
    {
        foreach(FieldInfo fieldInfo in GetType().GetFields()) {
            if(fieldInfo.Name.Contains("ConditionCheckFor") || fieldInfo.Name.Contains("ActionOn") ) {
                UnityEventBase unityEvent = (UnityEventBase)typeof(EventManager).GetField(fieldInfo.Name.Replace("ConditionCheckFor","").Replace("ActionOn", "")).GetValue(null);
                if(fieldInfo.GetValue(this) != null && !Listeners.Contains(unityEvent) && unityEvent != null) {
                    Listeners.Add(unityEvent);
                }
            }
        }
        base.OnStart();
    }

    public override void OnInvokeOneFifthSecondElapsedNotRealtime(){
        base.OnInvokeOneFifthSecondElapsedNotRealtime();
        if (ConditionCheckForOneFifthSecondElapsedNotRealtime != null && ConditionCheckForOneFifthSecondElapsedNotRealtime.Invoke() && ActionOnOneFifthSecondElapsedNotRealtime != null)
        {
            ActionOnOneFifthSecondElapsedNotRealtime.Invoke(this);
        }
    }

    public override void OnInvokeProjectileCreated(Projectile projectile){
        base.OnInvokeProjectileCreated(projectile);
        if (ConditionCheckForProjectileCreated != null && ConditionCheckForProjectileCreated.Invoke(projectile) && ActionOnProjectileCreated != null)
        {
            ActionOnProjectileCreated.Invoke(projectile, this);
        }
    }

    public override void OnInvokeAbilityUsed(Ability ability){
        base.OnInvokeAbilityUsed(ability);
        if (ConditionCheckForAbilityUsed != null && ConditionCheckForAbilityUsed.Invoke(ability) && ActionOnAbilityUsed != null)
        {
            ActionOnAbilityUsed.Invoke(ability, this);
        }
    }

    public override void OnInvokeAbilityEnded(Ability ability){
        base.OnInvokeAbilityEnded(ability);
        if (ConditionCheckForAbilityEnded != null && ConditionCheckForAbilityEnded.Invoke(ability) && ActionOnAbilityEnded != null)
        {
            ActionOnAbilityEnded.Invoke(ability, this);
        }
    }

    public override void OnInvokeEffectStarted(Effect effect) {
        base.OnInvokeEffectStarted(effect);
        if (ConditionCheckForEffectStarted != null && ConditionCheckForEffectStarted.Invoke(effect) && ActionOnEffectStarted != null)
        {
            ActionOnEffectStarted.Invoke(effect, this);
        }
    }

    public override void OnInvokeEffectEnded(Effect effect) {
        base.OnInvokeEffectStarted(effect);
        if (ConditionCheckForEffectEnded != null && ConditionCheckForEffectEnded.Invoke(effect) && ActionOnEffectEnded != null)
        {
            ActionOnEffectEnded.Invoke(effect, this);
        }
    }

    public override void OnInvokeEffectEmpowered(Effect existingEffect, Effect newEffect){
        base.OnInvokeEffectEmpowered(existingEffect, newEffect);
        if (ConditionCheckForEffectEmpowered != null && ConditionCheckForEffectEmpowered.Invoke(existingEffect, newEffect) && ActionOnEffectEmpowered != null)
        {
            ActionOnEffectEmpowered.Invoke(existingEffect, newEffect, this);
        }
    }

    public override void OnInvokeDamageWasDodged(Damage damage){
        base.OnInvokeDamageWasDodged(damage);
        if (ConditionCheckForDamageWasDodged != null && ConditionCheckForDamageWasDodged.Invoke(damage) && ActionOnDamageWasDodged != null)
        {
            ActionOnDamageWasDodged.Invoke(damage, this);
        }
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount){
        base.OnInvokeUnitStatCurrentAmountChanged(stat, amount);
        if (stat.ShouldInvoke && ConditionCheckForUnitStatCurrentAmountChanged != null && ConditionCheckForUnitStatCurrentAmountChanged.Invoke(stat, amount) && ActionOnUnitStatCurrentAmountChanged != null)
        {
            ActionOnUnitStatCurrentAmountChanged.Invoke(stat, amount, this);
        }
    }

    public override void OnInvokeCooldownAdded(Cooldown cooldown) {
        base.OnInvokeCooldownAdded(cooldown);
        if (ConditionCheckForCooldownAdded != null && ConditionCheckForCooldownAdded.Invoke(cooldown) && ActionOnCooldownAdded != null)
        {
            ActionOnCooldownAdded.Invoke(cooldown, this);
        }
    }

    public override void OnInvokeAbilityEnergyConsumed(Ability ability, float amount) {
        base.OnInvokeAbilityEnergyConsumed(ability, amount);
        if (ConditionCheckForAbilityEnergyConsumed != null && ConditionCheckForAbilityEnergyConsumed.Invoke(ability, amount) && ActionOnAbilityEnergyConsumed != null)
        {
            ActionOnAbilityEnergyConsumed.Invoke(ability, amount, this);
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2) {
        base.OnInvokeItemEquipped(item1, item2);
        if (ConditionCheckForItemEquipped != null && ConditionCheckForItemEquipped.Invoke(item1, item2) && ActionOnItemEquipped != null)
        {
            ActionOnItemEquipped.Invoke(item1, item2, this);
        }
    }

    public override void OnInvokeUnitKnockedOut(Damage damage) {
        base.OnInvokeUnitKnockedOut(damage);
        if (ConditionCheckForUnitKnockedOut != null && ConditionCheckForUnitKnockedOut.Invoke(damage) && ActionOnUnitKnockedOut != null)
        {
            ActionOnUnitKnockedOut.Invoke(damage, this);
        }
    }

    public override void OnInvokeHealthBarBroken(Damage damage) {
        base.OnInvokeHealthBarBroken(damage);
        if (ConditionCheckForHealthBarBroken != null && ConditionCheckForHealthBarBroken.Invoke(damage) && ActionOnHealthBarBroken != null)
        {
            ActionOnHealthBarBroken.Invoke(damage, this);
        }
    }

    public override void OnInvokeAmmoAmountChanged() {
        base.OnInvokeAmmoAmountChanged();
        if (ConditionCheckForAmmoAmountChanged != null && ConditionCheckForAmmoAmountChanged.Invoke() && ActionOnAmmoAmountChanged != null)
        {
            ActionOnAmmoAmountChanged.Invoke(this);
        }
    }
}
