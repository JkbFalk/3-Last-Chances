using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwinBlades_BerserkersBlades : Item
{
    public TwinBlades_BerserkersBlades(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(15, 3, 0.75f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksRestoreHealBasedOnMissingHealth")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttacksDealMoreDamageBasedOnMissingHealth")};
    }
}

