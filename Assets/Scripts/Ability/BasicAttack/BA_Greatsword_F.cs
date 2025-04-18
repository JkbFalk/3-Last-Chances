using UnityEngine;

public class BA_Greatsword_F : BasicAttack {

    public BA_Greatsword_F(Unit ability_user) : base(ability_user) {
        PowerBudget = 160;
        DamageSources.Add(new DamageSource(80, 80, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_S(User);
        }
        else if (ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FF(User);
            ((BA_Greatsword_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Greatsword_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FF(User);
            ((BA_Greatsword_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_Greatsword_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
    }
}