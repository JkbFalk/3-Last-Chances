using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Jailer : Item
{
    public Armor_Jailer(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Armor;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {CustomParam = GetFirstModifierEffectValue() * 0.375f, TriggersOncePerAbility = true, EffectTypeName="ApplyChainedOnRiposteOrCounter", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 0.375f).ToString()}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.SourceOfDamage.User is Player && (damage.SourceOfDamage.IsRiposte || damage.SourceOfDamage.IsCounter)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(damage.SourceOfDamage.TriggeredEffects.Contains(effect) == false) {
                        damage.TargetOfDamage.AddEffect(new Effect_Chained(damage.SourceOfDamage.IsCounter ? effect.CustomParam * 2 : effect.CustomParam, new(this)));
                    }
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeEffectPower(typeof(Effect_Chained), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, GetSecondModifierEffectValue(false) * 0.5f, new(this)) };
    }
}
