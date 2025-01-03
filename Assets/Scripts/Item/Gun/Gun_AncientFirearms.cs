using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_AncientFirearms : Item
{
    public Gun_AncientFirearms(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Ranged;
        WeaponClass = Constants.WeaponClass.Gun;
        SetBaseWeaponStats(130, 100, 0.75f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Supercharge)},EffectTypeName="ApplySuperchargeOnTakedown", CustomParam = GetFirstModifierEffectValue() * 0.5f, DescriptionParameters = new List<String> { Utils.GetFormattedFloat(GetFirstModifierEffectValue() * 0.5f)}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.DamageKilledTheTarget)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.SourceOfDamage.User.AddEffect(new Effect_Supercharge(effect.CustomParam, new(this)));
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.RangedInjury, new(this)) { PercentageAmount = 0.75f }, new Effect_ChangeStat(Player.Instance.RangedStagger, new(this)) { PercentageAmount = 0.75f }};
    }
}
