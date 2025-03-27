using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Ancient : Item
{
    public Armor_Ancient(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Ancient;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(this)) { PercentageAmount = 0.5f }, 
            new Effect_AncientBreastplate(new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) { PercentageAmount = 0.5f } };
    }
}
