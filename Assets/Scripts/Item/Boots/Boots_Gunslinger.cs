using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Gunslinger : Item
{
    public Boots_Gunslinger(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Gunslinger;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_FarEnemiesTakeExtraDamage(4, 4, 3, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.CooldownReduction, new(this)) { PercentageAmount = 0.5f } };
    }
}
