using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCAbility_FireMissiles : Ability {
    public static float Cooldown = 12;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public List<Projectile> Missiles;
    public int MissileAmount;
    public NPCAbility_FireMissiles(Unit ability_user) : base(ability_user) {
        HitSoundVolume = 0.2f;
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        WaitTimeBeforeNextAction = 0.5f;
        MissileAmount = UnityEngine.Random.Range(4, 9);
        Missiles = new();
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        DamageSources.Add(new DamageSource(75, 25, Constants.DamageType.Magic));
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Fire/DynamiteIgnite", 0.5f);
        for(int i = 0; i < MissileAmount; i++) {
            Missiles.Add(Utils.CreateProjectile(new(this), "FireMissile"));
            Missiles[i].transform.SetParent(User.transform.root.parent);
            Missiles[i].transform.position = User.transform.position + new Vector3(UnityEngine.Random.Range(0, User.Actions.IsFlipped ? 200 : -200) / 100f, UnityEngine.Random.Range(-100, 100) / 100f);
            Missiles[i].CleanUpAfter(5 / User.MagicAttackSpeed.Current);
        }
    }

    public override void CallAbilityEvent2()
    {
        for(int i = 0; i < Missiles.Count; i++) {
            GameController.Instance.WaitAndRunMethod(0.2f * i, SendMissileInTargetsGeneralDirection, Missiles[i].gameObject);
        }
    }

    public void SendMissileInTargetsGeneralDirection(GameObject missile) {
        Utils.PlaySoundEffect(User.AudioSource, "Bow/Bullet Flyby " + UnityEngine.Random.Range(1, 21), 0.5f);
        missile.GetComponent<Projectile>().DealingDamage = true;
        missile.GetComponent<Projectile>().IsFlying = true;
        missile.transform.up = (Target.transform.position + new Vector3(UnityEngine.Random.Range(-30, 30) / 100f, UnityEngine.Random.Range(-30, 30) / 100f) - missile.transform.position).normalized;
    }


    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(10 * User.MagicStagger.Current / 100, new(this)));
    }
}