using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAbility_FlameBackstab : Ability {
    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private bool _attackFromRightSide = true;
    public NPCAbility_FlameBackstab(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        AddCustomSound("Teleport", "Fire/FlameTeleport", 0.9f);
        Properties.AddRange(new List<Ability.AbilityProperty> {AbilityProperty.ImmuneToFlinch, AbilityProperty.CounteredByRoll, AbilityProperty.CounteredByRiposte, AbilityProperty.CountersBackstep});
        DamageSources.Add(new DamageSource(300, 300, Constants.DamageType.Heavy));
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
            User.transform.position = _attackFromRightSide ? User.CurrentTarget.transform.position + new Vector3(2.0f, 0) : User.CurrentTarget.transform.position + new Vector3(-2.0f, 0);
        }
    }

    public override void CallAbilityEvent2() {
        ChaseCurrentTargetAtGivenDegreeAngle(100, 20);
    }
}