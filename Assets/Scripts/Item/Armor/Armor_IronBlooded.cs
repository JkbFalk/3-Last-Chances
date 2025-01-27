using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_IronBlooded : Item
{
    public Armor_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
