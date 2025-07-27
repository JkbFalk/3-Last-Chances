using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_SnowAvalanche : Ability {
    private GameObject _iceWave;
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Glacies;
    private Projectile _projectile;

    public NPCAbility_SnowAvalanche(Unit ability_user) : base(ability_user)
    {
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(30, 50, Constants.DamageType.Ranged) { KnockbackInMeters = 1.5f });
        AddCustomSound("Use", "Ice/Ice_Use2", 0.15f);
        HitSoundType = Constants.HitSoundTypeEnum.Ice;
        HitSoundVolume = 0.3f;
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void CallAbilityEvent1()
    {
        _iceWave = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Projectile/Projectile_Criminal_FreezeCaster_Avalanche")) as GameObject;
        _iceWave.GetComponent<ParticleSystem>().Stop();
        GameController.Instance.WaitAndRunMethod(0.1f, PlayIceWaveAnimation);
        GameController.Instance.WaitAndRunMethod(1f, CallAbilityEvent2);
        _projectile = _iceWave.GetComponent<Projectile>();
        float horizontalAdjustment = User.Actions.IsFlipped ? 0.25f : -0.25f;
        _projectile.transform.position = User.transform.position + new Vector3(horizontalAdjustment, 0);
        _projectile.SourceAbility = this;
        _projectile.Target = User.CurrentTarget;
        _projectile.transform.up = Utils.GetDirectionVector(User.transform.position, _projectile.Target.transform.position, User.Actions.IsFlipped, 30);
        _projectile.enabled = false;
        EventManager.OneTenthSecondElapsedInGame.AddListener(ResetTargets);
        GameController.Instance.WaitAndRunMethod(2, new System.Action(() => { EventManager.OneTenthSecondElapsedInGame.RemoveListener(ResetTargets); }));
    }
    
    public void ResetTargets() {
        if(_projectile != null && _projectile.gameObject != null && _projectile.gameObject.IsDestroyed() == false) {
            ResetPotentialTargets();
        }
    }

    public override void CallAbilityEvent2()
    {
        if (_iceWave != null && _iceWave.IsDestroyed() == false)
        {
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
        damage.TargetOfDamage.AddEffect(new Effect_Slow(10, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_Freeze(10, new(this)));
    }
}