using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_SurvivalModeStats : Effect
{
    public Effect_SurvivalModeStats(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Neutral;
        IsRemovable = false;
    }

    public override void OnStart() {
        base.OnStart();
        TargetOfEffect.Health.Base = TargetOfEffect.Health.Base * 0.7f;
        TargetOfEffect.StaggerBar.Base = TargetOfEffect.Health.Base * 0.7f;
        TargetOfEffect.HeavyInjury.Base = TargetOfEffect.HeavyInjury.Base * 1.3f;
        TargetOfEffect.LightInjury.Base = TargetOfEffect.LightInjury.Base * 1.3f;
        TargetOfEffect.RangedInjury.Base = TargetOfEffect.RangedInjury.Base * 1.3f;
        TargetOfEffect.MagicInjury.Base = TargetOfEffect.MagicInjury.Base * 1.3f;
        TargetOfEffect.HeavyStagger.Base = TargetOfEffect.HeavyStagger.Base * 1.3f;
        TargetOfEffect.LightStagger.Base = TargetOfEffect.LightStagger.Base * 1.3f;
        TargetOfEffect.RangedStagger.Base = TargetOfEffect.RangedStagger.Base * 1.3f;
        TargetOfEffect.MagicStagger.Base = TargetOfEffect.MagicStagger.Base * 1.3f;
    }

}
