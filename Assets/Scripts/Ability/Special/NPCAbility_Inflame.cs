using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_Inflame : Ability {

    public static new bool DoesNotRequireTarget = true;
    public static float Cooldown = 30;
    public static AbilityFamily Family = AbilityFamily.Ignis;

    public NPCAbility_Inflame(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/Inflame", 0.65f);
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 0.1f;
        Properties.Add(AbilityProperty.ImmuneToFlinch);

        NameOfAnimationToAutoPlay = User.DamageCategory == Constants.DamageType.Heavy ? "Inflame_Heavy" : "Inflame_Light";
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        User.AddEffect(new Effect_Inflame(User.DamageCategory, new(this)), 25);
    }
}