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
        SetBaseWeaponStats(13.5f, 5, 1.1f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealExtraHeavyStaggerWithCooldown")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RefundCooldownOnHittingStaggered")};
    }
}
