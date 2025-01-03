using UnityEngine;

public class BA_Gun_FS : BasicAttack {
    public  static int AmmoRequiredToUseAbility = 2;
    public BA_Gun_FS(Unit ability_user) : base(ability_user) {
        IsStrongBasicAttack = true;
        DamageSources.Add(new DamageSource(300, 600, Constants.DamageType.Ranged));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1() {
        User.ApplyForce(User.Actions.IsFlipped ? Vector2.right * 1000 : Vector2.left * 1000, this);
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Gun/Gun_BasicAttack" + Utils.GetRandomSoundNumber("Gun_BasicAttack"), 0.6f);
    }
}