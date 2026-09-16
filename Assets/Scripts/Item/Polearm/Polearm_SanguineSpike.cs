using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polearm_SanguineSpike : Item
{
    public Polearm_SanguineSpike(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(11, 8, 0.95f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageBasedOnMissingHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("GainArmorBasedOnMissingHealth")};
    }
}
