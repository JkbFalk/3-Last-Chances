using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Knight : Item
{
    public Armor_Knight(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Knight;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="GainKnightDRAfterHit", CustomParam = GetFirstModifierEffectValue(false) * 0.625f, DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.625f), "10"}, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.TargetOfDamage == Player.Instance),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                Effect drBuff = Player.Instance.GetEffect(new Func<Effect, bool> (effect => effect.ExtraInfo == "GainKnightDRAfterHit"));
                if(drBuff != null) {
                    drBuff.RemainingDuration = 10;
                }
                else {
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageAmount=effect.CustomParam, ExtraInfo="GainKnightDRAfterHit"}, 10);
                }
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.Health, new(this)) { FlatAmount = 5 }, new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { FlatAmount = 5 }};
    }
}
