using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_JumpSlam : Ability {

    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Molis;
    public NPCAbility_JumpSlam(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 500, Constants.DamageType.Heavy, "AoE Stronger"));
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy, "AoE Weaker") {Knockback = 250});
        DamageSources.Add(new DamageSource(150, 150, Constants.DamageType.Heavy, "Spike AoE") {Knockback = 1100});
        WaitTimeBeforeNextAction = 0.5f;
        AddCustomSound("OnUse", "Heavy Object/HeavyObject_Slam1", 0.85f);
        Properties.Add(AbilityProperty.CounteredByRiposte);
        Properties.Add(AbilityProperty.ImmuneToFlinch);
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(100, 60, 5);
        User.ApplyForce(new Vector2(0.2f, 1).normalized * 300, this);
    }

    public override void CallAbilityEvent2()
    {
        if (Vector2.Distance(Player.Instance.transform.position, User.transform.position) < 10)
        {
            float magnitude = 1.4f / Vector2.Distance(Player.Instance.transform.position, User.transform.position);
            if (magnitude > 1)
            {
                magnitude = 1.4f;
            }
            CameraController.Instance.ShakeScreen(0.2f, magnitude);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        if(damage.AbilityDamageSource.ColliderName == "Spike AoE") {
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        }
    }
}