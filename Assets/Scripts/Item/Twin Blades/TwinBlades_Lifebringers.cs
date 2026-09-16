using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_Lifebringers : Item
{
    public TwinBlades_Lifebringers(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(11, 4, 1.2f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("RestoreHealthWhenBasicAttacking")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("LightAttackSpeed")};
    }
}

