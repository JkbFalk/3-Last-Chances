using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Duelist : Item
{
    public Boots_Duelist(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Duelist;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableEffectOnEvent(new(this)) { EffectTypeName="ReducedDamageAfterRiposteOrCounter", DescriptionParameters = new() {Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.5f)}, PercentageAmount = GetFirstModifierEffectValue(false) * 0.5f, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                (ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter)))), 
            ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, customizableEffect) =>  {
                if(ability.Is(Ability.AbilityProperty.Riposte)) {
                    if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.Identifier == "ReducedDamageAfterRiposte") != null) {
                        Player.Instance.CurrentEffects.FirstOrDefault(e => e.Identifier == "ReducedDamageAfterRiposte").EndThisEffect();
                    }
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(ability)) {PercentageAmount = customizableEffect.PercentageAmount, Identifier="ReducedDamageAfterRiposte"});
                }
                else {
                    if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.Identifier == "ReducedDamageAfterCounter") != null) {
                        Player.Instance.CurrentEffects.FirstOrDefault(e => e.Identifier == "ReducedDamageAfterCounter").EndThisEffect();
                    }
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(ability)) {PercentageAmount = customizableEffect.PercentageAmount * 2, Identifier="ReducedDamageAfterCounter"});
                }
        })} };
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.25f }, new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) { PercentageAmount = 0.25f } };
    }
}
