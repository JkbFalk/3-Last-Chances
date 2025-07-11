using System.Collections.Generic;
using UnityEngine;

public class Effect_ShadowInfusion : Effect {
    public float ExtraStagger = 0;
    public float LifestealPercentage = 0;
    private GameObject _visualEffect;
    private GameObject _visualEffect2;
    public Constants.DamageType EffectCategory;
    public Effect_ChangeStat AttackSpeedBuff;
    public Effect_ChangeStat ArmorEffect;
    public Effect_Unstunnable CcImmunityEffect;
    public float AttackSpeedBuffAmount = 0;

    public bool MasteryB = false;
    public bool MasteryA = false;

    public Effect_ShadowInfusion(float extra_stagger, float attack_speed_buff, Constants.DamageType weapon_category, SourceOfEffect source_of_effect) : base(source_of_effect) {
        ExtraStagger = extra_stagger;
        AttackSpeedBuffAmount = attack_speed_buff;
        EffectCategory = weapon_category;
        Type = EffectType.Buff;
        PathToUIGraphic = "Ability/ShadowInfusion";
        ShowsInUI = true;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnStart() {
        base.OnStart();
        if (EffectCategory == Constants.DamageType.Heavy) {
            _visualEffect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_SuperCharge_Heavy" + (SourceOfEffect.SourceAbility.Is(Ability.Property.UpgradeA) ? "_MasteryA" : ""))) as GameObject;
            _visualEffect.transform.SetParent(TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone").transform);
            _visualEffect.transform.localPosition = new Vector2(1f, 0);
            _visualEffect.transform.localRotation = Quaternion.Euler(0, 0, -90);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
        if (EffectCategory == Constants.DamageType.Light) {
            _visualEffect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_SuperCharge_Light" + (SourceOfEffect.SourceAbility.Is(Ability.Property.UpgradeA) ? "_MasteryA" : ""))) as GameObject;
            _visualEffect.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Right"].SpriteRenderer.transform.Find("Light Right Bone").transform);
            _visualEffect.transform.localPosition = new Vector2(0.5f, 0);
            _visualEffect.transform.localRotation = Quaternion.Euler(0, 0, -90);
            _visualEffect2 = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_SuperCharge_Light" + (SourceOfEffect.SourceAbility.Is(Ability.Property.UpgradeA) ? "_MasteryA" : ""))) as GameObject;
            _visualEffect2.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Left"].SpriteRenderer.transform.Find("Light Left Bone").transform);
            _visualEffect2.transform.localPosition = new Vector2(0.5f, 0);
            _visualEffect2.transform.localRotation = Quaternion.Euler(0, 0, -90);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.LightAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
        else if (EffectCategory == Constants.DamageType.Ranged) {
            _visualEffect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/VisualEffect/VisualEffect_SuperCharge_Ranged" + (SourceOfEffect.SourceAbility.Is(Ability.Property.UpgradeA) ? "_MasteryA" : ""))) as GameObject;
            _visualEffect.transform.SetParent(TargetOfEffect.SpriteRenderers["Ranged"].SpriteRenderer.transform.Find("Ranged Bone").transform);
            _visualEffect.transform.localPosition = new Vector2(0.25f, 0);
            _visualEffect.transform.localRotation = Quaternion.Euler(0, 0, -90);
            AttackSpeedBuff = new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, SourceOfEffect) {PercentageAmount = AttackSpeedBuffAmount};
            Player.Instance.AddEffect(AttackSpeedBuff);
        }
        if(MasteryB) {
            ArmorEffect = new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {PercentageAmount = 200};
            CcImmunityEffect = new Effect_Unstunnable(SourceOfEffect);
            Player.Instance.AddEffect(ArmorEffect, 3);
            Player.Instance.AddEffect(CcImmunityEffect, 3);
        }
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && CheckIfAbilityIsValidForSuperCharge(damage.SourceOfDamage)) {
            damage.Stagger += ExtraStagger;
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }

    public override void OnInvokeDamageDealt(Damage damage) {

        if (damage.SourceOfDamage.User == TargetOfEffect && CheckIfAbilityIsValidForSuperCharge(damage.SourceOfDamage)) {
            if(MasteryA)
            {
                damage.SourceOfDamage.User.Health.Current += damage.InjuryDealt * LifestealPercentage / 100 + damage.StaggerDealt * LifestealPercentage / 100;
            }
            base.OnInvokeDamageDealt(damage);
            EndThisEffect();
        }
    }

    public override void OnEnd() {
        base.OnEnd();
        MonoBehaviour.Destroy(_visualEffect);
        if (_visualEffect2 != null) {
            MonoBehaviour.Destroy(_visualEffect2);
        }
        AttackSpeedBuff.EndThisEffect();
    }

    public bool CheckIfAbilityIsValidForSuperCharge(Ability used_ability) {
        return used_ability != null && (used_ability.Is(Ability.Property.BasicAttack) || used_ability.Is(Ability.Property.Riposte) || used_ability.Is(Ability.Property.Counter) || used_ability.Is(Ability.Property.Backstab)) && used_ability.DamageSources.Count > 0 && used_ability.DamageSources[0].DamageType == EffectCategory;
    }
}