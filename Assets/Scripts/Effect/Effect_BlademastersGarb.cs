using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect_BlademastersGarb : Effect {
    public float BladedBasicAttackBuff = 0.75f;
    public float BladedArmorDebuff = -0.25f;
    public float NonBladedBasicAttackDebuff = -0.25f;
    public float NonBladedArmorBuff = 0.75f;
    public Effect_ChangeStat ArmorBuff;
    public Effect_ChangeStat ArmorDebuff;

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
        if((EffectEnded || CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) && ArmorBuff != null && ArmorBuff.EffectEnded == false) {
            ArmorBuff.EndThisEffect();
            ArmorBuff = null;
        }
        if((EffectEnded || !CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) && ArmorDebuff != null && ArmorDebuff.EffectEnded == false) {
            ArmorDebuff.EndThisEffect();
            ArmorDebuff = null;
        }
        if(!CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass) && (ArmorBuff == null || ArmorBuff.EffectEnded) && !EffectEnded) {
            ArmorBuff = new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {PercentageAmount = NonBladedArmorBuff};
            Player.Instance.AddEffect(ArmorBuff);
        }
        if(CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass) && (ArmorDebuff == null || ArmorDebuff.EffectEnded) && !EffectEnded) {
            ArmorDebuff = new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {PercentageAmount = BladedArmorDebuff};
            Player.Instance.AddEffect(ArmorDebuff);
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
        if(damage.SourceOfDamage.User != Player.Instance || damage.SourceOfDamage.IsNot(Ability.Property.BasicAttack)) {
            return;
        }
        base.OnInvokeHitDealt(damage);
        if(CheckIfIsBladedWeapon(Player.Instance.CurrentStance.WeaponClass)) {
            damage.InjuryDealtPercentageModifier += BladedBasicAttackBuff;
            damage.StaggerDealtPercentageModifier += BladedBasicAttackBuff;
        }
        else {
            damage.InjuryDealtPercentageModifier += NonBladedBasicAttackDebuff;
            damage.StaggerDealtPercentageModifier += NonBladedBasicAttackDebuff;
        }
    }

    private bool CheckIfIsBladedWeapon(Constants.WeaponClass weapon_class) {
        return weapon_class == Constants.WeaponClass.Greatsword || weapon_class == Constants.WeaponClass.Longblade ||  weapon_class == Constants.WeaponClass.TwinBlades || weapon_class == Constants.WeaponClass.Daggers;
    }
}