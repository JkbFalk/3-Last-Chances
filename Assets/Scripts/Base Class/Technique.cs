using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Technique : Ability
{
    public Technique(Unit ability_user) : base(ability_user)
    {
        if (Player.Instance.PreparingForUltimate)
        {
            Properties.Add(Property.Ultimate);
        }
        else if (SaveFile.Instance.ActiveUpgrades.Contains(GetType().ToString() + "_UpgradeA"))
        {
            Properties.Add(Property.UpgradeA);
        }
        else if (SaveFile.Instance.ActiveUpgrades.Contains(GetType().ToString() + "_UpgradeB"))
        {
            Properties.Add(Property.UpgradeB);
        }
        Properties.Add(Property.ImmuneToFlinch);
        HoldingTechniqueButton = true;
        Properties.Add(Property.Technique);
    }

    public void ConsumeEnergyAndCooldownForTheAbility()
    {
        User.Actions.ConsumeEnergyAndCooldownForTheAbility(this);
    }
}
