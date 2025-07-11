using UnityEngine;

public class BA_TwinBlades_FFF : BasicAttack {

    public BA_TwinBlades_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(300 / 2, 200 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        PlayerControls.BasicAttackButtonPressCounter = 0;
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFS(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFS(User);
        }
    }
}