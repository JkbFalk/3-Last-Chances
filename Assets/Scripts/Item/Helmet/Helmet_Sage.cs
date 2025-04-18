using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Sage : Item
{
    public Helmet_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("UsingTechniquesDecreasesRemainingCooldownOfAllOtherTechniques")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGain")};
    }
}

