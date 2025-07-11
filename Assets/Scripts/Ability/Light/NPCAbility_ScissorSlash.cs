using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_ScissorSlash : Ability {
    public static float Cooldown = 6;

    public NPCAbility_ScissorSlash(Unit ability_user) : base(ability_user) {
        AddCustomSound("Scrape", "Steel/SteelScrape1", 1);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitExceptTwinWeapon;
        DamageSources.Add(new DamageSource(0, 400 / 2, Constants.DamageType.Light) {KnockbackInMeters = 3.5f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
    }
}