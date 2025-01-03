using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Tool_Caltrops : Item
{
    public static float Cooldown = 40;
    public Tool_Caltrops(ItemGrade grade) : base(grade)
    {
        Category = Constants.ItemCategory.Tool;
        OnUseAbility = typeof(Ability_Caltrops);
        DamageSources = new List<Ability.DamageSource>()
        {
            new Ability.DamageSource(0.1f * GetMultiplierForGrade(), 0, Constants.DamageType.None, "Caltrops"),
        };
    }

    public override string GetDescription(bool detailed = false)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "Simple")), new object[] { Ability_Caltrops.CaltropsAmount[GradeIndex], Utils.GetFormattedFloat(0.125f * GetFirstModifierEffectValue()), Utils.GetFormattedFloat(2 + 0.03125f * GetFirstModifierEffectValue()) }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n<sprite name=\"Cooldown\"> " + Cooldown.ToString();
    }
}
