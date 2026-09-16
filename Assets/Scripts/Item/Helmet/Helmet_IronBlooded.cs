using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_IronBlooded : Item
{
    public Helmet_IronBlooded(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.IronBlooded;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RegeneratePortionOfInjuryTakenAsHealthOver30Seconds")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("FlatHealth")};
    }
}
