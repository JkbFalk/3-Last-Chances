using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwinBlades_GaleforgedRapier : Item
{
    public TwinBlades_GaleforgedRapier(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(20, 135, 1.05f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainInvincibleOnRiposteOrCounter")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("RiposteDamage", 0.5f), new ItemEffect("CounterDamage", 0.5f)};
    }
}

