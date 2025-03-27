using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_BlockTechniquesFromGivenFamily : Effect {

    public Ability.AbilityFamily BlockedFamily;
    public float CooldownLength = 20;
    public List<Cooldown> CooldownsAdded = new();

    public Effect_BlockTechniquesFromGivenFamily(Ability.AbilityFamily blocked_family, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        BlockedFamily = blocked_family;
    }

    public override void OnStart()
    {
        base.OnStart();
        foreach(Type ability_type in SaveFile.Instance.UnlockedAbilities.Where(ab => Ability.GetFamily(ab) == BlockedFamily)) {
            if(TargetOfEffect.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == ability_type && cd.RemainingDuration > CooldownLength) == null) {
                Cooldown cd = new Cooldown(ability_type, CooldownLength, TargetOfEffect);
                CooldownsAdded.Add(cd);
                Player.Instance.AddCooldown(cd);
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        foreach(Cooldown cd in CooldownsAdded) {
            cd.RemainingDuration = 0;
        }
    }
}