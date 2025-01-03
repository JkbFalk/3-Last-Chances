using UnityEngine;

public class BA_Daggers_FF : BasicAttack {

    public BA_Daggers_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(125, 10, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 2 && BA_Daggers_S.CheckIfAnyValidTargetInRange())
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_S(User);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFF(User);
            ((BA_Daggers_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FFF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FFF(User);
            ((BA_Daggers_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FFF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
    }
}