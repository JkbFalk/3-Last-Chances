using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_ExpertBadge : Item
{
    public new float[] Duration = new float[] { 1, 1, 1, 1, 1};
    public float[] TotalEnergyRestored = new float[] { 40.99f, 50.99f, 65.99f, 80.99f, 100.99f};
    public float[] CooldownIncrease = new float[] { 120, 100, 80, 60, 30};
    public static float[] CooldownPerGrade = new float[]{60, 50, 40, 30, 15};

    public Tool_ExpertBadge(ItemGrade grade) : base(grade)
    {
        Type = Constants.ItemType.Tool;
        OnUseAbility = typeof(Ability_ExpertBadge);
    }

    public override string GetDescription(bool detailed = true)
    {
        return string.Format(Label.Get(GetType().ToString() + "_Description" + (detailed ? "Detailed" : "")), new object[] { Utils.GetFormattedFloat((int)(TotalEnergyRestored[GradeIndex])), Utils.GetFormattedFloat(CooldownIncrease[GradeIndex]) }) + (detailed ? "" : " <sprite name=\"Detailed\">") + "\n\n[CD] " + GetCooldown(GetType(), Grade).ToString();
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        if(user.Energy.Current >=  user.Energy.Maximum) {
            NotificationController.ShowTextNotification(Label.Get("CannotUseAtMaxEnergyWarning"));
        }
        return user.Energy.Current < user.Energy.Maximum;
    }
}