using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_QuickLeftAndRightDown : Ability {

    public bool IsRight = false;
    public static float Cooldown = 1;
    public NPCAbility_QuickLeftAndRightDown(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(130, 50, Constants.DamageType.Light));
        WaitTimeBeforeNextAction = 0.1f;
        IsRight = UnityEngine.Random.Range(0, 100) < 50;
        NameOfAnimationToAutoPlay = IsRight ? "QuickRightDown" : "QuickLeftDown";
        CanBeInterruptedByFlinching = false;
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(250, 10, 3);
    }

    public override void CallAbilityEvent2()
    {
        if(UnityEngine.Random.Range(0, 100) < 90) {
            if(User.CurrentTarget.transform.position.x > User.transform.position.x && User.Actions.IsFlipped) {
                User.Actions.IsFlipped = false;
            }
            else if(User.CurrentTarget.transform.position.x < User.transform.position.x && !User.Actions.IsFlipped) {
                User.Actions.IsFlipped = true;
            }
            if(IsRight) {
                IsRight = false;
                User.PlayAnimation("QuickLeftDown", 0, 0);
            }
            else {
                IsRight = true;
                User.PlayAnimation("QuickRightDown", 0, 0);
            }
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        User.AddEffect(new Effect_Acceleration(20, new(this)));
    }
}