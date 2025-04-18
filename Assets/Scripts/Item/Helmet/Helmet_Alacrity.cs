using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Alacrity : Item
{
    public Helmet_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AfterDodgingEmpowerNextAttack")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("IncreaseInvincibilityTimeOfDodge")};
    }
}
