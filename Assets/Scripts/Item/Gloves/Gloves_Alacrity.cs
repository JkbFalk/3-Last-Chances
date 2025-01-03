using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Alacrity : Item
{
    public Gloves_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.5f } };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 10 } };
    }
}
