using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_RunningAndSlashing : Ability {
    public static float Cooldown = 6;

    public NPCAbility_RunningAndSlashing(Unit ability_user) : base(ability_user) {
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
        DamageSources.Add(new DamageSource(100, 25, Constants.DamageType.Light) {KnockbackInMeters = 2f});
        DamageSources.Add(new DamageSource(100, 250, Constants.DamageType.Light, "2") {KnockbackInMeters = 7.5f});
    }

    public override void CallAbilityEvent1() {
        Vector2 direction_vector_towards_target = Utils.GetDirectionVector(User.transform.position, User.CurrentTarget != null ? User.CurrentTarget.transform.position : User.Actions.IsFlipped ? User.transform.position + Vector3.left : User.transform.position + Vector3.right, User.Actions.IsFlipped, 35);
        User.PushInTargetDirection(direction_vector_towards_target * 3, this);
    }

}