using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_PlundererAbilityAmplify : Effect
{
    private GameObject _vfx;
    public Ability.AbilityFamily AmplifiedFamily = Ability.AbilityFamily.None;
    public Effect_PlundererAbilityAmplify(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        ShowsInUI = true;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnStart()
    {
        PathToUIGraphic = "UI/" + AmplifiedFamily;
        base.OnStart();
        UIText = Utils.GetFormattedFloat(PercentageAmount, 0);
        if(PercentageAmount <= 0) {
            return;
        }
        _vfx = Utils.CreateVisualEffect(SourceOfEffect, "Plunderer_" + AmplifiedFamily.ToString());
        _vfx.transform.SetParent(Player.Instance.SpriteRenderers["Heavy"].Bone);
        _vfx.transform.localEulerAngles = new Vector3(0, 0, 90);
        _vfx.transform.localPosition = new Vector2(1.3f, 0);
    }

    public override void OnEnd() {
        base.OnEnd();
        if(_vfx != null && _vfx?.gameObject?.IsDestroyed() == false) {
            _vfx.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
    }

    public override void OnInvokeHitDealt(DamageInstance damage) {
        FieldInfo family = damage.SourceOfDamage.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static);
        if(damage.SourceOfDamage.User == TargetOfEffect && family != null && family.GetValue(null).ToString() == AmplifiedFamily.ToString()) {
            damage.DamageDealtPercentageModifier = PercentageAmount;
            EndThisEffect();
            base.OnInvokeHitDealt(damage);
        } 
    }
}
