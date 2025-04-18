using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Alacrity : Item
{
    public Gloves_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainSuperchargeBasedOnDistanceTravelled")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("SuperchargeAmount")};
    }
}
