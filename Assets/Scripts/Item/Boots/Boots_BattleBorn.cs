using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_BattleBorn : Item
{
    public Boots_BattleBorn(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.BattleBorn;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { 
            new Effect_CustomizableEffectOnEvent(new(this)) { PercentageAmount = GetFirstModifierEffectValue() * 0.25f, EffectTypeName="GainDamageReductionAsHealthLowers", DescriptionParameters = new List<string> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.25f)},
                ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => stat.Owner == Player.Instance && stat is Health), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "GainDamageReductionAsHealthLowers");
                    if(drBuff == null) {
                        drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, new("GainDamageReductionAsHealthLowers")) {PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum), Identifier="GainDamageReductionAsHealthLowers", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%"};
                        Player.Instance.AddEffect(drBuff);
                    }
                    else {
                        drBuff.PercentageAmount = effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum);
                        drBuff.EffectIndicatorText = Utils.GetFormattedFloat(effect.PercentageAmount * (1 - Player.Instance.Health.Current / Player.Instance.Health.Maximum)) + "%";
                    }
            })}};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(this)) { FlatAmount = 1 }};
    }
}
