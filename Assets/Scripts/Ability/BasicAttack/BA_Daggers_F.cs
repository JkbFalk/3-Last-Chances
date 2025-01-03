using UnityEngine;

public class BA_Daggers_F : BasicAttack {

    public BA_Daggers_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(125, 10, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 1 && BA_Daggers_S.CheckIfAnyValidTargetInRange())
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_S(User);
        }
        else if (ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FF(User);
            ((BA_Daggers_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Daggers_FF(User);
            ((BA_Daggers_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Daggers_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
    }
}