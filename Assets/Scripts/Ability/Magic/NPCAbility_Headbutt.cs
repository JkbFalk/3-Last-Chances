using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCAbility_Headbutt : Ability {

    public static float Cooldown = 5;

    public NPCAbility_Headbutt(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing", "Generic/Generic_Swoosh4", 0.5f);
        DamageSources.Add(new DamageSource(50, 300, Constants.DamageType.Magic));
        WaitTimeBeforeNextAction = 1f;
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 1);
        User.UnitAI.PerformAnAttack();
    }
}