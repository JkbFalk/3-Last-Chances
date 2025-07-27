using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_DelayedJumpSlam : Ability {

    public static float Cooldown = 20;

    public NPCAbility_DelayedJumpSlam(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 100, Constants.DamageType.Light) {KnockbackInMeters = 2.5f});
        DamageSources.Add(new DamageSource(50, 500, Constants.DamageType.Light, "AoE"));
        AddCustomSound("Crack", "Earth/Earth_Crack3", 0.8f);
        Properties.AddRange(new List<Ability.Property> {Property.CounteredByRiposte, Property.ImmuneToFlinch});
        WaitTimeBeforeNextAction = 0.2f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        User.PushInTargetDirection(new Vector2(0.2f * (User.Actions.IsFlipped ? -1 : 1), 1).normalized * 5, this);
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(6, 45);
    }

    public override void CallAbilityEvent3()
    {
        Utils.CreateAreaOfEffect(new(this), "Ryker_JumpAssault" + (User.Actions.IsFlipped ? " Flipped" : ""));
    }

    public override void CallAbilityEvent4()
    {
        if(UnityEngine.Random.Range(0, 100) < 40 && User.gameObject.name != "Unit_TutorialRyker") {
            User.PlayAnimation("DelayedJumpSlam", 0, 0.47f);
        }
    }
}