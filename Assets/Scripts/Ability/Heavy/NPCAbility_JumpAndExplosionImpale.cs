using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_JumpAndExplosionImpale : Ability {
    public static float Cooldown = 6;
    private AreaOfEffect _aoe;

    public NPCAbility_JumpAndExplosionImpale(Unit ability_user) : base(ability_user) {
        AddCustomSound("Jump", "Explosion/Ground Explosion", 0.6f);
        AddCustomSound("Hit", "Explosion/Explosion1", 0.8f);
        DamageSources.Add(new DamageSource(100, 600, Constants.DamageType.Heavy) {Knockback = 600});
        DamageSources.Add(new DamageSource(75, 450, Constants.DamageType.Heavy, "Main AoE") {Knockback = 600});
        DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Heavy, "Weaker AoE"));
        AbilityModifiers.AddRange(new List<Constants.AbilityModifier> { Constants.AbilityModifier.CounteredByRiposte });
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "SpearStrikeTheGround", User.CurrentTarget.transform.position.x, User.CurrentTarget.transform.position.y - 0.5f);
    }

    public override void CallAbilityEvent2()
    {
        User.transform.position = new Vector2(_aoe.transform.position.x, _aoe.transform.position.y + 0.8f);
    }

    public override void CallAbilityEvent3()
    {
        AbilityModifiers.Clear();
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        if(damage.DamagingObject.gameObject.name.Contains("Weaker AoE") == false) {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 3);
        }
    }
}