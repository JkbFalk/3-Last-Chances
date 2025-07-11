using UnityEngine;

public class BA_Daggers_FFFF : BasicAttack {

    public BA_Daggers_FFFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 50, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        Unit closestTarget = User.GetClosestValidTarget(true);
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS  && BA_Daggers_S.CheckIfAnyValidTargetInRange())
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 4 && closestTarget != null && Vector2.Distance(closestTarget.transform.position, User.transform.position) < 3)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFFFF(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        Unit closestTarget = User.GetClosestValidTarget(true);
        if (CanFollowUpAttack && closestTarget != null && Vector2.Distance(closestTarget.transform.position, User.transform.position) < 3)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFFFF(User);
        }
    }
}