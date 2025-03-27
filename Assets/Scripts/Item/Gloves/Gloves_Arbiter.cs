using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Arbiter : Item
{
    public Gloves_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(this)) { PercentageAmount = 1} };
    }
}
