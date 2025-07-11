using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_Enforcer : Item
{
    public Outfit_Enforcer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhileAtFullEnergyBasicAttacksDealMoreDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EnergyGain", 0.5f), new ItemEffect("StrongBasicAttackDamage", 0.5f)};
    }
}