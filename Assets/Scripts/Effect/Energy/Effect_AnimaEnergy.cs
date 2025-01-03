using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AnimaEnergy : Effect_EnergyUpgrade
{
    public Effect_ChangeStat DRDuringHardCC;
    public Effect_AnimaEnergy(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners = new List<UnityEventBase> {EventManager.EffectStarted, EventManager.EffectEnded};
        Family = Ability.AbilityFamily.Anima;
    }
    public override void OnInvokeEffectStarted(Effect effect)
    {
        if(effect.TargetOfEffect is Player && effect.IsHardCrowdControl()) {
            if(DRDuringHardCC == null || DRDuringHardCC.EffectEnded) {
                DRDuringHardCC = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageAmount = 80};
            }
            Player.Instance.AddEffect(DRDuringHardCC);
            if(UpgradeLevel == 3 && effect.GetOriginalBaseDuration() >= 1 && Player.Instance.EffectCooldowns.FirstOrDefault(cooldown => cooldown.Type == typeof(Effect_AnimaEnergy)) == null) {
                Player.Instance.AddCooldown(this, 15);
                effect.EndThisEffect();
                Utils.PlaySoundEffect(Player.Instance.AudioSource, "Effect/AnimaEnergy3", 0.9f);
                Utils.CreateVisualEffect(SourceOfEffect, "Anima3", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
            }
        }
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        if(effect.TargetOfEffect is Player && effect.IsHardCrowdControl()) {
            if(DRDuringHardCC != null) {
                DRDuringHardCC.EndThisEffect();
            }
            if(UpgradeLevel >= 2 && effect.GetOriginalBaseDuration() >= 1) {
                effect.SourceOfEffect.User.AddEffect(new Effect_Prone(100, SourceOfEffect));
                effect.SourceOfEffect.User.AddEffect(new Effect_Onslaught(100, SourceOfEffect));
                Utils.PlaySoundEffect(effect.SourceOfEffect.User.AudioSource, "Effect/AnimaEnergy2", 0.6f);
                Utils.CreateVisualEffect(SourceOfEffect, "Anima2", effect.SourceOfEffect.User.transform.position.x, effect.SourceOfEffect.User.transform.position.y);
            }
        }
    }
}
