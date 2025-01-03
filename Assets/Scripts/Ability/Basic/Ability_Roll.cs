using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Roll : Ability_Dodge {
    private readonly int _jumpDistance = 50;
    public float UntargetabilityDurationInSeconds = 0.75f;

    public Ability_Roll(Unit ability_user) : base(ability_user) {
        AddCustomSound("Jump", "Grunts/Male Jump 4", 1f);
        GameController.Instance.WaitAndRunMethod(1, ForceFinish);
    }

    public override void OnAbilityStart() {
        base.OnAbilityStart();
        User.ApplyForce(Direction * 12 * _jumpDistance * User.MovementSpeed.Current, this);
    }

    public override void CallAbilityEvent1()
    {
        Effect burn = User.GetEffect(typeof(Effect_Burn));
        if(burn != null) {
            burn.AddDecayingAmount(-burn.DecayingAmount / 2);
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
        User.ApplyForce(Direction * 2 * _jumpDistance * User.MovementSpeed.Current, this);
    }

    public void ForceFinish() {
        if(User.Actions.CurrentAbilityBeingPerformed == this) {
            EndThisAbility();
        }
    }
}