using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Tool_FortificationPotion : Item
{
    public float TotalStaggerRestored;
    public float[] HealDuration = new float[5] { 60, 60f, 60f, 60f, 60f };
    public static float Cooldown = 60;
    public Tool_FortificationPotion(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Tool;
        OnUseAbility = typeof(Ability_DrinkItem);
        TotalStaggerRestored = 30 * GetMultiplierForGrade();
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description"), new object[] { Utils.GetFormattedFloat(TotalStaggerRestored), Utils.GetFormattedFloat(HealDuration[GradeIndex]) }) + "\n\n[CD] " + Cooldown.ToString();
    }

    public override void OnUse()
    {
        base.OnUse();
        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.StaggerBar, new(this)) { ShowsInUI=true, EffectGraphic = Utils.LoadSpriteFromMultiple("Tool Icons", "Tool Icons_4"), RegenerationFlatModifier = TotalStaggerRestored / HealDuration[GradeIndex]}, HealDuration[GradeIndex]);
    }
}
