using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Void : Item
{
    public Boots_Void(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Void;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ExtraVoid(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Void)}} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { PercentageAmount = 0.75f } };
    }
}
