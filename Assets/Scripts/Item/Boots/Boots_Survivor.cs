using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boots_Survivor : Item
{
    public Boots_Survivor(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Survivor;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> { new Effect_CustomizableDamageChange(new(this)) {CustomParam = GetFirstModifierEffectValue() * 9.375f, EffectTypeName="HealOnFallingBelowHealthThreshold", DescriptionParameters=new List<String>{(GetFirstModifierEffectValue() * 9.375f).ToString(), "10", "60"}, ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    damage.TargetOfDamage == Player.Instance && Player.Instance.Health.Current < Player.Instance.Health.Maximum / 4),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(!Player.Instance.CheckIfEffectIsOnCooldown(effect)) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new(this)) {ShowsInUI=true, EffectGraphic = Utils.LoadSpriteFromMultiple("Boots Icons", "Boots Icons_2"), RegenerationFlatAmount = effect.CustomParam / 10}, 10);
                        Player.Instance.AddCooldown(effect, 60);
                    }
                })}};
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) { PercentageAmount = 0.5f } };
    }
}
