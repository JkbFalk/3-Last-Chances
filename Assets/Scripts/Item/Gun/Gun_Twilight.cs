using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Twilight : Item
{
    public Gun_Twilight(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(11.5f, 11.5f, 0.8f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ConvertAllInjuryIntoStaggerOrViceVersaDependingOnWhichIsLowerForEnemy")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage")};
    }
}
