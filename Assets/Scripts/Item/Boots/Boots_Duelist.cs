using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Duelist : Item
{
    public Boots_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DamageReductionAfterRiposteOrCounter")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AttackSpeed", 0.5f), new ItemEffect("MovementSpeed", 0.5f)};
    }
}
