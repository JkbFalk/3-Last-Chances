using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polearm_HeavenlyHalberd : Item
{
    public Polearm_HeavenlyHalberd(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Heavy;
        WeaponClass = Constants.WeaponClass.Polearm;
        SetBaseWeaponStats(25, 125, 1.1f);
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {new Effect_CustomizableDamageChange(new(this)) {UsesTheFollowingEffects=new() {typeof(Effect_Staggered)}, EffectTypeName="StealStaggerBarUsingHeavyHits", CustomParam = GetFirstModifierEffectValue(), DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue())}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    float stolenStagger = effect.CustomParam;
                    if(stolenStagger > damage.TargetOfDamage.StaggerBar.Maximum - damage.TargetOfDamage.StaggerBar.Current) {
                        stolenStagger = damage.TargetOfDamage.StaggerBar.Maximum - damage.TargetOfDamage.StaggerBar.Current + 0.1f;
                    }
                    damage.TargetOfDamage.StaggerBar.Current += stolenStagger;
                    Player.Instance.StaggerBar.Current -= stolenStagger;
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.HeavyStagger, new(this)) { PercentageAmount = 1.25f, RemainsActiveInOtherStances = true }};
    }
}
