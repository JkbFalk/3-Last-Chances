using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Enforcer : Item
{
    public Armor_Enforcer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Enforcer;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.5f }  };
    }
}