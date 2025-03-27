using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_GainDamageAndDamageReductionForEachDebuff : Effect
{
    public float DamageReductionGainedPerDebuff;
    public float DamageGainedPerDebuff;
    public Effect DamageReductionBuff;
    public Effect_ChangeCompositeStat DamageBuff;
    public int MaxDebuffs;

    public Effect_GainDamageAndDamageReductionForEachDebuff(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.EffectEnded);
        TriggersOncePerAbility = true;
    }

    public override void OnEffectValueChanged()
    {
        DamageReductionGainedPerDebuff *= NonLinearEffectValue;
        DamageGainedPerDebuff *= NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(DamageGainedPerDebuff), Utils.GetFormattedFloat(DamageReductionGainedPerDebuff), MaxDebuffs.ToString()};
    }

    public void Activate() {
        if(EffectEnded && DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false) {
            DamageReductionBuff.EndThisEffect();
            DamageBuff.EndThisEffect();
        }
        else if(!EffectEnded) {
            int debuffCount = Player.Instance.CurrentEffects.Where(effect => effect.Type == EffectType.Debuff && effect.CountsAsSeparateEffect).Count();
            Debug.Log("XXX: Found debuffs: " + debuffCount);
            if(debuffCount > MaxDebuffs) {
                debuffCount = MaxDebuffs;
            }
            if(DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false) {
                DamageReductionBuff.EndThisEffect();
                DamageBuff.EndThisEffect();
            }
            if(debuffCount == 0) {
                return;
            }
            DamageReductionBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {ShowsInUI=true, EffectIndicatorText=Utils.GetFormattedFloat(DamageReductionGainedPerDebuff * debuffCount) + "%", PercentageAmount = DamageReductionGainedPerDebuff * debuffCount};
            DamageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, SourceOfEffect) {ShowsInUI=true, EffectIndicatorText=Utils.GetFormattedFloat(DamageGainedPerDebuff * debuffCount) + "%", PercentageAmount = DamageGainedPerDebuff * debuffCount};
            Player.Instance.AddEffect(DamageReductionBuff);
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
