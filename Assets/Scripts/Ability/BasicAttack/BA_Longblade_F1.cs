using UnityEditor;
using UnityEngine;

public class BA_Longblade_F1 : BasicAttack {

    private bool _charging = false;
    public bool FollowUpAttacking = false;
    public BA_Longblade_F1(Unit ability_user) : base(ability_user)
    {
        PowerBudget = 160;
        TransitionIntoAnimationDuration = 0.02f;
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration == 0)
        {
            User.PlayAnimation("BA_Longblade_F1", 0.03f, 0.58f);
            DamageSources.Add(new DamageSource(40, 120, Constants.DamageType.Heavy));
            User.Actions.SetFaceVariant("Eyes Squinted");
        }
        else
        {
            _charging = true;
        }
    }

    public override void CallAbilityEvent2()
    {
        Properties.Add(Property.StrongBasicAttack);
        Utils.PlaySoundEffect(User.AudioSource, "Ability/QuickdrawIndicator", 0.45f);
    }

    public override void CallAbilityEvent3()
    {
        DamageSources.Add(new DamageSource(40 * 4, 120 * 4, Constants.DamageType.Heavy));
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
        Debug.Log($"LONGBLADE1 {IsNot(Property.StrongBasicAttack)}");
        if (IsNot(Property.StrongBasicAttack) && PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            Debug.Log($"LONGBLADE2");
            FollowUpAttacking = true;
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
        else if (IsNot(Property.StrongBasicAttack))
        {
            Debug.Log($"LONGBLADE3");
            CanFollowUpAttack = true;
        }
    }

    public override void CallAbilityEvent5() {
        if(IsNot(Property.StrongBasicAttack) && !FollowUpAttacking) {
            EndThisAbility();
        }
    }

    public override void CallAbilityEvent6()
    {
        Debug.Log("ZYZY1: " + User.Animator.IsInTransition(0));
        if (User.Animator.IsInTransition(0) == false)
        {
            Debug.Log("ZYZY2");
            EndThisAbility();
        }
    }

    public override void OnBasicAttackButtonRelease()
    {
        base.OnBasicAttackButtonRelease();
        if (!_charging)
        {
            return;
        }
        float powerMultiplier = Utils.GetValueBasedOnMinAndMax(User.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.045f, 0.57f, 1, 4);
        DamageSources.Add(new DamageSource(40 * powerMultiplier, 120 * powerMultiplier, Constants.DamageType.Heavy));
        User.PlayAnimation("BA_Longblade_F1", 0.03f, 0.58f);
        _charging = false;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        Debug.Log("BUTTON PRESSED2: " + CanFollowUpAttack);
        if (CanFollowUpAttack)
        {
            FollowUpAttacking = true;
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
    }
}