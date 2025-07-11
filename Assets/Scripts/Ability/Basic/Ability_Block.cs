using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Block : Ability {

    public bool CanStopBlocking = false;

    public Ability_Block(Unit ability_user) : base(ability_user) {
        TransitionIntoAnimationDuration = 0.05f;
        AutoPlayAbilityAnimation = false;
        CanMoveWhileUsing = false;
        Player.Instance.IsPerfectlyBlocking = true;
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Block(new(this)) };
        GameController.Instance.WaitAndRunMethod(0.5f / Player.Instance.PerfectBlockSpeed, CallAbilityEvent1);
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        User.PlayAnimation(User.CurrentWeaponClass + "_PerfectBlock", TransitionIntoAnimationDuration);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Player.Instance.IsPerfectlyBlocking = false;
        Player.Instance.PerfectBlockSpeed += 0.5f;
    }

    public override void CallAbilityEvent1()
    {
        if(Player.Instance.Actions.CurrentAbilityBeingPerformed == this) {
            CanMoveWhileUsing = true;
            Player.Instance.IsPerfectlyBlocking = false;
            GameController.Instance.WaitAndRunMethod(0.5f - 0.5f / Player.Instance.PerfectBlockSpeed, CanReleaseBlock);
        }
    }

    public override void OnBlockButtonRelease()
    {
        if(CanStopBlocking) {
            EndThisAbility();
        }
    }

    public void CanReleaseBlock() {
        if(PlayerControls.BlockButtonHoldDuration == 0) {
            EndThisAbility();
        }
        else {
            CanStopBlocking = true;
            CanInterruptCurrentAbility = true;
        }
    }
}