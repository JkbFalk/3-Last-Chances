using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Alacrity : Item
{
    public Boots_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("IncreaseInvincibilityTimeOfDodge")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("DodgingDamageAppliesProne")};
    }
}
