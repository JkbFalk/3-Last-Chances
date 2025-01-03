using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_PureIron : Item
{
    public Gloves_PureIron(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.PureIron;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
