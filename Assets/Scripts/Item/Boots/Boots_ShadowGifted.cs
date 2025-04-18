using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_ShadowGifted : Item
{
    public Boots_ShadowGifted(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Boots;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("GainDamageReductionBasedOnPositiveStackingEffectsOnYouAndNegativeOnEnemy")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Damage")};
    }
}
