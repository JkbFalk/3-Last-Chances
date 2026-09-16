using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Compile-time maps from weapon class / ability name to <see cref="Type"/>.
/// Replaces <c>Type.GetType("Prefix_" + name)</c> so renames fail at compile time
/// and missing mappings log instead of failing silently.
/// </summary>
public static class AbilityTypeRegistry
{
    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> Ripostes = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(TwinBlades_Riposte) },
        { Constants.WeaponClass.Gun, typeof(Gun_Riposte) },
        { Constants.WeaponClass.Bow, typeof(Bow_Riposte) },
        { Constants.WeaponClass.Polearm, typeof(Polearm_Riposte) },
        { Constants.WeaponClass.Daggers, typeof(Daggers_Riposte) },
        { Constants.WeaponClass.Greatsword, typeof(Greatsword_Riposte) },
        { Constants.WeaponClass.Longblade, typeof(Longblade_Riposte) },
        { Constants.WeaponClass.Cannon, typeof(Cannon_Riposte) },
        { Constants.WeaponClass.Magic, typeof(Magic_Riposte) },
        { Constants.WeaponClass.Gauntlets, typeof(Gauntlets_Riposte) },
    };

    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> RiposteCounters = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(TwinBlades_RiposteCounter) },
        { Constants.WeaponClass.Gun, typeof(Gun_RiposteCounter) },
        { Constants.WeaponClass.Bow, typeof(Bow_RiposteCounter) },
        { Constants.WeaponClass.Polearm, typeof(Polearm_RiposteCounter) },
        { Constants.WeaponClass.Daggers, typeof(Daggers_RiposteCounter) },
        { Constants.WeaponClass.Greatsword, typeof(Greatsword_RiposteCounter) },
        { Constants.WeaponClass.Longblade, typeof(Longblade_RiposteCounter) },
        { Constants.WeaponClass.Cannon, typeof(Cannon_RiposteCounter) },
        { Constants.WeaponClass.Magic, typeof(Magic_RiposteCounter) },
        { Constants.WeaponClass.Gauntlets, typeof(Gauntlets_RiposteCounter) },
    };

    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> RollCounters = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(TwinBlades_RollCounter) },
        { Constants.WeaponClass.Gun, typeof(Gun_RollCounter) },
        { Constants.WeaponClass.Bow, typeof(Bow_RollCounter) },
        { Constants.WeaponClass.Polearm, typeof(Polearm_RollCounter) },
        { Constants.WeaponClass.Daggers, typeof(Daggers_RollCounter) },
        { Constants.WeaponClass.Greatsword, typeof(Greatsword_RollCounter) },
        { Constants.WeaponClass.Longblade, typeof(Longblade_RollCounter) },
        { Constants.WeaponClass.Cannon, typeof(Cannon_RollCounter) },
        { Constants.WeaponClass.Magic, typeof(Magic_RollCounter) },
        { Constants.WeaponClass.Gauntlets, typeof(Gauntlets_RollCounter) },
    };

    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> BackstepCounters = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(TwinBlades_BackstepCounter) },
        { Constants.WeaponClass.Gun, typeof(Gun_BackstepCounter) },
        { Constants.WeaponClass.Bow, typeof(Bow_BackstepCounter) },
        { Constants.WeaponClass.Polearm, typeof(Polearm_BackstepCounter) },
        { Constants.WeaponClass.Daggers, typeof(Daggers_BackstepCounter) },
        { Constants.WeaponClass.Greatsword, typeof(Greatsword_BackstepCounter) },
        { Constants.WeaponClass.Longblade, typeof(Longblade_BackstepCounter) },
        { Constants.WeaponClass.Cannon, typeof(Cannon_BackstepCounter) },
        { Constants.WeaponClass.Magic, typeof(Magic_BackstepCounter) },
        { Constants.WeaponClass.Gauntlets, typeof(Gauntlets_BackstepCounter) },
    };

    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> RegularBasicAttacks = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(BA_TwinBlades_F) },
        { Constants.WeaponClass.Gun, typeof(BA_Gun_F) },
        { Constants.WeaponClass.Bow, typeof(BA_Bow_F) },
        { Constants.WeaponClass.Polearm, typeof(BA_Polearm_F) },
        { Constants.WeaponClass.Daggers, typeof(BA_Daggers_F) },
        { Constants.WeaponClass.Greatsword, typeof(BA_Greatsword_F) },
        { Constants.WeaponClass.Cannon, typeof(BA_Cannon_F) },
    };

    public static readonly IReadOnlyDictionary<Constants.WeaponClass, Type> Backstabs = new Dictionary<Constants.WeaponClass, Type>
    {
        { Constants.WeaponClass.TwinBlades, typeof(BA_TwinBlades_Backstab) },
        { Constants.WeaponClass.Gun, typeof(BA_Gun_Backstab) },
        { Constants.WeaponClass.Bow, typeof(BA_Bow_Backstab) },
        { Constants.WeaponClass.Polearm, typeof(BA_Polearm_Backstab) },
        { Constants.WeaponClass.Daggers, typeof(BA_Daggers_Backstab) },
        { Constants.WeaponClass.Greatsword, typeof(BA_Greatsword_Backstab) },
        { Constants.WeaponClass.Longblade, typeof(BA_Longblade_Backstab) },
        { Constants.WeaponClass.Cannon, typeof(BA_Cannon_Backstab) },
        { Constants.WeaponClass.Magic, typeof(BA_Magic_Backstab) },
        { Constants.WeaponClass.Gauntlets, typeof(BA_Gauntlets_Backstab) },
    };

    public static readonly Type[] GauntletLightBasicAttacks =
    {
        typeof(BA_Gauntlets_L1),
        typeof(BA_Gauntlets_L2),
        typeof(BA_Gauntlets_L3),
        typeof(BA_Gauntlets_L4),
        typeof(BA_Gauntlets_L5),
    };

    public static readonly Type[] LongbladeBasicAttacks =
    {
        typeof(BA_Longblade_F1),
        typeof(BA_Longblade_F2),
        typeof(BA_Longblade_F3),
        typeof(BA_Longblade_F4),
        typeof(BA_Longblade_F5),
    };

    public static readonly IReadOnlyDictionary<Ability.AbilityFamily, Type> PlundererFollowUps = new Dictionary<Ability.AbilityFamily, Type>
    {
        { Ability.AbilityFamily.Ignis, typeof(NPCAbility_PlundererIgnis) },
        { Ability.AbilityFamily.Glacies, typeof(NPCAbility_PlundererGlacies) },
        { Ability.AbilityFamily.Anima, typeof(NPCAbility_PlundererAnima) },
        { Ability.AbilityFamily.Molis, typeof(NPCAbility_PlundererMolis) },
        { Ability.AbilityFamily.Salutis, typeof(NPCAbility_PlundererSalutis) },
        { Ability.AbilityFamily.Tonitrui, typeof(NPCAbility_PlundererTonitrui) },
    };

    private static Dictionary<string, Type> _typesByName;

    private static void EnsureInitialized()
    {
        if (_typesByName != null)
        {
            return;
        }

        _typesByName = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            string assemblyName = assembly.GetName().Name;
            if (assemblyName != "Assembly-CSharp" && assemblyName != "Assembly-CSharp-firstpass")
            {
                continue;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = Array.FindAll(ex.Types, type => type != null);
            }

            foreach (Type type in types)
            {
                if (type == null || type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                if (typeof(Ability).IsAssignableFrom(type) || typeof(Effect).IsAssignableFrom(type) || typeof(Item).IsAssignableFrom(type))
                {
                    _typesByName[type.Name] = type;
                }
            }
        }
    }

    public static Type GetByName(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        EnsureInitialized();
        if (_typesByName.TryGetValue(typeName, out Type type))
        {
            return type;
        }

        return null;
    }

    public static Type GetRequiredByName(string typeName)
    {
        Type type = GetByName(typeName);
        if (type == null)
        {
            Debug.LogError("AbilityTypeRegistry has no type named '" + typeName + "'.");
        }
        return type;
    }

    /// <summary>
    /// Resolves a UnitAI inspector action name. Entries may be the full class name
    /// (e.g. NPCAbility_JumpSlam / AI_Chase) or the short NPC name (JumpSlam).
    /// </summary>
    public static Type ResolveUnitAIAction(string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return null;
        }

        Type type = GetByName(actionName);
        if (type != null)
        {
            return type;
        }

        if (!actionName.StartsWith("AI_", StringComparison.Ordinal) && !actionName.StartsWith("NPCAbility_", StringComparison.Ordinal))
        {
            type = GetByName("NPCAbility_" + actionName);
        }

        if (type == null)
        {
            Debug.LogError("Could not find ability type with name: " + actionName);
        }
        return type;
    }

    public static Type GetRiposte(Constants.WeaponClass weaponClass)
    {
        return GetMapped(Ripostes, weaponClass, "Riposte");
    }

    public static Type GetRiposteCounter(Constants.WeaponClass weaponClass)
    {
        return GetMapped(RiposteCounters, weaponClass, "RiposteCounter");
    }

    public static Type GetRollCounter(Constants.WeaponClass weaponClass)
    {
        return GetMapped(RollCounters, weaponClass, "RollCounter");
    }

    public static Type GetBackstepCounter(Constants.WeaponClass weaponClass)
    {
        return GetMapped(BackstepCounters, weaponClass, "BackstepCounter");
    }

    public static Type GetBackstab(Constants.WeaponClass weaponClass)
    {
        return GetMapped(Backstabs, weaponClass, "Backstab");
    }

    public static Type GetRegularBasicAttack(Constants.WeaponClass weaponClass)
    {
        return GetMapped(RegularBasicAttacks, weaponClass, "basic attack");
    }

    public static Type GetGauntletBasicAttack(int number)
    {
        int index = number - 1;
        if (index < 0 || index >= GauntletLightBasicAttacks.Length)
        {
            Debug.LogError("No gauntlet basic attack registered for number " + number + ".");
            return null;
        }
        return GauntletLightBasicAttacks[index];
    }

    public static Type GetLongbladeBasicAttack(int number)
    {
        int index = number - 1;
        if (index < 0 || index >= LongbladeBasicAttacks.Length)
        {
            Debug.LogError("No longblade basic attack registered for number " + number + ".");
            return null;
        }
        return LongbladeBasicAttacks[index];
    }

    public static Type GetPlundererFollowUp(Ability.AbilityFamily family, bool isOmni)
    {
        if (isOmni)
        {
            return typeof(NPCAbility_PlundererOmni);
        }
        if (PlundererFollowUps.TryGetValue(family, out Type type))
        {
            return type;
        }
        Debug.LogError("No plunderer follow-up registered for family " + family + ".");
        return null;
    }

    private static Type GetMapped(IReadOnlyDictionary<Constants.WeaponClass, Type> map, Constants.WeaponClass weaponClass, string kind)
    {
        if (map.TryGetValue(weaponClass, out Type type))
        {
            return type;
        }
        Debug.LogError("No " + kind + " registered for weapon class " + weaponClass + ".");
        return null;
    }
}
