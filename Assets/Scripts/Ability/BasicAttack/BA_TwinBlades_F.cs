using UnityEngine;

public class BA_TwinBlades_F : BasicAttack {

    public BA_TwinBlades_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 50, Constants.DamageType.Light));
        TransitionIntoAnimationDuration = 0f;
    }

    public override void CallAbilityEvent1() {

        if (HoldingMainButton && ButtonPressedCounter == 1) {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_S(User);
        }
        else if(ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FF(User);
            ((BA_TwinBlades_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_TwinBlades_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if(CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_TwinBlades_FF(User);
            ((BA_TwinBlades_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
            ((BA_TwinBlades_FF)User.Actions.CurrentAbilityBeingPerformed).ButtonPressedCounter = ButtonPressedCounter;
        }
    }
}