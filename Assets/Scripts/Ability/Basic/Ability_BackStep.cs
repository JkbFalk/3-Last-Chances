using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability_BackStep : Ability_Dodge {
    private readonly int _jumpDistance = 90;
    private int _flipped;
    private float _movementSpeed;

    public Ability_BackStep(Unit ability_user) : base(ability_user) {
        AddCustomSound("Jump", "Grunts/Male Jump 2", 1f);
        GameController.Instance.WaitAndRunMethod(1, ForceFinish);
    }

    public override void CallAbilityEvent1() {
        Player.Instance.AddEffect(new Effect_Backstep(new(this)), 0.5f);
        _flipped = User.Actions.IsFlipped ? -1 : 1;
        _movementSpeed = User.MovementSpeed.Current;
        Player.Instance.ApplyForce(new Vector2(-0.8f * _flipped, 0.8f) * 3 * _jumpDistance * _movementSpeed, this);
        GameController.Instance.WaitAndRunMethod(0.02f, Angle2);
        GameController.Instance.WaitAndRunMethod(0.04f, Angle3);
        GameController.Instance.WaitAndRunMethod(0.06f, Angle4);
        GameController.Instance.WaitAndRunMethod(0.08f, Angle5);
    }

    public void Angle2() {
        Player.Instance.ApplyForce(new Vector2(-0.7f * _flipped, 0.5f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle3() {
        Player.Instance.ApplyForce(new Vector2(-1f * _flipped, 0) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle4() {
        Player.Instance.ApplyForce(new Vector2(-0.7f * _flipped, -0.8f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void Angle5() {
        Player.Instance.ApplyForce(new Vector2(-0.6f * _flipped, -1f) * 2 * _jumpDistance * _movementSpeed, this);
    }

    public void ForceFinish() {
        if(User.Actions.CurrentAbilityBeingPerformed == this) {
            EndThisAbility();
        }
    }
}