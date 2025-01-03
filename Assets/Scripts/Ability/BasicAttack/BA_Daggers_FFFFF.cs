using UnityEngine;

public class BA_Daggers_FFFFF : BasicAttack {

    public BA_Daggers_FFFFF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(75, 150, Constants.DamageType.Light));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "DaggersBasicAttack");
        Utils.CopyGameObjectAppearance(proj.gameObject, Player.Instance.SpriteRenderers["Light Left"].SpriteResolver.gameObject, false);
        proj.transform.localScale = Player.Instance.SpriteRenderers["Light Left"].Bone.localScale;
    }

    public override void CallAbilityEvent2()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "DaggersBasicAttack");
        Utils.CopyGameObjectAppearance(proj.gameObject, Player.Instance.SpriteRenderers["Light Right"].SpriteResolver.gameObject, false);
        proj.transform.localScale = Player.Instance.SpriteRenderers["Light Right"].Bone.localScale;
    }
}