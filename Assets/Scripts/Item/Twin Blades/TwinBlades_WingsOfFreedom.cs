using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinBlades_WingsOfFreedom : Item
{
    public TwinBlades_WingsOfFreedom(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(110, 40, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="ConvertChainedIntoHealth", CustomParam = GetFirstModifierEffectValue() * 0.4f, CustomParam2 = GetSecondModifierEffectValue() * 0.5f, DescriptionParameters = new List<String> { "15", Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.4f)}, 
            ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Chained))),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                Effect_Chained playersChained = (Effect_Chained)Player.Instance.GetEffect(typeof(Effect_Chained));
                Player.Instance.Health.Current += effect.CustomParam / 10 * playersChained.DecayingAmount * 0.15f;
                if(effect.CustomParam2 > 0) {
                    Player.Instance.AddEffect(new Effect_Barrier(effect.CustomParam2 / 10 * playersChained.DecayingAmount * 0.15f, new(this)));
                }  
                playersChained.ChangeDecayingAmount(-playersChained.DecayingAmount * 0.15f);
            })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_Description(new(this)) {EffectTypeName = "ConvertedChainedGeneratesBarrier", DescriptionParameters=new List<string> {Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.5f)}}};
    }
}

