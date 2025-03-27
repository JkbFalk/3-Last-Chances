using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cannon_UnforgivableSin : Item
{
    public Cannon_UnforgivableSin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Cannon;
        SetBaseWeaponStats(70, 140, 0.85f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {
            new Effect_CustomizableEffectOnEvent(new(this)) { EffectTypeName="NoDescription", FlatAmount = GetFirstModifierEffectValue() * 0.75f,
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.BasicAttack)), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                Player.Instance.AddEffect(new Effect_Burn(effect.FlatAmount, new(this)));
            })},
            new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="GainBurnOnBasicAttackAndIncreaseDamageBasedOnBurn", CustomParam = GetFirstModifierEffectValue() * 1.5625f, DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.75f), Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 1.5625f)}, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage?.User == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                    damage.ExtraInjuryDealtPercentage += playersBurn.DecayingAmount * effect.CustomParam / 100 / 10;
                    damage.ExtraStaggerDealtPercentage += playersBurn.DecayingAmount * effect.CustomParam / 100 / 10;
            })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {
            new Effect_CustomizableEffectOnEvent(new(this)) { EffectTypeName="CleanseBurnOnWeaponSwitch", DescriptionParameters = new List<string> {Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.75f), "30"}, CustomParam = 30, FlatAmount = GetSecondModifierEffectValue() * 0.75f,
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch)) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Burn)) && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.ExtraInfo == "CleanseBurnOnWeaponSwitch") == null), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Player.Instance.AddCooldown(new Cooldown(effect.GetType(), effect.CustomParam, Player.Instance) {ExtraInfo="CleanseBurnOnWeaponSwitch"});
                    Effect_Burn playersBurn = (Effect_Burn)Player.Instance.GetEffect(typeof(Effect_Burn));
                    playersBurn.ChangeDecayingAmount(-effect.FlatAmount);
            })}};
    }
}
