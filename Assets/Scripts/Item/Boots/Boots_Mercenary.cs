using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Mercenary : Item
{
    public Boots_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Category = Constants.ItemCategory.Boots; 
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.StaggerBar, 30, 15f, new(this) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[P]"}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 10 } };
    }
}
