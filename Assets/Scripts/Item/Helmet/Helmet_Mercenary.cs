using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Mercenary : Item
{
    public Helmet_Mercenary(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Mercenary;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 15 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = -5 } };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ProtectFromFlinchingOnce(new(this)) };
    }
}
