using System.Collections.Generic;

public class BA_Gun_Backstab : Backstab {

    public BA_Gun_Backstab(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.Backstab);
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Ranged));
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Ranged"].Bone.GetComponent<UnitWeapon>(), null);
    }
}