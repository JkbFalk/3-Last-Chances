using UnityEngine;

public class BA_Polearm_FFF : BasicAttack {

    public BA_Polearm_FFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(175, 100, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_F(User) { ComboCounter = ComboCounter+1};
        }
        CanFollowUpAttack = true;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        Utils.PushUnitIntoPosition(damage.TargetOfDamage, User.Actions.IsFlipped ? (User.transform.position + new Vector3(-4.5f, 0)) : (User.transform.position + new Vector3(4.5f, 0)), this, 50);
    }
}