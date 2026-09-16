using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_GainStealthOnceEveryNSeconds : Effect
{
    public float Cooldown;
    public Effect_GainStealthOnceEveryNSeconds(float cooldown, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Cooldown = cooldown;
        Type = EffectType.Buff;
        Listeners.AddRange(new List<UnityEngine.Events.UnityEventBase> { EventManager.CooldownEnded, EventManager.EnterCombat });
    }

    public void CooldownEnded(Cooldown cd)
    {
        if (cd.Type == GetType()) {
            Player.Instance.AddEffect(new Effect_Stealth(SourceOfEffect), FlatAmount);
            Player.Instance.AddCooldown(GetType(), Cooldown);
        }
    }
    
    public void EnterCombat(Unit u)
    {
        if (!Player.Instance.CheckIfEffectIsOnCooldown(typeof(Effect_GainStealthOnceEveryNSeconds))) {
            Player.Instance.AddEffect(new Effect_Stealth(SourceOfEffect), FlatAmount);
            Player.Instance.AddCooldown(GetType(), Cooldown);
        }
    }
}
