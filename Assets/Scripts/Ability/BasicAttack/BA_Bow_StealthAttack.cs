using System.Collections.Generic;

public class BA_Bow_StealthAttack : StealthAttack {

    public BA_Bow_StealthAttack(Unit ability_user) : base(ability_user) {
        IsStealthAttack = true;
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Ranged));
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Ranged"].Bone.GetComponent<UnitWeapon>(), null);
    }
}