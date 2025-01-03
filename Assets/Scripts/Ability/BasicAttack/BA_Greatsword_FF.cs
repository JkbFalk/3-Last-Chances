using UnityEngine;

public class BA_Greatsword_FF : BasicAttack {

    public BA_Greatsword_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 160, Constants.DamageType.Heavy));

    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FS(User);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFF(User);
            ((BA_Greatsword_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFF(User);
            ((BA_Greatsword_FFF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
    }
}