using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_HeelCleaver : Ability {

    public static float Cooldown = 15;

    public NPCAbility_HeelCleaver(Unit ability_user) : base(ability_user) {
        TransitionOutOfAnimationDuration = 0;
        TransitionIntoAnimationDuration = 0;
        DamageSources.Add(new DamageSource(250, 150, Constants.DamageType.Light));
        AddCustomSound("Swing", "Generic/Generic_Swoosh1", 1f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallBlunt;
        Properties.AddRange(new List<Ability.Property> {Property.CounteredByBackstep, Property.ImmuneToFlinch});
        WaitTimeBeforeNextAction = 0.3f;
    }
}