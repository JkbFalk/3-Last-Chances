using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Judge : Item
{
    public Helmet_Judge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Judge;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}
