using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Greatsword_HollowDrive : Item
{
    //40 overall at 0.5 attk, 30 at 0.75, 20 at 1, 15 at 1.5, 10 at 2
    public Greatsword_HollowDrive(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(9, 15, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BlockingGivesBarrierPerCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyTechniqueDamage")};
    }
}
