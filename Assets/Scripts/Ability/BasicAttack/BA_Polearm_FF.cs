using UnityEngine;

public class BA_Polearm_FF : BasicAttack {

    public BA_Polearm_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 50, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FFF(User) { ComboCounter = ComboCounter+1};
        }
        CanFollowUpAttack = true;
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FFF(User) { ComboCounter = ComboCounter+1};
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.PushIntoPosition(User.Actions.IsFlipped ? (User.transform.position + new Vector3(-3.5f, 0)) : (User.transform.position + new Vector3(3.5f, 0)), this, 1.25f);
    }
}