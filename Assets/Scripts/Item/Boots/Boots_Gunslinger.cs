using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Gunslinger : Item
{
    public Boots_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainMovementSpeedWhenThereIsAnEnemyNearYou")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("BasicAttackDamage")};
    }
}
