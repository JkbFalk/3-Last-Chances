using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_ShadowLeap : Technique
{
    public static float EnergyCost = 20;
    public static float Cooldown = 30;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.CurrentWeapon;

    public Ability_ShadowLeap(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Shadow;
        DamageSources.Add(new DamageSource(0, 0, Constants.DamageType.Light));
        AddCustomSound("Start", "Ability/Ability_WindBlast_Use", 0.4f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { };
    }
}