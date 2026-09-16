using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_Harpoon : Item
{
    public Polearm_Harpoon(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(7, 13, 0.85f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyStagger")};
    }
}
