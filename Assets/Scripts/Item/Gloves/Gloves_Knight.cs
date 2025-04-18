using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gloves_Knight : Item
{
    public Gloves_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BlockXAmountOfDamageOnceEveryNSeconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BurnAmountOnPlayer")};
    }
}

