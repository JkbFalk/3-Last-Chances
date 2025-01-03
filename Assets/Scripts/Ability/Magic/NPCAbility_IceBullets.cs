using UnityEngine;

public class NPCAbility_IceBullets : Ability {
    public static float Cooldown = 4;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    public NPCAbility_IceBullets(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.25f;
        HitSoundVolume = 0.3f;
        DamageSources.Add(new DamageSource(50, 0, Constants.DamageType.Magic, "Criminal_FreezeCaster_IceShots_Basic"));
        DamageSources.Add(new DamageSource(50, 200, Constants.DamageType.Magic, "Criminal_FreezeCaster_IceShots_Final"));
        AddCustomSound("Use", "Ice/Ice_Use3", 0.2f);
        AddCustomSound("Use2", "Ice/Ice_Use4", 0.3f);
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void ExtraBehaviourOnDamage(Damage damage) {
        if (damage.DamagingObject.gameObject.name == "Criminal_FreezeCaster_IceShots_Basic") {
            damage.TargetOfDamage.AddEffect(new Effect_Slow(50, new(this)), 0.3f);
        }
        else if (damage.DamagingObject.gameObject.name == "Criminal_FreezeCaster_IceShots_Final") {
            damage.TargetOfDamage.AddEffect(new Effect_Freeze(50, new(this)), 1.5f);
        }
    }
}