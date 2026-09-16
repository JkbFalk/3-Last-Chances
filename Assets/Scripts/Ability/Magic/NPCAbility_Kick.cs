using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCAbility_Kick : Ability {
    public static float Cooldown = 6;
    public NPCAbility_Kick(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing", "Generic/Generic_Swoosh2", 0.5f);
        DamageSources.Add(new DamageSource(150, 150, Constants.DamageType.Magic));
        WaitTimeBeforeNextAction = 1f;
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 0.5f);
        damage.TargetOfDamage.AddEffect(new Effect_Onslaught(50, new(this)), 5);
        User.UnitAI.PerformAnAttack();
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(3, 15);
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "SmallCircleAoE");
        aoe.AddComponent<AttachObjectToBodyPart>();
        aoe.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Left Foot";
        aoe.GetComponent<AttachObjectToBodyPart>().AttachToBone = true;
        aoe.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        aoe.GetComponent<DestroyGameObjectAfterGivenTime>().DestroyAfterSeconds = 0.3f;
    }
}