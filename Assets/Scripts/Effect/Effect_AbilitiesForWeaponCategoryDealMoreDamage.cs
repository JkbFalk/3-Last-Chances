using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AbilitiesForWeaponCategoryDealMoreDamage : Effect { 
    public float IncreasedInjury = 0;
    public float IncreasedStagger = 0;
    public Constants.DamageType DamageCategory;

    public override string ToString()
    {
        string mod = "";
        if(IncreasedInjury != 0 && IncreasedStagger == 0)
        {
            mod = "Health";
        }
        else if(IncreasedInjury == 0 && IncreasedStagger != 0)
        {
            mod = "Stagger";
        }
        if (GetDescriptionParameters().Count > 0)
        {
            return Utils.GetFormattedLabel(GetType() + mod + "_DescriptionSimple", GetDescriptionParameters()) + (Label.ContainsKey(GetType() + "_DescriptionDetailed") ? " <sprite name=\"Detailed\">" : "");
        }
        return Utils.GetFormattedLabel(GetType() + mod + "_DescriptionSimple", GetDescriptionParameters());
    }
    public Effect_AbilitiesForWeaponCategoryDealMoreDamage(Constants.DamageType category, float health_damage_percentage, float stagger_damage_percentage, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        IncreasedInjury = health_damage_percentage;
        IncreasedStagger = stagger_damage_percentage;
        DamageCategory = category;
        RemainsActiveInOtherStances = true;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnEffectValueChanged()
    {
        IncreasedInjury *= LinearEffectValue;
        IncreasedStagger *= LinearEffectValue;
        DescriptionParameters = new List<string> { Label.Get("WeaponCategory_" + DamageCategory.ToString()), Utils.GetFormattedFloat(IncreasedInjury), Utils.GetFormattedFloat(IncreasedStagger), Utils.GetIconForPhrase(DamageCategory == Constants.DamageType.Heavy ? "HI" : DamageCategory == Constants.DamageType.Light ? "LI" : DamageCategory == Constants.DamageType.Ranged ? "RI" : "MI"), Utils.GetIconForPhrase(DamageCategory == Constants.DamageType.Heavy ? "HS" : DamageCategory == Constants.DamageType.Light ? "LS" : DamageCategory == Constants.DamageType.Ranged ? "RS" : "MS")};
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage?.SourceOfDamage?.User != TargetOfEffect) {
            return;
        }
        if(damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource.DamageType == Player.Instance.CurrentWeaponDamageCategory && IncreasedInjury > 0)
        {
            damage.ExtraInjuryDealtPercentage += IncreasedInjury;
        }
        if (damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource.DamageType == Player.Instance.CurrentWeaponDamageCategory && IncreasedStagger > 0)
        {
            damage.ExtraStaggerDealtPercentage += IncreasedStagger;
        }
        base.OnInvokeHitDealt(damage);
    }
}
