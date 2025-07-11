using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ChargedPunch : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Molis;
    public NPCAbility_ChargedPunch(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 600, Constants.DamageType.Light, "Stronger AoE") {KnockbackInMeters = 20f, CustomHitSound = "Earth/Earth_Punch1"});
        DamageSources.Add(new DamageSource(20, 300, Constants.DamageType.Light, "Weaker AoE") {KnockbackInMeters = 5f});
        AddCustomSound("Punch", "Explosion/Explosion3", 0.75f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallBlunt;
        Properties.AddRange(new List<Ability.Property> {Property.CounteredByRoll, Property.ImmuneToFlinch});
        WaitTimeBeforeNextAction = 0.4f;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}