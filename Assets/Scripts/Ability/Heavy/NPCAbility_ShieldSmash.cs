using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ShieldSmash : Ability {
    public static float Cooldown = 8;

    public NPCAbility_ShieldSmash(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.05f;
        DamageSources.Add(new DamageSource(50, 400, Constants.DamageType.Heavy));
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        User.UnitAI.PerformAnAttack();
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 0.5f);
    }
}