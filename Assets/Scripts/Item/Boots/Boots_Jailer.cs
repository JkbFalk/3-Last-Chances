using System;
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
        return new List<Effect> { new Effect_CustomizableEffectOnEvent(new(this)) { FlatAmount = GetFirstModifierEffectValue() * 9.375f, EffectTypeName="RestoreStaggerFromDebuffs", DescriptionParameters = new List<string> {Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 9.375f), "5"},
            ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect.TargetOfEffect == Player.Instance && effect.Type == Effect.EffectType.Debuff), 
            ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarted, effect) =>  {
                Player.Instance.StaggerBar.Current -= effect.FlatAmount;
          })}};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) { CustomParam = GetSecondModifierEffectValue(false) * 0.5f, EffectTypeName="HealFromBeingDamagedByStackingEffects", DescriptionParameters=new List<string>{Utils.GetFormattedFloat(GetSecondModifierEffectValue(false) * 0.5f)},
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.TargetOfDamage == Player.Instance && (damage.Is(Damage.DamageProperty.Burn) || damage.Is(Damage.DamageProperty.Freeze) || damage.Is(Damage.DamageProperty.Incision))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })} };
    }
}
