using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greatsword_Conviction : Item
{

    public Greatsword_Conviction(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Greatsword;
        SetBaseWeaponStats(9, 15, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BlockingGivesInvinciblePerCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyTechniqueDamage")};
    }
}
