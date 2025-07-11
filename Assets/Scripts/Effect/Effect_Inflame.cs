using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_Inflame : Effect {
    private GameObject _vfx;
    private GameObject _vfx2;
    public Constants.DamageType DamageCategory;

    public Effect_Inflame( Constants.DamageType weapon_category, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners.Add(EventManager.HitDealt);
        ShowsInUI = true;
        DamageCategory = weapon_category;
        Type = EffectType.Buff;
        PathToUIGraphic = "UI/Ignis";
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnStart() {
        base.OnStart();
        if (DamageCategory == Constants.DamageType.Heavy) {
            _vfx = Utils.CreateVisualEffect(SourceOfEffect, "Inflame_Heavy");
            _vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Heavy"].SpriteRenderer.transform.Find("Heavy Bone").transform);
            _vfx.transform.localPosition = new Vector2(0.5f, 0);
            _vfx.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (DamageCategory == Constants.DamageType.Light) {
            _vfx = Utils.CreateVisualEffect(SourceOfEffect, "Inflame_Light");
            _vfx.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Right"].SpriteRenderer.transform.Find("Light Right Bone").transform);
            _vfx.transform.localPosition = new Vector2(0.5f, 0);
            _vfx.transform.localRotation = Quaternion.Euler(0, 0, -90);
            _vfx2 = Utils.CreateVisualEffect(SourceOfEffect, "Inflame_Light");
            _vfx2.transform.SetParent(TargetOfEffect.SpriteRenderers["Light Left"].SpriteRenderer.transform.Find("Light Left Bone").transform);
            _vfx2.transform.localPosition = new Vector2(0.5f, 0);
            _vfx2.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(EffectEnded || damage?.SourceOfDamage?.User != TargetOfEffect || damage?.AbilityDamageSource?.DamageType != DamageCategory || (damage?.InjuryWasHigherThan0 == false && damage?.StaggerWasHigherThan0 == false)) {
            return;
        }
        damage.TargetOfDamage.AddEffect(new Effect_Burn(damage.InjuryDealt / 5 + damage.StaggerDealt / 5, SourceOfEffect));
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnEnd() {
        base.OnEnd();
        if (_vfx != null && _vfx.gameObject.IsDestroyed() == false) {
            _vfx.GetComponent<ParticleSystem>().Stop();
            _vfx.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
        if (_vfx2 != null && _vfx2.gameObject.IsDestroyed() == false) {
            _vfx2.GetComponent<ParticleSystem>().Stop();
            _vfx2.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
    }
}