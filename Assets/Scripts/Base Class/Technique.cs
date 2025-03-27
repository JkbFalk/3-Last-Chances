using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class Technique : Ability
{
    public Technique(Unit ability_user) : base(ability_user)    
    {
        Properties.Add(AbilityProperty.ImmuneToFlinch);
        HoldingTechniqueButton = true;
        Properties.Add(AbilityProperty.Technique);
    }
}
