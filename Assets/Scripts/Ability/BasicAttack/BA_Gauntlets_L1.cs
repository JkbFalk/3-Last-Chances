using UnityEngine;

public class BA_Gauntlets_L1 : BasicAttack {

    public BA_Gauntlets_L1(Unit ability_user) : base(ability_user) {
        TransitionIntoAnimationDuration = 0.02f;
        PowerBudget = 160;
        DamageSources.Add(new DamageSource(20, 0, Constants.DamageType.Light));
    }

    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > 0.2f)
        {
            Player.Instance.PerformGauntletStrongBasicAttack();
            
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            Player.Instance.PerformGauntletBasicAttack();
        }
        CanFollowUpAttack = true;
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(1, 30);
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            Player.Instance.PerformGauntletBasicAttack();
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        damage.TargetOfDamage.AddEffect(new Effect_ProneToKnockout(Constants.PRONE_TO_KNOCKOUT_AMOUNT_ADDED_BY_GAUNTLET_BA, new(this)));
    }
}