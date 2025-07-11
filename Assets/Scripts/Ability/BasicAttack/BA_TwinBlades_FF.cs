using UnityEngine;

public class BA_TwinBlades_FF : BasicAttack {

    public BA_TwinBlades_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 50, Constants.DamageType.Light));
    }

    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FS(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFF(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFF(User);
        }
    }
}