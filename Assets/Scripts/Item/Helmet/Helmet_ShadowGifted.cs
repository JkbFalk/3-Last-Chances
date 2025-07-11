using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_ShadowGifted : Item
{
    public Helmet_ShadowGifted(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Helmet;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("DealMoreDamageBasedOnPositiveStackingEffectsOnYouAndNegativeOnEnemy")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("Armor")};
    }
}
