using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_Reaper : Item
{
    public Longblade_Reaper(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Executioner;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(135, 50, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("HealOnTakedowns")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ReduceCooldownsOnTakedowns")};
    }
}
