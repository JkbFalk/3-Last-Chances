using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Arbiter : Item
{
    public Armor_Arbiter(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Arbiter;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Void)},EffectTypeName="DecreaseStaggerBarOnTakingDamage", CustomParam = GetFirstModifierEffectValue() * 0.625f, DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.625f).ToString()}, TriggersOncePerAbility = true, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.SourceOfDamage.User.AddEffect(new Effect_Void(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.Health, 15, 15f, new(this) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[R]"} };
    }
}
