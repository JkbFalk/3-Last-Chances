using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet_Assassin : Item
{
    public Helmet_Assassin(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Assassin;
        Category = Constants.ItemCategory.Helmet; 
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_LowerStealthAttackImmunity(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_ImmunityToStealthAttack)}} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.3f }, new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) { PercentageAmount = 0.3f } };
    }
}
