using UnityEngine;

public class BA_TwinBlades_FF : BasicAttack {

    public BA_TwinBlades_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 50, Constants.DamageType.Light));
    }

    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FS(User);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFF(User);
            ((BA_TwinBlades_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FFF(User);
            ((BA_TwinBlades_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
    }
}