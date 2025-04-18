using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_AzureMoon : Item
{
    public Polearm_AzureMoon(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.ShadowGifted;
        Type = Constants.ItemType.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(110, 80, 0.95f);
        FirstItemEffects = new List<ItemEffect> {new ItemEffect("OnHitApplyRandomStackingEffectYouDoNotCurrentlyPossess")};
        SecondItemEffects = new List<ItemEffect> {new ItemEffect("EffectCooldownReduction")};
    }
}
