using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_PureIron : Item
{
    public Armor_PureIron(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.PureIron;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
