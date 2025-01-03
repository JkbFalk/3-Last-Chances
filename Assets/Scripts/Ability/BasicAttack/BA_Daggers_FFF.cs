using UnityEngine;

public class BA_Daggers_FFF : BasicAttack {

    public BA_Daggers_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 50, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 3 && BA_Daggers_S.CheckIfAnyValidTargetInRange())
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_S(User);
        }
        else if (ButtonPressedCounter > 3)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFFF(User);
            ((BA_Daggers_FFFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FFFF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFFF(User);
            ((BA_Daggers_FFFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FFFF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
    }
}