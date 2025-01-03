using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_PlundererGlacies : Ability {

    public static AbilityFamily Family = AbilityFamily.Glacies;   
    public NPCAbility_PlundererGlacies(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        DamageSources.Add(new DamageSource(80, 80, Constants.DamageType.Heavy) {Knockback = 100});
        WaitTimeBeforeNextAction = 0.1f;
        AddCustomSound("Shoot", "Ice/Ice_Shot1", 0.4f);
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
        TransitionIntoAnimationDuration = 0;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent1()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "PlundererGlacies");
        proj.CleanUpAfter(5);
        proj.transform.up = Utils.GetDirectionVector(proj.transform.position, Target.transform.position, User.Actions.IsFlipped, 30);
        PlayCustomSound("Shoot");
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_Slow(User.MagicStagger.Current, new(this)));
    }
}