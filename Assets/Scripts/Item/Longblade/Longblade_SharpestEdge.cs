using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Longblade_SharpestEdge : Item
{
    public Longblade_SharpestEdge(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Longblade;
        SetBaseWeaponStats(90, 120, 0.9f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("ExtraEffectiveButConsumableSharp")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("ReceiveSharpWhileInProximity")};
    }
}
