using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FlameExplosion : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;

    public NPCAbility_FlameExplosion(Unit ability_user) : base(ability_user) {
        AddCustomSound("Buildup", "Fire/DynamiteIgnite", 0.6f);
        AddCustomSound("Shot", "Fire/FireExplosion1", 1.0f);
        DamageSources.Add(new DamageSource(350, 300, Constants.DamageType.Ranged, "Stronger AoE") {Knockback = 1250});
        DamageSources.Add(new DamageSource(200, 150, Constants.DamageType.Ranged, "Weaker AoE") {Knockback = 500});
        AbilityModifiers.Add(Constants.AbilityModifier.CountersRoll);
        AbilityModifiers.Add(Constants.AbilityModifier.CountersBackstep);
        AbilityModifiers.Add(Constants.AbilityModifier.CountersRiposte);
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        WaitTimeBeforeNextAction = 1f;
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        if(damage.DamagingObject.gameObject.name == "Stronger AoE") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(120 * User.MagicStagger.Current / 100, new(this)));
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(60 * User.MagicStagger.Current / 100, new(this)));
        }
    }
}