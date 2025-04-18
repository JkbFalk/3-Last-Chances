using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_ShadowGifted : Item
{
    public Gloves_ShadowGifted(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Gloves;
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("AllStackingEffectsAmount")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("AllStackingEffectsDecay")};
    }
}
