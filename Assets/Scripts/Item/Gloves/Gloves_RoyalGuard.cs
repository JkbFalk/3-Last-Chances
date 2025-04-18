using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_RoyalGuard : Item
{
    public Gloves_RoyalGuard(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.RoyalGuard;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AnalysisAlsoIncreasesWeaponDamage")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("WeaponDamage")};
    }
}
