using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_EclipseBoundary : Item
{
    public Longblade_EclipseBoundary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(13.5f, 5, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("HeavyDamageAppliesMassiveInjuryAndEvenMoreIfBackstab")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyInjury", 0.5f), new ItemEffect("BackstabInjury", 0.5f)};
    }
}
