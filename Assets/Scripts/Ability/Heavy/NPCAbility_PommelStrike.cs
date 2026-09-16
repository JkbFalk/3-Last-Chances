using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_PommelStrike : Ability {
    public static float Cooldown = 6;

    public NPCAbility_PommelStrike(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0f;
        DamageSources.Add(new DamageSource(50, 300, Constants.DamageType.Heavy));
        Properties.Add(Property.CounteredByRoll);
        Properties.Add(Property.CounteredByBlock);
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 0.8f);
        User.UnitAI.PerformAnAttack();
    }
}