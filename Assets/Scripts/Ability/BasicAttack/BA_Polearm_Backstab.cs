using UnityEngine;

public class BA_Polearm_Backstab : Backstab {

    public BA_Polearm_Backstab(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Backstab);
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Heavy) {KnockbackInMeters = 1.5f});
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Heavy"].Bone.GetComponent<UnitWeapon>(), null);
    }
}