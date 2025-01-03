using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Knight : Item
{
    public Boots_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -GetFirstModifierEffectValue(false) * 0.625f, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 }};
    }
}
