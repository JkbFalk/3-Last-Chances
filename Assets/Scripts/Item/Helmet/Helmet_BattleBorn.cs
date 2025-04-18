using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helmet_BattleBorn : Item
{
    public Helmet_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainAttackSpeedAsHealthLowers")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Injury")};
    }
}
