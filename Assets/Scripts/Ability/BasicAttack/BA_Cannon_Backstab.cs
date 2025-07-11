using System.Collections.Generic;

public class BA_Cannon_Backstab : Backstab {

    public BA_Cannon_Backstab(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Backstab);
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Heavy));
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Heavy"].Bone.GetComponent<UnitWeapon>(), null);
    }
}