using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_IronBlooded : Item
{
    public Helmet_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
