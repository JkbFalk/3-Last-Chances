using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ShieldSpike : Ability {

    public static float Cooldown = 15;
    public NPCAbility_ShieldSpike(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(500, 0, Constants.DamageType.Heavy, "AoE Stronger"));
        DamageSources.Add(new DamageSource(75, 250, Constants.DamageType.Heavy, "AoE Weaker") {Knockback = 850});
        WaitTimeBeforeNextAction = 0.5f;
        AddCustomSound("OnUse","Heavy Object/HeavyObject_Slam1", 0.85f);
        AbilityModifiers.AddRange(new List<Constants.AbilityModifier> { Constants.AbilityModifier.CounteredByRiposte });
        CanBeInterruptedByFlinching = false;
    }

    public override void CallAbilityEvent1()
    {
        if(Vector2.Distance(Player.Instance.transform.position, User.transform.position) < 10)
        {
            float magnitude = 1 / Vector2.Distance(Player.Instance.transform.position, User.transform.position);
            if(magnitude > 1)
            {
                magnitude = 1;
            }
            CameraController.Instance.ShakeScreen(0.2f, magnitude);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        if(damage.AbilityDamageSource.ColliderName == "AoE Weaker") {
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        }
    }
}