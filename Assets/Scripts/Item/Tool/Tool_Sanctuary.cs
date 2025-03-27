using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Tool_Sanctuary : Item
{
    public new float[] Duration = new float[] { 20, 22, 24, 26, 30};
    public float[] MaxDamageBlocked = new float[] { 250, 350, 500, 700, 1000};
    public static float Cooldown = 60;

    public Tool_Sanctuary(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Tool;
        OnUseAbility = typeof(Ability_DrinkItem);
        CustomAnimation = "ActivateItem";
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description"), new object[] { Utils.GetFormattedFloat(Duration[GradeIndex]), Utils.GetFormattedFloat(MaxDamageBlocked[GradeIndex]) }) + "\n\n[CD] " + Cooldown.ToString();
    }

    public override void OnUse()
    {
        base.OnUse();
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/Sanctuary_Use");
        Utils.CreateVisualEffect(new(this), "Sanctuary", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        Player.Instance.AddEffect(new Effect_CustomizableDamageChange(new(this)) { ShowsInUI=true, EffectGraphic = Utils.LoadSpriteFromMultiple("Tool Icons", "Tool Icons_2"), CustomParam = MaxDamageBlocked[GradeIndex], ConditionCheckAfterHitDamageCalculation = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.TargetOfDamage == Player.Instance)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury -= effect.CustomParam;
                    damage.Stagger -= effect.CustomParam;
                })}, Duration[GradeIndex]);
    }
}
