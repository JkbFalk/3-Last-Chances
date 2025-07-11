using System.Collections.Generic;
using UnityEngine;

public class Effect_ShadowInfusion_Ultimate : Effect {
    private GameObject _vfx;
    private GameObject _vfx2;
    public Constants.DamageType DamageCategory;
    public Effect_ChangeStat AttackSpeedBuff;
    public float AttackSpeedBuffAmount;
    public float ExtraStagger;
    public float ExtraInjury;

    private List<Ability> EmpoweredAttacks = new List<Ability>();

    public Effect_ShadowInfusion_Ultimate( Constants.DamageType weapon_category, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners.Add(EventManager.HitDealt);
        ShowsInUI = true;
        PathToUIGraphic = "Effect/ShadowInfusion_Ultimate";
        DamageCategory = weapon_category;
        Type = EffectType.Buff;
    }

    public override void OnStart() {
        base.OnStart();
        if (DamageCategory == Constants.DamageType.Heavy) {
            _vfx = Utils.CreateVisualEffect(SourceOfEffect, "SpiritWeapon_Heavy");
            _vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone").transform);
            _vfx.transform.localPosition = new Vector2(0f, 0);
            _vfx.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Utils.CopyWeaponCollider(_vfx.GetComponent<BoxCollider2D>(), Constants.DamageType.Heavy);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
        else if (DamageCategory == Constants.DamageType.Light) {
            _vfx = Utils.CreateVisualEffect(SourceOfEffect, "SpiritWeapon_Light");
            _vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Right"].SpriteRenderer.transform.Find("Light Right Bone").transform);
            _vfx.transform.localPosition = new Vector2(0f, 0);
            _vfx.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _vfx2 = Utils.CreateVisualEffect(SourceOfEffect, "SpiritWeapon_Light");
            _vfx2.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Left"].SpriteRenderer.transform.Find("Light Left Bone").transform);
            _vfx2.transform.localPosition = new Vector2(0f, 0);
            _vfx2.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Utils.CopyWeaponCollider(_vfx.GetComponent<BoxCollider2D>(), Constants.DamageType.Light);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.LightAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
        else if (DamageCategory == Constants.DamageType.Ranged) {
            _vfx = Utils.CreateVisualEffect(SourceOfEffect, "SpiritWeapon_Ranged");
            _vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Ranged"].SpriteRenderer.transform.Find("Ranged Bone").transform);
            _vfx.transform.localPosition = new Vector2(0f, 0);
            _vfx.transform.localRotation = Quaternion.Euler(0, 0, 0);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
    }


    public override void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && CheckIfAbilityIsValidForSuperCharge(damage.SourceOfDamage)) {
            damage.Stagger += ExtraStagger;
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }

    public override void OnEnd() {
        base.OnEnd();
        _vfx.GetComponent<ParticleSystem>().Stop();
        _vfx.GetComponent<TemporaryObject>().MakeObjectDisappear();
        if (DamageCategory == Constants.DamageType.Heavy) {
            GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Weapon Collider/" + SaveFile.Instance.Stances[0].WeaponClass.ToString())) as GameObject;
            Utils.CopyWeaponCollider(go.GetComponent<BoxCollider2D>(), Constants.DamageType.Heavy);
            MonoBehaviour.Destroy(go);
        }
        else if (DamageCategory == Constants.DamageType.Light) {
            GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Weapon Collider/" + SaveFile.Instance.Stances[1].WeaponClass.ToString())) as GameObject;
            Utils.CopyWeaponCollider(go.GetComponent<BoxCollider2D>(), Constants.DamageType.Light);
            MonoBehaviour.Destroy(go);
        }
        if (_vfx2 != null) {
            _vfx.GetComponent<ParticleSystem>().Stop();
            _vfx2.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
        AttackSpeedBuff.EndThisEffect();
    }

    public bool CheckIfAbilityIsValidForSuperCharge(Ability used_ability) {
        return TargetOfEffect.Actions.CurrentAbilityBeingPerformed != null && (TargetOfEffect.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.BasicAttack) || TargetOfEffect.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.Riposte) || TargetOfEffect.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.Counter) || TargetOfEffect.Actions.CurrentAbilityBeingPerformed.Is(Ability.Property.Backstab)) && used_ability.DamageSources.Count > 0 && used_ability.DamageSources[0].DamageType == DamageCategory;
    }
}