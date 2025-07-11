using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_GainDamageAndArmorForEachDebuff : Effect
{
    public float ArmorGainedPerDebuff;
    public float DamageGainedPerDebuff;
    public Effect ArmorBuff;
    public Effect_ChangeCompositeStat DamageBuff;
    public int MaxDebuffs;

    public Effect_GainDamageAndArmorForEachDebuff(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.EffectEnded);
        TriggersOncePerAbility = true;
    }


    public void Activate() {
        if(EffectEnded && ArmorBuff != null && ArmorBuff.EffectEnded == false) {
            ArmorBuff.EndThisEffect();
            DamageBuff.EndThisEffect();
        }
        else if(!EffectEnded) {
            int debuffCount = Player.Instance.CurrentEffects.Where(effect => effect.Type == EffectType.Debuff && effect.CountsAsSeparateEffect).Count();
            if(debuffCount > MaxDebuffs) {
                debuffCount = MaxDebuffs;
            }
            if(ArmorBuff != null && ArmorBuff.EffectEnded == false) {
                ArmorBuff.EndThisEffect();
                DamageBuff.EndThisEffect();
            }
            if(debuffCount == 0) {
                return;
            }
            ArmorBuff = new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {
                ShowsInUI = true, 
                UIText = Utils.GetFormattedFloat(ArmorGainedPerDebuff * debuffCount, 0), 
                PercentageAmount = ArmorGainedPerDebuff * debuffCount
            };
            DamageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, SourceOfEffect) {
                ShowsInUI = true, 
                UIText = Utils.GetFormattedFloat(DamageGainedPerDebuff * debuffCount, 0), 
                PercentageModifier = DamageGainedPerDebuff * debuffCount
            };
            Player.Instance.AddEffect(ArmorBuff);
            Player.Instance.AddEffect(DamageBuff);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        Activate();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Activate();
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        if(effect.TargetOfEffect != Player.Instance || effect.Type != EffectType.Debuff) {
            return;
        }
        base.OnInvokeEffectStarted(effect);
        Activate();
    }

    public override void OnInvokeEffectEnded(Effect effect)
    {
        if(effect.TargetOfEffect != Player.Instance || effect.Type != EffectType.Debuff) {
            return;
        }
        base.OnInvokeEffectEnded(effect);
        Activate();
    }
}
