using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Ancient : Item
{
    public Gloves_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { PercentageAmount = 0.5f}, new Effect_AncientGauntlets(new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Control, new(this)) {PercentageAmount = 0.5f}};
    }
}
