using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_BlazingShadowWatchesYourBack : Effect
{
    public float BurningInflicted = 100;
    public float DecreasedBackstabDamageTaken = 100;
    public Effect_BlazingShadowWatchesYourBack(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DecreasedBackstabDamageTaken), Utils.GetFormattedFloat(BurningInflicted)};
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DecreasedBackstabDamageTaken), Utils.GetFormattedFloat(BurningInflicted)};
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect || TargetOfEffect.CheckIfEffectIsOnCooldown(typeof(Effect_BlazingShadowWatchesYourBack))) {
            return;
        }
        if(((Player.Instance.Actions.IsFlipped && damage.SourceOfDamage.User.Actions.IsFlipped && damage.SourceOfDamage.User.transform.position.x > Player.Instance.transform.position.x)
        ||
        (!Player.Instance.Actions.IsFlipped && !damage.SourceOfDamage.User.Actions.IsFlipped && damage.SourceOfDamage.User.transform.position.x < Player.Instance.transform.position.x)) 
        &&
        damage.SourceOfCollision.GetComponentInParent<Projectile>() == null)
        {
            damage.Injury -= DecreasedBackstabDamageTaken;
            damage.Stagger -= DecreasedBackstabDamageTaken;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/Fire11", 0.5f);
            GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "BlazingShadowRetaliation", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
            damage.SourceOfDamage.User.AddEffect(new Effect_Burn(BurningInflicted, SourceOfEffect));
            vfx.transform.eulerAngles = new Vector3(0, Player.Instance.Actions.IsFlipped ? 0 : 180, 0);
            TargetOfEffect.AddCooldown(this, 10);
            base.OnInvokeHitDealt(damage);
        }
    }
}
