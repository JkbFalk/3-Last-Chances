using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Brawler : Item
{
    public Boots_Brawler(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Brawler;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { EffectTypeName="BasicAttackDamage", DescriptionParameters = new() {Utils.GetFormattedFloat(GetFirstModifierEffectValue())}, InjuryPercentageChange = GetFirstModifierEffectValue(), StaggerPercentageChange = GetFirstModifierEffectValue(), ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack))} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}