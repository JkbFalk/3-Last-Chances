using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Ancient : Item
{
    public Helmet_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.CooldownReduction, new(this)) { PercentageAmount = 0.5f }, new Effect_AncientCrown(new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 } };
    }
}

