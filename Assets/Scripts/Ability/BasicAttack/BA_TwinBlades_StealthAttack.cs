using System.Collections.Generic;

public class BA_TwinBlades_StealthAttack : StealthAttack {

    public BA_TwinBlades_StealthAttack(Unit ability_user) : base(ability_user) {
        IsStealthAttack = true;
        DamageSources.Add(new DamageSource(User.InCombat ? 200 / 2 : 400 / 2, User.InCombat ? 500 / 2 : 1000 / 2, Constants.DamageType.Light) {Knockback = 150});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Light Right"].Bone.GetComponent<UnitWeapon>(), null);
    }
}