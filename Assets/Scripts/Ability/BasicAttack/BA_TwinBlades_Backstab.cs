using System.Collections.Generic;

public class BA_TwinBlades_Backstab : Backstab {

    public BA_TwinBlades_Backstab(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Backstab);
        DamageSources.Add(new DamageSource(User.InCombat ? 200 / 2 : 400 / 2, User.InCombat ? 500 / 2 : 1000 / 2, Constants.DamageType.Light) {KnockbackInMeters = 1.5f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Light Right"].Bone.GetComponent<UnitWeapon>(), null);
    }
}