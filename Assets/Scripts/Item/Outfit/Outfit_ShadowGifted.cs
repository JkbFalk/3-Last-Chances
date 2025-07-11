using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit_ShadowGifted : Item
{
    public Outfit_ShadowGifted(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Outfit;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("WhenAttackingAnEnemyWithAllSignatureEffectsAppliedDealMassiveDamageBasedOnAmountAndRemoveAllOfThem")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Health", 0.5f), new ItemEffect("StaggerBar", 0.5f)};
    }
}
