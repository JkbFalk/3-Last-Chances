using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armor_Duelist : Item
{
    public Armor_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_GainPortionOfStackingEffectAsStat(Player.Instance.DamageReduction, typeof(Effect_Sharp), 0.05f, GetFirstModifierEffectValue() * 0.75f, new(this)), new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, GetFirstModifierEffectValue(false) * 0.25f, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 15 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = -5 }};
    }
}
