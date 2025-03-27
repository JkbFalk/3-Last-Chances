using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_SnowAvalanche : Ability {
    private GameObject _iceWave;
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Glacies;

    public NPCAbility_SnowAvalanche(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(60, 120, Constants.DamageType.Ranged) {Knockback = 150});
        AddCustomSound("Use", "Ice/Ice_Use2", 0.15f);
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        HitSoundVolume = 0.3f;
        PerformActionAfterIntervals(20, 0.5f);
        Properties.Add(AbilityProperty.ImmuneToFlinch);
    }

    public override void CallAbilityEvent1() {
        _iceWave = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Projectile/Projectile_Criminal_FreezeCaster_Avalanche")) as GameObject;
        _iceWave.GetComponent<ParticleSystem>().Stop();
        GameController.Instance.WaitAndRunMethod(0.1f, PlayIceWaveAnimation);
        GameController.Instance.WaitAndRunMethod(1f, CallAbilityEvent2);
        Projectile projectile = _iceWave.GetComponent<Projectile>();
        float horizontalAdjustment = User.Actions.IsFlipped ? 0.25f : -0.25f;
        projectile.transform.position = User.transform.position + new Vector3(horizontalAdjustment, 0);
        projectile.SourceAbility = this;
        projectile.Target = User.CurrentTarget;
        projectile.transform.up = Utils.GetDirectionVector(User.transform.position, projectile.Target.transform.position, User.Actions.IsFlipped, 30);
        projectile.enabled = false;
    }

    public override void CallAbilityEvent2() {
        if (_iceWave != null && _iceWave.IsDestroyed() == false) {
            _iceWave.GetComponent<Projectile>().enabled = true;
            _iceWave.GetComponent<Projectile>().DealingDamage = true;
        }
    }

    public void PlayIceWaveAnimation() {
        if (_iceWave != null && _iceWave.IsDestroyed() == false) {
            _iceWave.GetComponent<ParticleSystem>().Play();
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage) {
        damage.TargetOfDamage.AddEffect(new Effect_Slow(35, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(35, new(this)));
    }

    public override void ActionToPerformAfterIntervals()
    {
        ResetPotentialTargets();
    }
}