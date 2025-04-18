using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longblade_SwornProtector : Item
{
    public Longblade_SwornProtector(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(135, 50, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealsIncreasedStaggerOnceEveryNSecondsAndRefundCooldownOnHittingStaggered")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("HeavyStagger")};
    }
}
