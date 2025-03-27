using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_CannonsBlast : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Proprius;

    public NPCAbility_CannonsBlast(Unit ability_user) : base(ability_user) {
        AddCustomSound("Buildup", "Explosion/Buildup", 0.6f);
        AddCustomSound("Shot", "Explosion/Explosion6", 1.0f);
        DamageSources.Add(new DamageSource(250, 500, Constants.DamageType.Ranged, "Stronger AoE") {Knockback = 1250});
        DamageSources.Add(new DamageSource(100, 200, Constants.DamageType.Ranged, "Weaker AoE") {Knockback = 500});
        Properties.AddRange(new List<Ability.AbilityProperty> {AbilityProperty.Unstoppable, AbilityProperty.ImmuneToFlinch});
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this))};
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}
