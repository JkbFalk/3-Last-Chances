using UnityEngine;

public class BA_Greatsword_F : BasicAttack {

    public BA_Greatsword_F(Unit ability_user) : base(ability_user) {
        PowerBudget = 160;
        DamageSources.Add(new DamageSource(80, 80, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS )
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FF(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FF(User);
        }
    }
}