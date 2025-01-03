using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Survivor : Item
{
    public Armor_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
