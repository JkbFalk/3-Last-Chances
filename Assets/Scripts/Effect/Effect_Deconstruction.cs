using System;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Deconstruction : Effect
{
    public bool UpgradeA = false;
    public bool UpgradeB = false;
    public int Stacks = 2;
    public Type DeconstructionTarget;
    public static List<Type> UniqueDeconstructions = new List<Type>();
    public Effect_Deconstruction(Type deconstruction_target, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        PathToUIGraphic = "Ability/Deconstruction";
        ShowsInUI = true;
        DeconstructionTarget = deconstruction_target;
        Listeners.Add(EventManager.HitDealt);
        Listeners.Add(EventManager.DamageDealt);
        Listeners.Add(EventManager.AbilityEnded);
        Listeners.Add(EventManager.EnterCombat);
    }

    public override void OnStart()
    {
        base.OnStart();
        UIText = "x" + Stacks.ToString();
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        base.OnInvokeHitDealt(damage);
        if (UpgradeB && damage.TargetOfDamage == TargetOfEffect && damage.SourceOfDamage.GetType() == DeconstructionTarget)
        {
            damage.ArmorModifier += Stacks * Ability_Deconstruction.UpgradeBArmorPerStack;
            return;
        }
        else if (damage.SourceOfDamage.User != TargetOfEffect || (damage.SourceOfDamage.IsNot(Ability.Property.Riposte) && damage.SourceOfDamage.IsNot(Ability.Property.Counter)) || damage.SourceOfDamage?.OriginalRipostedAbility?.GetType() != DeconstructionTarget)
        {
            return;
        }
        damage.DamageDealtMultiplier *= Stacks;
    }

    public override void OnInvokeAbilityEnded(Ability ability)
    {
        base.OnInvokeAbilityEnded(ability);
        if (ability.User == Player.Instance && (ability.Is(Ability.Property.Riposte) || ability.Is(Ability.Property.Counter)) && ability.OriginalRipostedAbility?.GetType() == DeconstructionTarget)
        {
            IncrementStacks(ability.OriginalRipostedAbility);
        }
    }

    public override void OnInvokeEnterCombat(Unit unit)
    {
        base.OnInvokeEnterCombat(unit);
        UniqueDeconstructions.Clear();
    }

    public void IncrementStacks(Ability target_ability)
    {
        int max = UpgradeA ? Ability_Deconstruction.UpgradeAMaxStacks : Ability_Deconstruction.MaxStacks;
        if (UpgradeA && target_ability.Is(Ability.Property.Unstoppable))
        {
            Stacks = max;
        }
        else
        {
            Stacks = Stacks == max ? max : Stacks + 1;
        }
        UIText = "x" + Stacks.ToString();
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Ability_Deconstruction" + (Stacks > (UpgradeA ? 6 : 4) ? "3" : "2"));
    }
}