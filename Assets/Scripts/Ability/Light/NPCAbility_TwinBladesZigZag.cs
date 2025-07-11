using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_TwinBladesZigZag : Ability {
    public static float Cooldown = 6;
    private bool _startByGoingUp = false;

    public NPCAbility_TwinBladesZigZag(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing1", "TwinBlades/TwinBlades_Swing2", 0.9f);
        AddCustomSound("Swing2", "TwinBlades/TwinBlades_Swing3", 0.9f);
        AddCustomSound("Swing3", "TwinBlades/TwinBlades_Swing6", 0.9f);
        DamageSources.Add(new DamageSource(50 / 2, 350 / 2, Constants.DamageType.Light) {KnockbackInMeters = 2.5f});
        DamageSources.Add(new DamageSource(400 / 2, 0, Constants.DamageType.Light, "2") {KnockbackInMeters = 5f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }

    public override void CallAbilityEvent1()
    {
        if(User.CurrentTarget != null) {
            _startByGoingUp = User.CurrentTarget.transform.position.y > (User.transform.position.y - 1.5f);
            User.ApplyForce(new Vector2(400 * (User.Actions.IsFlipped ? -1 : 1), (User.IsBoss ? 1 : 0.4f) * 5 * (_startByGoingUp ? 1 : -1)), this);
        }
    }

    public override void CallAbilityEvent2()
    {
        User.ApplyForce(new Vector2(400 * (User.Actions.IsFlipped ? -1 : 1), (User.IsBoss ? 1 : 0.4f) * 7.5f * (_startByGoingUp ? -1 : 1)), this);
    }

    public override void CallAbilityEvent3()
    {
        User.ApplyForce(new Vector2(150 * (User.Actions.IsFlipped ? -1 : 1), (User.IsBoss ? 1 : 0.4f) * 3.5f * (_startByGoingUp ? 1 : -1)), this);
        ChaseCurrentTargetAtGivenDegreeAngle(3, 15);
    }
}