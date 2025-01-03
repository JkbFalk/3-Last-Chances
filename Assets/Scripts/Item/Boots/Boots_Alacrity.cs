using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boots_Alacrity : Item
{
    public Boots_Alacrity(ItemGrade grade) : base(grade)
    {
        Set = ItemSetEnum.Alacrity;
        Category = Constants.ItemCategory.Boots;
    }

    public override List<Effect> GetFirstModifier() {
        return new List<Effect> {
            new Effect_CustomizableEffectOnEvent(new(this)) {EffectTypeName="GainASAndMovementSpeedOnDodge", DescriptionParameters = new List<String> {Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.75f, 1), Utils.GetFormattedFloat(GetFirstModifierEffectValue(false) * 0.75f, 1), "3", "10"}, TriggersOncePerAbility=true, PercentageAmount = GetFirstModifierEffectValue(false) * 0.75f, ConditionCheckForDamageWasDodged = new Func<Damage, bool>((damage) => 
                damage.TargetOfDamage == Player.Instance && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.ExtraInfo == "GainASAndMovementSpeedOnDodge") == null), ActionOnDamageWasDodged = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) {PercentageAmount = effect.PercentageAmount, DisplayEffectIndicator=true, PathToEffectGraphic="UI/MovementSpeed"}, 3);
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) {PercentageAmount = effect.PercentageAmount}, 3);
                Player.Instance.AddCooldown(new Cooldown(typeof(Effect_CustomizableEffectOnEvent), 10, Player.Instance) {ExtraInfo="GainASAndMovementSpeedOnDodge"});
        })}};  
    }
    public override List<Effect> GetSecondModifier() {
        return new List<Effect> { new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(this)) { PercentageAmount = 0.2f }, new Effect_ChangeStat(Player.Instance.MovementSpeed, new(this)) { PercentageAmount = 0.2f } };
    }
}
