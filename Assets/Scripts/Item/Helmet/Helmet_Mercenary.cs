using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Mercenary : Item
{
    public Helmet_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertHealthToInjury", 0.5f), new ItemEffect("ConvertStaggerBarToStagger", 0.5f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
