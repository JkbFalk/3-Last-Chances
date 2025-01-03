using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCAbility_ClariseAssassination : Ability {

    public static float Cooldown = 25;

    public static AbilityFamily Family = AbilityFamily.Salutis;
    private bool _attackFromRightSide = true;
    public NPCAbility_ClariseAssassination(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(450, 250, Constants.DamageType.Heavy));
        AddCustomSound("Dash1", "Impact/Impact 3", 0.6f);
        AddCustomSound("Dash2", "Grunts/Female Jump 2", 1f);
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
    }

    public override void CallAbilityEvent1()
    {
        _attackFromRightSide = !User.CurrentTarget.Actions.IsFlipped;
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
        if(User.CurrentTarget == null) {
            GameController.Instance.WaitAndRunMethod(0.05f, CallAbilityEvent1);
        }
        else {
            User.Actions.IsFlipped = _attackFromRightSide;
            User.transform.position = _attackFromRightSide ? User.CurrentTarget.transform.position + new Vector3(3f, 0) : User.CurrentTarget.transform.position + new Vector3(-3f, 0);
        }
    }
}