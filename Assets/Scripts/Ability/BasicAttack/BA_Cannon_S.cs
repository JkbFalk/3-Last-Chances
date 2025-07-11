using UnityEngine;

public class BA_Cannon_S : BasicAttack
{

    public static int AmmoRequiredToUseAbility = 1;
    private bool _charging = false;
    public bool FollowUpAttacking = false;
    public BA_Cannon_S(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("PreSwing", "Cannon/Cannon_Preswing", 1f);
        AddCustomSound("Swing", "Cannon/Cannon_Swing", 1f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        DamageSources.Add(new DamageSource(0, 50, Constants.DamageType.Ranged) {KnockbackInMeters = 1.5f});
        PowerBudget = 200;
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(5, 50);
    }

    public override void CallAbilityEvent2()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration < Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            DamageSources.Add(new DamageSource(40, 120, Constants.DamageType.Ranged, "CannonBasicAttack"));
            User.PlayAnimation("BA_Cannon_S", 0.03f, 0.72f);
        }
        else
        {
            _charging = true;
        }
    }

    public override void CallAbilityEvent3()
    {
        DamageSources.Add(new DamageSource(40 * 4, 120 * 4, Constants.DamageType.Ranged));
        _charging = false;
        User.Actions.SetFaceVariant("Regular");
        if (IsNot(Property.StrongBasicAttack))
        {
            User.Actions.PlaySwingSound();
            ChaseCurrentTargetAtGivenDegreeAngle(1.5f, 45);
        }
        else
        {
            User.Actions.PlayHeavySwingSound();
            ChaseCurrentTargetAtGivenDegreeAngle(4, 45);
        }
    }

    public override void CallAbilityEvent4()
    {
        if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            FollowUpAttacking = true;
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
        else
        {
            CanFollowUpAttack = true;
        }
    }

    public override void OnBasicAttackButtonRelease()
    {
        base.OnBasicAttackButtonRelease();
        if (!_charging)
        {
            return;
        }
        float powerMultiplier = Utils.GetValueBasedOnMinAndMax(User.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.18f, 0.72f, 1, 4);
        DamageSources.Add(new DamageSource(40 * powerMultiplier, 120 * powerMultiplier, Constants.DamageType.Ranged));
        User.PlayAnimation("BA_Cannon_S", 0.03f, 0.72f);
        _charging = false;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            FollowUpAttacking = true;
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
    }
    
    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Cannon/Cannon_BasicAttack" + Utils.GetRandomSoundNumber("Cannon_BasicAttack"), 0.6f);
    }
}