using System.Collections.Generic;

public class BA_Daggers_Backstab : Backstab {

    public BA_Daggers_Backstab(Unit ability_user) : base(ability_user) {
        IsBackstab = true;
        DamageSources.Add(new DamageSource(User.InCombat ? 300 / 2 : 600 / 2, User.InCombat ? 750 / 2 : 1500 / 2, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        CustomHitSound = "Ability/Ability_SeverVitality";
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Light Right"].Bone.GetComponent<UnitWeapon>(), null);
    }
}