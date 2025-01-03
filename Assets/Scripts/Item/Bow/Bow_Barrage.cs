using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Barrage : Item
{
    public Bow_Barrage(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Bow;
        SetBaseWeaponStats(125, 35, 1.2f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {EffectTypeName="BarrageBow", CustomParam = GetFirstModifierEffectValue() * 0.5f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.5f)}, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack && damage.SourceOfDamage.GetType() == typeof(BA_Bow_F))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury += ((BA_Bow_F)damage.SourceOfDamage).BarrageComboNumber * effect.CustomParam;
            })}};
    }

    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.RangedAttackSpeed, new(this)) {PercentageAmount = 0.25f}};
    }
}
