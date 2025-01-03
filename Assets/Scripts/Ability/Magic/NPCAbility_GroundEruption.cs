using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_GroundEruption : Ability {

    public static float Cooldown = 30;
    public static AbilityFamily Family = AbilityFamily.Molis;

    private float _stunDuration = 2f;
    public NPCAbility_GroundEruption(Unit ability_user) : base(ability_user) {
        TransitionOutOfAnimationDuration = 0.05f;
        DamageSources.Add(new DamageSource(80, 150, Constants.DamageType.Magic, "AoE 1") {Knockback = 75});
        DamageSources.Add(new DamageSource(0, 400, Constants.DamageType.Magic, "AoE 2") {Knockback = 75});
        DamageSources.Add(new DamageSource(250, 0, Constants.DamageType.Magic, "AoE 3") {Knockback = 75});
        AddCustomSound("Explosion1", "Explosion/Explosion4", 0.5f);
        AddCustomSound("Explosion2", "Explosion/Explosion5", 0.5f);
        AddCustomSound("Explosion3", "Explosion/Explosion6", 0.5f);
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        WaitTimeBeforeNextAction = 0.2f;
        
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this))};
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        CanBeInterruptedByFlinching = false;
    }

    public override void CallAbilityEvent1()
    {
        _stunDuration = 0;
    }

    public override void CallAbilityEvent2()
    {
        _stunDuration = 4;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        if(damage.InjuryDealt > 0 && _stunDuration > 0)
        {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), _stunDuration);
        }
    }
}