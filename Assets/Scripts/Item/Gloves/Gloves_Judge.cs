using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Judge : Item
{
    public Gloves_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
