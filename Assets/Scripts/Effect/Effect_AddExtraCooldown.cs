using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_AddExtraCooldown : Effect
{
    public float CooldownIncrease;
    public Type AbilityItemOrEffectCooldown;

    public Effect_AddExtraCooldown(Type cooldown, float cooldown_increase, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        AbilityItemOrEffectCooldown = cooldown;
        CooldownIncrease = cooldown_increase;
        Type = EffectType.Neutral;
    }
}
