using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Knight : Item
{
    public Gloves_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -GetFirstModifierEffectValue(false) * 0.625f, new(this))};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 }};
    }
}

