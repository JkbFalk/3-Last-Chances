using System;
using Steamworks;
using TMPro;
using UnityEngine;

public class CooldownReduction : Stat {

    public CooldownReduction(Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find("CooldownReduction/Value").GetComponent<TextMeshProUGUI>();
        }
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public float GetEffectCooldownReduction(Type effect_type = null)
    {
        float extraCDR = 0;
        foreach (Effect e in Owner.GetEffects(effect => effect.Id == "EffectCooldownReduction" || effect.Id == ("EffectCooldownReduction - " + effect_type.ToString()))) {
            extraCDR += e.FlatAmount;
        }
        return Current + extraCDR;
    }

    public float GetToolCooldownReduction(Type tool_type = null)
    {
        float extraCDR = 0;
        foreach (Effect e in Owner.GetEffects(effect => effect.Id == "ToolCooldownReduction" || effect.Id == ("ToolCooldownReduction - " + tool_type.ToString()))) {
            extraCDR += e.FlatAmount;
        }
        return Current + extraCDR;
    }

    public float GetTechniqueCooldownReduction(Type ability_type)
    {
        string family = ability_type == null || ability_type.IsSubclassOf(typeof(Ability)) ? "" : Ability.GetFamily(ability_type).ToString();
        float extraCDR = 0;
        foreach (Effect e in Owner.GetEffects(effect => effect.Id == "TechniqueCooldownReduction" || effect.Id == ("TechniqueCooldownReduction - " + ability_type.ToString()) || effect.Id == ("TechniqueCooldownReduction - " + family))) {
            extraCDR += e.FlatAmount;
        }
        return Current + extraCDR;
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = (Current < 0 ? "" : "+") + Utils.GetFormattedFloat(Current);
    }
}