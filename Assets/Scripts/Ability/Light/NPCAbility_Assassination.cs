using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_Assassination : Ability {

    public static float Cooldown = 12;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    private bool _attackFromRightSide = true;
    public NPCAbility_Assassination(Unit ability_user) : base(ability_user) {
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        DamageSources.Add(new DamageSource(350 / 2, 0, Constants.DamageType.Light));
        Properties.Add(AbilityProperty.CounteredByBlock);
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
        _attackFromRightSide = User.CurrentTarget.Actions.IsFlipped;
    }

    public override void CallAbilityEvent1() {
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
        if(User.CurrentTarget == null) {
            GameController.Instance.WaitAndRunMethod(0.05f, CallAbilityEvent1);
        }
        else {
            User.Actions.IsFlipped = _attackFromRightSide;
            User.transform.position = _attackFromRightSide ? User.CurrentTarget.transform.position + new Vector3(1.0f, 0) : User.CurrentTarget.transform.position + new Vector3(-1.0f, 0);
        }
    }
}