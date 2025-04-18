using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Knight : Item
{
    public Boots_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("FlatHealth", 0.5f), new ItemEffect("Health", 0.5f)};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FreezeAmountOnPlayer")};
    }
}
