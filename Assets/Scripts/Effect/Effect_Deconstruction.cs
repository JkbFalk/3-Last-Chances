using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Deconstruction : Effect
{
    public bool MasteryB = false;
    public int Stacks = 1;
    public Type DeconstructionTarget;
    public Effect_Deconstruction(Type deconstruction_target, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        PathToUIGraphic = "Ability/Deconstruction";
        ShowsInUI = true;
        DeconstructionTarget = deconstruction_target;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnStart()
    {
        base.OnStart();
        UIText = "1";
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        base.OnInvokeHitDealt(damage);
        if (MasteryB && damage.TargetOfDamage == TargetOfEffect && damage.SourceOfDamage.GetType() == DeconstructionTarget)
        {
            damage.ArmorModifier += Stacks * Ability_Deconstruction.UpgradeBArmorPerStack;
        }
        else if (damage.SourceOfDamage.User != TargetOfEffect || (damage.SourceOfDamage.IsNot(Ability.Property.Riposte) && damage.SourceOfDamage.IsNot(Ability.Property.Counter)) || damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed?.GetType() != DeconstructionTarget)
        {
            return;
        }
        damage.DamageDealtMultiplier *= Stacks;
    }
}