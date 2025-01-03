using UnityEngine;

public class BA_Polearm_FF : BasicAttack {

    public BA_Polearm_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 50, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonHoldDuration > 0.3f)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FFF(User) {HoldingMainButton = HoldingMainButton, ButtonPressedCounter = ButtonPressedCounter, ComboCounter = ComboCounter+1};
        }
        CanFollowUpAttack = true;
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FFF(User) {HoldingMainButton = HoldingMainButton, ButtonPressedCounter = ButtonPressedCounter, ComboCounter = ComboCounter+1};
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        Utils.PushUnitIntoPosition(damage.TargetOfDamage, User.Actions.IsFlipped ? (User.transform.position + new Vector3(-3.5f, 0)) : (User.transform.position + new Vector3(3.5f, 0)), this, 50);
    }
}