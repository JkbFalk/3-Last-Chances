using UnityEngine;

public class NPCAbility_ArrowOnslaught : Ability {
    public static float Cooldown = 6;

    public NPCAbility_ArrowOnslaught(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(20, 120, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        User.Actions.DisableConsumable();
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bow_Release11", 0.6f);

    }

    public override void CallAbilityEvent1()
    {
        if(UnityEngine.Random.Range(0, 100) < 30) {
            User.PlayAnimation("ArrowOnslaught", 0.05f, 0.7f);
        }
    }
}