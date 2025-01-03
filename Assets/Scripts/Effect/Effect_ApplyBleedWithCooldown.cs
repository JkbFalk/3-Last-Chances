using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Effect;

public class Effect_ApplyBleedWithCooldown : Effect
{
    public float[] MinHealth = {50 * Item.GetMultiplierForGrade(Item.ItemGrade.Regular), 50 * Item.GetMultiplierForGrade(Item.ItemGrade.Excellent), 50 * Item.GetMultiplierForGrade(Item.ItemGrade.Masterful), 50 * Item.GetMultiplierForGrade(Item.ItemGrade.Flawless), 50 * Item.GetMultiplierForGrade(Item.ItemGrade.Ultimate)};
    public float[] MaxHealth = {200 * Item.GetMultiplierForGrade(Item.ItemGrade.Regular), 200 * Item.GetMultiplierForGrade(Item.ItemGrade.Excellent), 200 * Item.GetMultiplierForGrade(Item.ItemGrade.Masterful), 200 * Item.GetMultiplierForGrade(Item.ItemGrade.Flawless), 200 * Item.GetMultiplierForGrade(Item.ItemGrade.Ultimate)};
    public int Grade = 0;
    public float Cooldown = 30;
    public float BleedAmount = 0;
    public Effect_ApplyBleedWithCooldown(float bleed_amount, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        BleedAmount = bleed_amount;
        Listeners.Add(EventManager.DamageDealt);
    }


    public override void OnEffectValueChanged()
    {
        BleedAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BleedAmount, 1), Utils.GetFormattedFloat(BleedAmount * 4, 1), MinHealth[Grade].ToString(), MaxHealth[Grade].ToString(), Utils.GetFormattedFloat(Cooldown)};
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsStrongBasicAttack && !damage.SourceOfDamage.User.CheckIfEffectIsOnCooldown(typeof(Effect_ApplyBleedWithCooldown)) && damage.IsDamageOverTime == false)
        {
            float actualBleed = Utils.GetValueBasedOnMinAndMax(damage.TargetOfDamage.Health.Maximum, MinHealth[Grade], MaxHealth[Grade], BleedAmount, BleedAmount*4);
            damage.TargetOfDamage.AddEffect(new Effect_Bleed(actualBleed, SourceOfEffect));
            damage.SourceOfDamage.User.AddCooldown(new Cooldown(GetType(), Cooldown, damage.SourceOfDamage.User) {ShowCooldownInEffectUI = true, CooldownGraphic = Utils.LoadSpriteFromMultiple("Heavy Icons", "Heavy Icons_8")});
            base.OnInvokeDamageDealt(damage);
        }
    }
}
 