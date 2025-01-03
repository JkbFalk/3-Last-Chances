using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Adept : Item
{
    public Gloves_Adept(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Adept;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_RefundCooldownForDamageCategory(Constants.DamageType.Magic, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.CooldownReduction, new(this)) { PercentageAmount = 0.5f } };
    }
}
