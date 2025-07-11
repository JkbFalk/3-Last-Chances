using UnityEngine;

public class BA_Daggers_FF : BasicAttack {

    public BA_Daggers_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(125, 10, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS  && BA_Daggers_S.CheckIfAnyValidTargetInRange())
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFF(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFF(User);
        }
    }
}