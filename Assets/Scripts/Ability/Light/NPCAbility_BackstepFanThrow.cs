using UnityEngine;

public class NPCAbility_BackstepFanThrow : Ability {

    public static float Cooldown = 10;
    public NPCAbility_BackstepFanThrow(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(60, 0, Constants.DamageType.Light));
        AddCustomSound("Throw", "Blade/Blade_Swing6", 0.8f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallSharp;
        WaitTimeBeforeNextAction = 0.1f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1() {
        User.PushInTargetDirection(User.Actions.IsFlipped ? Vector2.right * 6 * (1 + User.MovementSpeed.Current / 100) : Vector2.left * 6 * (1 + User.MovementSpeed.Current / 100), this);
    }

    public override void CallAbilityEvent2() {
        for (int i = 0; i < 5; i++) {
            Projectile dagger = Utils.CreateProjectile(new(this), "Criminal_Daggers_BasicAttack2");
            dagger.transform.Rotate(new Vector3(0, 0, -20 + 10 * i));
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage) {
        ResetPotentialTargets();
    }
}