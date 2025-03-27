using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwinBlades_ShacklesOfDuty : Item
{
    public TwinBlades_ShacklesOfDuty(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Jailer;
        Category = Constants.ItemCategory.Light;
        WeaponClass = Constants.WeaponClass.TwinBlades;
        SetBaseWeaponStats(120, 40, 1.25f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableEffectOnEvent(new(this)) {DescriptionParameters = new List<String> {"2", Utils.GetFormattedFloat(0.8f * GetFirstModifierEffectValue(), 1)}, EffectTypeName="ConvertCurrentHealthIntoChained", FlatAmount = 0.8f * GetSecondModifierEffectValue(), ConditionCheckForOneFifthSecondElapsedNotRealtime = new Func<bool>(() => 
            Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.1f && Player.Instance.InCombat), ActionOnOneFifthSecondElapsedNotRealtime = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                Player.Instance.Health.Current -= Player.Instance.Health.Maximum * 0.02f / 5;
                Player.Instance.AddEffect(new Effect_Chained(effect.FlatAmount * (Player.Instance.Health.Maximum * 0.02f / 5 / 100), new(this)));
        })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="BasicAttacksRestoreHealth", CustomParam = GetSecondModifierEffectValue() * 0.4f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetSecondModifierEffectValue() * 0.4f)}, 
            ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                Player.Instance.Health.Current += effect.CustomParam;  
            })}};
    }
}

