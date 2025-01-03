using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Jailer : Item
{
    public Boots_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, GetFirstModifierEffectValue(false) * -0.5f, new(this)) };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Tenacity, new(this)) { PercentageAmount = 0.5f } };
    }
}
