using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_DoubleEarthRipper : Ability {
    public static float Cooldown = 6;

    public NPCAbility_DoubleEarthRipper(Unit ability_user) : base(ability_user) {
        AddCustomSound("Bury", "Earth/Earth_Punch2", 0.5f);
        AddCustomSound("Jump", "Explosion/Ground Explosion", 0.6f);
        AddCustomSound("Wind", "Wind/WindWhoosh3", 0.75f);
        DamageSources.Add(new DamageSource(60, 200, Constants.DamageType.Light));
        DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Light, "AoE 1"));
        DamageSources.Add(new DamageSource(150, 0, Constants.DamageType.Light, "AoE 2"));
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        base.CallAbilityEvent1();
        GameObject vfx1 = Utils.CreateVisualEffect(new(this), "EarthRip");
        vfx1.AddComponent<AttachObjectToBodyPart>();
        vfx1.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Light Right";
        vfx1.GetComponent<AttachObjectToBodyPart>().AttachToBone = true;
        vfx1.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        GameObject vfx2 = Utils.CreateVisualEffect(new(this), "EarthRip");
        vfx2.AddComponent<AttachObjectToBodyPart>();
        vfx2.GetComponent<AttachObjectToBodyPart>().BodyPartName = "Light Left";
        vfx2.GetComponent<AttachObjectToBodyPart>().AttachToBone = true;
        vfx2.GetComponent<AttachObjectToBodyPart>().Initialize(User);
        ChaseCurrentTargetAtGivenDegreeAngle(3, 70);
    }

    public override void CallAbilityEvent3()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(2, 45);
    }

    public override void CallAbilityEvent2()
    {
        Projectile proj1 = Utils.CreateProjectile(new(this), "WindSlash", User.ProjectileSpawnLocation.transform.position.x - 0.25f, User.ProjectileSpawnLocation.transform.position.y + 0.25f);
        proj1.gameObject.name = "AoE 1";
        Projectile proj2 = Utils.CreateProjectile(new(this), "WindSlash", User.ProjectileSpawnLocation.transform.position.x + 0.25f, User.ProjectileSpawnLocation.transform.position.y - 0.25f);
        proj2.gameObject.name = "AoE 2";
    }
}