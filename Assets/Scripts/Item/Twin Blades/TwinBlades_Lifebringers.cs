using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_Lifebringers : Item
{
    public TwinBlades_Lifebringers(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(110, 40, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {};
    }
}

