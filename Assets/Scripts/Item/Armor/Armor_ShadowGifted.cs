using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_ShadowGifted : Item
{
    public Armor_ShadowGifted(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Armor;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
