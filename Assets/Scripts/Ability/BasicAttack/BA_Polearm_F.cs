using System.Linq;
using UnityEngine;

public class BA_Polearm_F : BasicAttack {

    public BA_Polearm_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(125, 25, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FF(User) {ComboCounter = ComboCounter+1};
        }
        CanFollowUpAttack = true;
    }

    public override void CallAbilityEvent3()
    {
        Unit target = User.CurrentTarget;
        if(target == null) {
            Unit u = Utils.GetAllUnits(true, true).FirstOrDefault(unit => Vector2.Distance(unit.transform.position, User.transform.position) < 6 && ((User.Actions.IsFlipped == false && unit.transform.position.x > User.transform.position.x) || (User.Actions.IsFlipped && unit.transform.position.x < User.transform.position.x)));
            target = u;
        }
        if(target == null || (User.Actions.IsFlipped == false && target.transform.position.x < User.transform.position.x) || (User.Actions.IsFlipped && target.transform.position.x > User.transform.position.x)) {
            return;
        }
        ChaseCurrentTargetAtGivenDegreeAngle(3, 45, target);
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FF(User) {ComboCounter = ComboCounter+1};
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.PushIntoPosition(User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2.5f, 0)) : (User.transform.position + new Vector3(2.5f, 0)), this, 1.25f);
    }
}