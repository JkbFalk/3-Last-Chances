using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_RoyalGuard : Item
{
    public Boots_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
