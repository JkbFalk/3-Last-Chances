using System.Linq;
using UnityEngine;

public class BA_Polearm_F : BasicAttack {

    public BA_Polearm_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(125, 25, Constants.DamageType.Heavy));
    }
    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ((ComboCounter == 1 && ReleasedMainButton == false) || (ComboCounter > 1 && ButtonHoldDuration > 0.3f)))
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (HoldingMainButton && ButtonHoldDuration > 0.3f)
        {
            User.Actions.CurrentAbilityBeingPerformed = BasicAttack.GetPolearmStrongAttack(ComboCounter);
        }
        else if (ButtonPressedCounter > 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FF(User) {HoldingMainButton = HoldingMainButton, ButtonPressedCounter = ButtonPressedCounter, ComboCounter = ComboCounter+1};
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
        ChaseCurrentTargetAtGivenDegreeAngle(70, 45, 10, target);
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Polearm_FF(User) {HoldingMainButton = HoldingMainButton, ButtonPressedCounter = ButtonPressedCounter, ComboCounter = ComboCounter+1};
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        Utils.PushUnitIntoPosition(damage.TargetOfDamage, User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2.5f, 0)) : (User.transform.position + new Vector3(2.5f, 0)), this, 50);
    }
}