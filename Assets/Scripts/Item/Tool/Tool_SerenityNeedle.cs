using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Tool_SerenityNeedle : Item
{
    public static float Cooldown = 15;
    public Tool_SerenityNeedle(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Tool;
        OnUseAbility = typeof(Ability_SerenityNeedle);
        DamageSources = new List<Ability.DamageSource>()
        {
            new Ability.DamageSource(0, GetMultiplierForGrade(), Constants.DamageType.None, "SerenityNeedle"),
        };
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "Simple")), new object[] { Ability_SerenityNeedle.SleepDurationBasedOnGrade[GradeIndex] }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n<sprite name=\"Cooldown\"> " + Cooldown.ToString();
    }
}
