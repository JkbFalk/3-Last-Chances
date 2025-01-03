using UnityEngine;

public class BA_TwinBlades_FFF : BasicAttack {

    public BA_TwinBlades_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(300 / 2, 200 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        ButtonPressedCounter = 0;
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton || ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFS(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFS(User);
        }
    }
}