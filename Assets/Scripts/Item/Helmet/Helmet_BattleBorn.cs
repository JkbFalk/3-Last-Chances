using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Helmet_BattleBorn : Item
{
    public Helmet_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Category = Constants.ItemCategory.Helmet;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableEffectOnEvent(new(this)) { PercentageAmount = GetFirstModifierEffectValue() * 0.25f, EffectTypeName="GainAttackSpeedAsHealthLowers", DescriptionParameters = new List<string> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.25f)},
                ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => stat.Owner == Player.Instance && stat is Health), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    Effect_ChangeCompositeStat asBuff = (Effect_ChangeCompositeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "GainAttackSpeedAsHealthLowers");
                    if(asBuff == null) {
                        asBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new("GainAttackSpeedAsHealthLowers")) {PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), Identifier="GainAttackSpeedAsHealthLowers", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%"};
                        Player.Instance.AddEffect(asBuff);
                    }
                    else {
                        asBuff.PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                        asBuff.EffectIndicatorText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%";
                    }
            })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(this)) { FlatAmount = 1 }};
    }
}
