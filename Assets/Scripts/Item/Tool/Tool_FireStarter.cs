using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_FireStarter : Item
{
    public static float Cooldown = 45;
    public Tool_FireStarter(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Tool;
        OnUseAbility = typeof(Ability_FireStarter);
        DamageSources = new List<Ability.DamageSource>()
        {
            new Ability.DamageSource(0, 5 * GetMultiplierForGrade(), Constants.DamageType.None, "FireStarter_" + Grade.ToString()) {KnockbackInMeters = 3f},
            new Ability.DamageSource(5 * GetMultiplierForGrade(), 0, Constants.DamageType.None, "AoE") {KnockbackInMeters = 7f}
        };
    }

    public override string GetDescription(bool detailed = true)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "")), new object[] { Utils.GetFormattedFloat(5 * GetMultiplierForGrade()), Utils.GetFormattedFloat(5 * GetMultiplierForGrade()), Utils.GetFormattedFloat(3 * GetMultiplierForGrade()) }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n[CD] " + Cooldown.ToString();
    }
}