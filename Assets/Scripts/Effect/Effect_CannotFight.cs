using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_CannotFight : Effect_HardCrowdControl
{
    public Effect_CannotFight(SourceOfEffect source_of_effect) : base(source_of_effect) {
        NameOfAnimationToAutoPlay = "HeavilyWounded";
        AdditionalEffectsAffectingTargetDuringEffect = new System.Collections.Generic.List<Effect> { new Effect_RootedInPlace(source_of_effect), new Effect_Invincible(SourceOfEffect) };
    }

    public override void OnStart()
    {
        base.OnStart();
        TargetOfEffect.InCombat = false;
    }
}
