using UnityEngine;

public class BA_Polearm_StealthAttack : StealthAttack {

    public BA_Polearm_StealthAttack(Unit ability_user) : base(ability_user) {
        IsStealthAttack = true;
        DamageSources.Add(new DamageSource(User.InCombat ? 200 : 400, User.InCombat ? 500 : 1000, Constants.DamageType.Heavy) {Knockback = 150});
    }

    public override void CallAbilityEvent1()
    {
        HandleEnemyHit(Target, User.SpriteRenderers["Heavy"].Bone.GetComponent<UnitWeapon>(), null);
    }
}