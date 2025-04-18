using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_BlademastersGarb : Effect {
    public float BladedBasicAttackBuff = 0.75f;
    public float BladedDamageReductionDebuff = -0.25f;
    public float NonBladedBasicAttackDebuff = -0.25f;
    public float NonBladedDamageReductionBuff = 0.75f;
    public Effect_ChangeStat DamageReductionBuff;
    public Effect_ChangeStat DamageReductionDebuff;

    public Effect_BlademastersGarb(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.StanceSwitched);
        Listeners.Add(EventManager.HitDealt);
        Listeners.Add(EventManager.ItemEquipped);
        TriggersOncePerAbility = true;
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

    public void Activate() {
        if((EffectEnded || CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) && DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false) {
            DamageReductionBuff.EndThisEffect();
            DamageReductionBuff = null;
        }
        if((EffectEnded || !CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) && DamageReductionDebuff != null && DamageReductionDebuff.EffectEnded == false) {
            DamageReductionDebuff.EndThisEffect();
            DamageReductionDebuff = null;
        }
        if(!CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass) && (DamageReductionBuff == null || DamageReductionBuff.EffectEnded) && !EffectEnded) {
            DamageReductionBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageModifier = NonBladedDamageReductionBuff};
            Player.Instance.AddEffect(DamageReductionBuff);
        }
        if(CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass) && (DamageReductionDebuff == null || DamageReductionDebuff.EffectEnded) && !EffectEnded) {
            DamageReductionDebuff = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageModifier = BladedDamageReductionDebuff};
            Player.Instance.AddEffect(DamageReductionDebuff);
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        base.OnInvokeItemEquipped(item1, item2);
        Activate();
    }

    public override void OnInvokeStanceSwitched()
    {
        base.OnInvokeStanceSwitched();
        Activate();
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != Player.Instance || damage.SourceOfDamage.IsNot(Ability.AbilityProperty.BasicAttack)) {
            return;
        }
        base.OnInvokeHitDealt(damage);
        if(CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) {
            damage.ExtraInjuryDealtPercentage += BladedBasicAttackBuff;
            damage.ExtraStaggerDealtPercentage += BladedBasicAttackBuff;
        }
        else {
            damage.ExtraInjuryDealtPercentage += NonBladedBasicAttackDebuff;
            damage.ExtraStaggerDealtPercentage += NonBladedBasicAttackDebuff;
        }
    }

    private bool CheckIfIsBladedWeapon(Constants.WeaponClass weapon_class) {
        return weapon_class == Constants.WeaponClass.Greatsword || weapon_class == Constants.WeaponClass.Longblade ||  weapon_class == Constants.WeaponClass.TwinBlades || weapon_class == Constants.WeaponClass.Daggers;
    }
}