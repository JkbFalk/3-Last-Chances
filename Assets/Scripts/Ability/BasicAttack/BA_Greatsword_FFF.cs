using UnityEngine;

public class BA_Greatsword_FFF : BasicAttack {

    public BA_Greatsword_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 300, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS )
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFS(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFS(User);
        }
    }
}