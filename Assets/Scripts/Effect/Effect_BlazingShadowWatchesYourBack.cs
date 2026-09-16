using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Effect_BlazingShadowWatchesYourBack : Effect
{
    public float BurnScalingInflictedToBackstabbers;
    public float ExtraArmorAgainstBackstabs;
    public Effect_BlazingShadowWatchesYourBack(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
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
            damage.ArmorModifier = ExtraArmorAgainstBackstabs;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Fire/Fire11", 0.5f);
            GameObject vfx = Utils.CreateVisualEffect(SourceOfEffect, "BlazingShadowRetaliation", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
            damage.SourceOfDamage.User.AddEffect(new Effect_Burn(BurnScalingInflictedToBackstabbers, SourceOfEffect));
            Utils.Apply2DFlip(vfx, Player.Instance.Actions.IsFlipped);
            TargetOfEffect.AddCooldown(this, 10);
            base.OnInvokeHitDealt(damage);
        }
    }
}
