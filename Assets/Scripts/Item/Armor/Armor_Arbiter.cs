using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Arbiter : Item
{
    public Armor_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.Health, 15, 15f, new(this) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[R]"} };
    }
}
