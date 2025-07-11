using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_ThrowRandomExplosives : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Tonitrui;
    public List<Projectile> Dynamites = new List<Projectile>();
    public NPCAbility_ThrowRandomExplosives(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/DynamiteIgnite", 0.6f);
        AddCustomSound("Explosion", "Explosion/Explosion1", 0.8f);
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        DamageSources.Add(new DamageSource(120, 100, Constants.DamageType.Magic) {KnockbackInMeters = 3f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        for(int i = 0; i < UnityEngine.Random.Range(2, 7); i++) {
            Dynamites.Add(Utils.CreateProjectile(new(this), "Dynamite"));
            Dynamites[i].transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(45, 135) * (User.Actions.IsFlipped ? 1 : -1));
            Dynamites[i].FlightSpeed = UnityEngine.Random.Range(3, 10);
            Dynamites[i].CleanUpAfter(8);
        }
        GameController.Instance.WaitAndRunMethod(1, ExplodeDynamites);
    }

    public void ExplodeDynamites() {
        PlayCustomSound("Explosion");
        foreach(Projectile dynamite in Dynamites) {
            Utils.CreateAreaOfEffect(new(this), "DynamiteExplosion", dynamite.transform.position.x, dynamite.transform.position.y);
            MonoBehaviour.Destroy(dynamite.gameObject);
        }
    }
}