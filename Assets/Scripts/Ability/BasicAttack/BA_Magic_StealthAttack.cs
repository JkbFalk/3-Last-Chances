using System.Collections.Generic;

public class BA_Magic_StealthAttack : StealthAttack {

    public BA_Magic_StealthAttack(Unit ability_user) : base(ability_user) {
        IsStealthAttack = true;
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Magic) {Knockback = 150});
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Ranged"].Bone.GetComponent<UnitWeapon>(), null);
    }
}