using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves_Brawler : Item
{
    public Gloves_Brawler(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Brawler;
        Category = Constants.ItemCategory.Gloves;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_BasicAttacksReduceCooldowns(new(this)) {CooldownReductionAmount = 0.015625f}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> {new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.5f }  };
    }
}