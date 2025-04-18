using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FireAwakening : Ability {

    public static new bool DoesNotRequireTarget = true;
    public static float Cooldown = 30;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public NPCAbility_FireAwakening(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        DamageSources.Add(new DamageSource(100, 300, Constants.DamageType.Magic) {Knockback = 300});
        WaitTimeBeforeNextAction = 0.1f;
        AddCustomSound("Use", "Fire/Fire14", 0.55f);
        Properties.Add(AbilityProperty.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1() {
        User.AddEffect(new Effect_IgnisCaptainBuff(new(this)), 12);
        User.AddEffect(new Effect_Inflame(User.DamageType, new(this)), 12);
    }
}