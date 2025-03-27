using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Sage : Item
{
    public Gloves_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Category = Constants.ItemCategory.Gloves; 
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(this)) { PercentageAmount = 0.625f }};
    }
}
