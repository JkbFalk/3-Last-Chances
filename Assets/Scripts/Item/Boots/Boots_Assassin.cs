using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Assassin : Item
{
    public Boots_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
