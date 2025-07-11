using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_RykerPayback : Ability {

    public static float Cooldown = 15;
    public static new bool DoesNotRequireTarget = true;
    private GameObject _aoe;
    public static AbilityFamily Family = AbilityFamily.Molis;

    public NPCAbility_RykerPayback(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(250, 0, Constants.DamageType.Magic, "AoE 1"));
        DamageSources.Add(new DamageSource(500, 0, Constants.DamageType.Magic, "AoE 2"));
        DamageSources.Add(new DamageSource(900, 0, Constants.DamageType.Magic, "AoE 3"));
        DamageSources.Add(new DamageSource(2500, 0, Constants.DamageType.Magic, "AoE 4"));
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        WaitTimeBeforeNextAction = 0.5f;
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        _aoe = Utils.CreateAreaOfEffect(new(this), "Ryker_Payback_Explosion").transform.parent.parent.parent.gameObject;
        _aoe.transform.SetParent(User.SpriteRenderers["Upper Body"].Bone, false);
        _aoe.transform.localPosition = Vector3.zero;
        Effect_RykerPayback effect = (Effect_RykerPayback)User.GetEffect(typeof(Effect_RykerPayback));
        if(effect.ExplosionGauge < 25)
        {
            _aoe.GetComponentInChildren<AreaOfEffect>(true).gameObject.name = "AoE 1";
            User.Animator.speed = 1;
            UpdateExplosionVFX(0.5f, 1, 2, 1, 2);
        }
        else if (effect.ExplosionGauge < 50)
        {
            _aoe.GetComponentInChildren<AreaOfEffect>(true).gameObject.name = "AoE 2";
            User.Animator.speed = 1.5f;
            UpdateExplosionVFX(0.8f, 1.5f, 1.5f, 2, 3);
        }
        else if (effect.ExplosionGauge < 75)
        {
            _aoe.GetComponentInChildren<AreaOfEffect>(true).gameObject.name = "AoE 3";
            User.Animator.speed = 2;
            UpdateExplosionVFX(1.1f, 2, 1f, 3, 4);
        }
        else
        {
            _aoe.GetComponentInChildren<AreaOfEffect>(true).gameObject.name = "AoE 4";
            User.PlayAnimation("RykerPayback_Instant");
            UpdateExplosionVFX(3, 4, 0.5f, 5, 5);
        }
        effect.ExplosionGauge = 0;
        effect.UpdateGlowVFX();
    }

    private void UpdateExplosionVFX(float glow_size, float speed, float explosion_delay, float ring_size, float aoe_size)
    {
        _aoe.GetComponentInChildren<CircleCollider2D>(true).radius = aoe_size;
        ParticleSystem.MainModule main = _aoe.GetComponent<ParticleSystem>().main;
        main.simulationSpeed = speed;
        main.startSize = glow_size;
        ParticleSystem.MainModule main2 = _aoe.transform.Find("Impact").GetComponent<ParticleSystem>().main;
        main2.startDelay = explosion_delay - 0.1f;
        ParticleSystem.MainModule main3 = _aoe.transform.Find("Impact/ImpactShape").GetComponent<ParticleSystem>().main;
        main3.startDelay = explosion_delay - 0.1f;
        ParticleSystem.MainModule main4 = _aoe.transform.Find("Impact/ImpactRing").GetComponent<ParticleSystem>().main;
        _aoe.transform.Find("Impact/ImpactRing").GetComponent<SetChildActiveAfterNSeconds>().Seconds = explosion_delay;
        _aoe.transform.GetComponentInChildren<AreaOfEffect>(true).SourceAbility = this;
        _aoe.transform.GetComponentInChildren<SetChildActiveAfterNSeconds>().PathToChild = _aoe.transform.GetComponentInChildren<AreaOfEffect>(true).gameObject.name;
        _aoe.transform.GetComponentInChildren<CircleCollider2D>(true).radius = ring_size * 2;
        main4.startDelay = explosion_delay + 0.1f;
        main4.startSize = ring_size;
        ParticleSystem.MainModule main5 = _aoe.transform.Find("Impact/RandomBrokenPieces").GetComponent<ParticleSystem>().main;
        main5.startDelay = explosion_delay - 0.1f;
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        User.Animator.speed = 1;
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        Effect_RykerPayback effect = (Effect_RykerPayback)user.GetEffect(typeof(Effect_RykerPayback));
        if(effect == null)
        {
            return false;
        }
        return effect.ExplosionGauge > 10;
    }

    public static void AdditionalActionsOnSettingsAbilityAsPotentialAction(Unit user) {
        user.AddEffect(new Effect_RykerPayback(new("Passive")));
    }
}