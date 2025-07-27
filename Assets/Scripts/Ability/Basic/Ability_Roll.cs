using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Roll : Ability_Dodge {
    private readonly float _jumpDistance = 0.5f;
    public float UntargetabilityDurationInSeconds = 0.75f;

    public Ability_Roll(Unit ability_user) : base(ability_user) {
        AddCustomSound("Jump", "Grunts/Male Jump 4", 1f);
        GameController.Instance.WaitAndRunMethod(1, ForceFinish);
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        User.PushInTargetDirection(Direction * 12 * _jumpDistance * (1 + User.MovementSpeed.Current / 100), this);
    }

    public override void CallAbilityEvent1()
    {
        Effect burn = User.GetEffect(typeof(Effect_Burn));
        if(burn != null) {
            burn.ChangeDecayingAmount(-burn.DecayingAmount / 3);
        }
        Effect freeze = User.GetEffect(typeof(Effect_Freeze));
        if(freeze != null) {
            freeze.ChangeDecayingAmount(-freeze.DecayingAmount / 5);
        }
        Effect incision = User.GetEffect(typeof(Effect_Incision));
        if(incision != null) {
            incision.ChangeDecayingAmount(incision.DecayingAmount / 10);
        }
        if (Direction == Vector2.right || Direction == Vector2.left) {
            Player.Instance.AddEffect(new Effect_RollForward(new(this)), UntargetabilityDurationInSeconds);
        }
        else {
            Player.Instance.AddEffect(new Effect_RollSideways(new(this)), UntargetabilityDurationInSeconds);
        }
    }

    public override void CallAbilityEvent2()
    {
        User.PushInTargetDirection(Direction * 2 * _jumpDistance * (1 + User.MovementSpeed.Current / 100), this);
    }

    public void ForceFinish() {
        if(User.Actions.CurrentAbilityBeingPerformed == this) {
            EndThisAbility();
        }
    }
}