using UnityEngine;

public class BA_Greatsword_FFF : BasicAttack {

    public BA_Greatsword_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 300, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton || ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFS(User);
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Greatsword_FFS(User);
        }
    }
}