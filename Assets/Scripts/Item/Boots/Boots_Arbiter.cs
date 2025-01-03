using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Arbiter : Item
{
    public Boots_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_ChangeEffectPower(typeof(Effect_Slow), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -GetFirstModifierEffectValue() / 3, new(this)) {AffectedUnitsType=Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player,}, 
            new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -GetFirstModifierEffectValue() / 3, new(this)) {AffectedUnitsType=Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 10 } };
    }
}
