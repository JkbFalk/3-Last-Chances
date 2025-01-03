using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_EnergyUpgrade : Effect
{
    public Ability.AbilityFamily Family;
    public Effect_EnergyUpgrade(SourceOfEffect source_of_effect) : base(source_of_effect) {
        IsRemovable = false;
    }
    
    public int UpgradeLevel = 1;
}
