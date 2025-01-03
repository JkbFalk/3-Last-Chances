using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Sage : Item
{
    public Helmet_Sage(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Sage;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.MagicInjury, new(this)) {PercentageAmount = 0.75f}, new Effect_ChangeStat(Player.Instance.MagicStagger, new(this)) {PercentageAmount = 0.75f} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.MagicAttackSpeed, new(this)) {PercentageAmount = 0.75f} };
    }
}

