using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Arbiter : Item
{
    public Boots_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("EnemiesRegainStaggerBarXPercentSlower")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("StaggerBar")};
    }
}
