using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_PlundererOmni : Ability {
    public NPCAbility_PlundererOmni(Unit ability_user) : base(ability_user) {
        HitSoundType = Constants.HitSoundTypeEnum.Magic;
        DamageSources.Add(new DamageSource(250, 300, Constants.DamageType.Heavy) {Knockback = 1500});
        WaitTimeBeforeNextAction = 0.1f;
        AddCustomSound("Use", "Impact/Impact 11", 0.4f);
        CanBeInterruptedByFlinching = false;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_RootedInPlace(new(this)) };
        AbilityModifiers.Add(Constants.AbilityModifier.CountersRoll);
        AbilityModifiers.Add(Constants.AbilityModifier.CountersBackstep);
        AbilityModifiers.Add(Constants.AbilityModifier.CountersBlock);
        AbilityModifiers.Add(Constants.AbilityModifier.CountersRiposte);
        TransitionIntoAnimationDuration = 0;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void CallAbilityEvent1()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "PlundererOmni");
        proj.CleanUpAfter(5);
        proj.transform.localEulerAngles = new Vector3(0, User.Actions.IsFlipped ? 180 : 0, -90);
        PlayCustomSound("Use");
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_Void(20, new(this)));
    }
}