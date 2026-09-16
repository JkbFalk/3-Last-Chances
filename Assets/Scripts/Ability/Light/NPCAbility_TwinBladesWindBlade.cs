using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_TwinBladesWindBlade : Ability {
    public static float Cooldown = 6;

    public NPCAbility_TwinBladesWindBlade(Unit ability_user) : base(ability_user) {
        AddCustomSound("Scrape", "Steel/SteelScrape1", 1);
        AddCustomSound("Wind", "Wind/WindWhoosh3", 0.75f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnit;
        DamageSources.Add(new DamageSource(400, 0, Constants.DamageType.Light) {KnockbackInMeters = 1.5f});
    }

    public override void CallAbilityEvent1()
    {
        Projectile proj = Utils.CreateProjectile(new(this), "TwinBladesWindBlade");
        proj.transform.eulerAngles = new Vector3(0, 0, User.Actions.IsFlipped ? 90 : -90);
        Utils.Apply2DFlip(proj.gameObject, User.Actions.IsFlipped);
    }
}