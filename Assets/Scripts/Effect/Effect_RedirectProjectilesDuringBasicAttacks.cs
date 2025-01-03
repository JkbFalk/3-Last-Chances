using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Effect_RedirectProjectilesDuringBasicAttacks : Effect
{
    public Constants.DamageType DamageType;
    public float InjuryScaling = 0;
    public float StaggerScaling = 0;

    public override List<String> GetDescriptionParameters() {
        return new List<string> { Utils.GetFormattedFloat(Player.Instance.HeavyInjury.Current * InjuryScaling / 100), Utils.GetFormattedFloat(Player.Instance.HeavyStagger.Current * StaggerScaling / 100) };
    }

    public Effect_RedirectProjectilesDuringBasicAttacks(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }
}
