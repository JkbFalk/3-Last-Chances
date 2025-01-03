using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Adept : Item
{
    public Helmet_Adept(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Adept;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 1 }};
    }
}

