using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Ability_SentientShadow : Technique
{
    public static float EnergyCost = 15;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    private Projectile _hook;
    private GameObject _tether;
    private static float _rangeInMeters = 10;
    private static float _stunDuration = 3;
    private static float _upgradeAOnslaughtApplied = 35;
    private static float _upgradeBCooldown = 30;
    private static float _upgradeBRangeInMeters = 15;
    private static float _upgradeBSpeedPercentage = 150;
    private static float _ultimateInjuryScaling = 200;
    private static float _ultimateStunDuration = 6;

    public static bool CanBeUsedDuringOtherAbilities
    {
        get
        {
            return true;
        }
    }

    public Ability_SentientShadow(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("RopeExtend", "Ability/RopeExtend", 0.8f);
        AddCustomSound("UltimateHit", "Blade/Blade_BloodStab2", 0.6f);
        DamageSources.Add(new DamageSource(0, 1, Constants.DamageType.None));
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _rangeInMeters.ToString(), _stunDuration.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeAOnslaughtApplied.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { _upgradeBCooldown.ToString(), (_upgradeBSpeedPercentage - 100).ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * _ultimateInjuryScaling / 100), _ultimateStunDuration.ToString() };
    }

    public override void ActionsToPerformDuringAnotherAbility()
    {
        if (Player.Instance.Energy.Current < EnergyCost || Player.Instance.TechniqueCooldowns.FirstOrDefault(cooldown => cooldown.Type == GetType()) != null || (_hook != null && _hook.gameObject!= null))
        {
            return;
        }
        if (Is(Property.Ultimate))
        {
            PerformUltimate();
        }
        else
        {
            PerformHook();
        }
    }

    private void PerformHook()
    {
        Vector2 hookDirection = User.Actions.GetCurrentAimVector();
        bool cannotBeStunned = Player.Instance.CurrentTarget != null && (Player.Instance.CurrentTarget.CheckIfUnderEffect(typeof(Effect_Unstunnable)) || (Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed != null && Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed.Is(Property.Unstoppable)));
        if (Is(Property.UpgradeA) && Player.Instance.CurrentTarget != null && cannotBeStunned == false)
        {
            hookDirection = Player.Instance.CurrentTarget.transform.position - Player.Instance.transform.position;
        }
        else if (Is(Property.UpgradeA))
        {
            if (cannotBeStunned == false)
            {
                return;
            }
            Vector3 directionTowardsTarget = (Player.Instance.CurrentTarget.transform.position - Player.Instance.transform.position).normalized;
            Vector3 preferredEscapePosition = Player.Instance.CurrentTarget.transform.position.x > Player.Instance.transform.position.x ? (Player.Instance.transform.position + Vector3.left * 6) : (Player.Instance.transform.position + Vector3.right * 6);

            List<Vector2> pointsWithinCircle = new List<Vector2>();
            for (int i = 0; i < 100; i++)
            {
                pointsWithinCircle.Add(Random.insideUnitCircle * 3);
            }

            List<Vector3> potentialHits = new List<Vector3>();
            foreach (Vector3 pointWithinCircle in pointsWithinCircle)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(preferredEscapePosition + pointWithinCircle, Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    GameObject hitGameObject = hit.collider.gameObject;
                    if (hitGameObject.GetComponent<DestructibleEnvironment>() != null || hitGameObject.GetComponent<TilemapCollider2D>() != null)
                    {
                        potentialHits.Add(preferredEscapePosition + pointWithinCircle);
                    }
                }
            }
            if (potentialHits.Count() == 0)
            {
                return;
            }
            else
            {
                hookDirection = potentialHits.OrderBy(hit => Vector2.Distance(preferredEscapePosition, hit)).FirstOrDefault() - Player.Instance.transform.position;
            }
        }
        PlayCustomSound("RopeExtend");
        _hook = Utils.CreateProjectile(new(this), "SentientShadows", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        ConsumeEnergyAndCooldownForTheAbility();
        _hook.transform.up = hookDirection;
        _hook.GetComponent<Rigidbody2D>().AddForce(_hook.transform.up * (Is(Property.UpgradeB) ? _upgradeBRangeInMeters : _rangeInMeters) * Constants.FORCE_REQUIRED_TO_PUSH_1M, ForceMode2D.Force);
        _hook.FlightSpeed = Is(Property.UpgradeB) ? _hook.FlightSpeed * _upgradeBSpeedPercentage / 100 : _hook.FlightSpeed;
        _tether = Utils.CreateVisualEffect(new(this), "SentientShadowsTether", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        EventManager.OneFrameElapsedInGame.AddListener(UpdateHookTether);
        GameController.Instance.WaitAndRunMethod(0.75f, DisableTether);
    }

    private void DisableTether()
    {
        if (_tether != null && _tether!= null)
        {
            _tether.GetComponent<ParticleSystem>().Stop();
        }
        if (_hook != null && _hook.gameObject!= null)
        {
            _hook.MakeObjectDisappear(0.25f);
            GameController.Instance.WaitAndRunMethod(0.25f, MakeHookDisappear);
        }
    }

    private void UpdateHookTether()
    {
        if (_tether == null || _tether== null || _hook == null || _hook.gameObject== null)
        {
            return;
        }
        _tether.transform.position = new Vector2((_hook.transform.position.x + Player.Instance.transform.position.x) / 2, (_hook.transform.position.y + Player.Instance.transform.position.y) / 2);
        ParticleSystem ps = _tether.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule psShape = ps.shape;
        psShape.radius = Vector2.Distance(_hook.transform.position, Player.Instance.transform.position) / 2 - 0.15f;
        ParticleSystem.EmissionModule psEmission = ps.emission;
        psEmission.rateOverTime = 2000 + Vector2.Distance(_hook.transform.position, Player.Instance.transform.position) * 1000;
        Vector2 direction = _hook.transform.position - Player.Instance.transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        _tether.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void HandleEnvironmentCollision()
    {
        GameObject vfx = Utils.CreateVisualEffect(new(_hook.SourceAbility), "WallHit", _hook.transform.position.x, _hook.transform.position.y);
        vfx.transform.up = _hook.transform.up * -1;
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/WallHit" + UnityEngine.Random.Range(1, 4), 0.25f);
        if (Is(Property.UpgradeB) && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Id == "SentientShadow_UpgradeB") == null)
        {
            Player.Instance.AddCooldown(new Cooldown(typeof(Effect_Id), _upgradeBCooldown, Player.Instance, "SentientShadow_UpgradeB") { PathToCooldownGraphic = "Ability/SentientShadow", ShowsInUI = true });
            Player.Instance.AddEffect(new Effect_Id("SentientShadow_UpgradeB", new(this)) { ShowsInUI = true, PathToUIGraphic = "Ability/SentientShadow" }, 5);
            Cooldown cd = Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == GetType());
            cd.EndThisCooldown();
        }
        else if (Is(Property.UpgradeB) && Player.Instance.GetEffect(new System.Func<Effect, bool>(effect => effect.Id == "SentientShadow_UpgradeB")) != null)
        {
            Effect e = Player.Instance.GetEffect(new System.Func<Effect, bool>(effect => effect.Id == "SentientShadow_UpgradeB"));
            e.EndThisEffect();
            Cooldown cd = Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == GetType());
            cd.EndThisCooldown();
        }
        HandleHookHit();
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), _stunDuration);
        HandleHookHit();
        if (Is(Property.UpgradeA))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Onslaught(_upgradeAOnslaughtApplied, new(this)));
        }
    }

    public void HandleHookHit()
    {
        _hook.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Utils.PlaySoundEffect(_hook.GetComponent<AudioSource>(), "Hit/Rock_Hit3");
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/RopePull");
        Player.Instance.PushIntoPositionOverTime(_hook.transform.position, _hook.SourceAbility, Is(Property.UpgradeB) ? 0.4f / (_upgradeBSpeedPercentage / 100) : 0.4f, Is(Property.UpgradeB) ? 1.75f : 1.25f);
        Player.Instance.CannotMoveDueToOwnTechnique = true;
        GameController.Instance.WaitAndRunMethod(0.25f, DisableTether);
        GameController.Instance.WaitAndRunMethod(0.4f, ReEnablePlayerMovement);
    }

    public void ReEnablePlayerMovement()
    {
        Player.Instance.CannotMoveDueToOwnTechnique = false;
    }

    public void MakeHookDisappear()
    {
        if (_hook != null && _hook.gameObject!= null)
        {
            MonoBehaviour.Destroy(_hook.gameObject);
            _hook = null;
            MonoBehaviour.Destroy(_tether);
            _tether = null;
        }
        EventManager.OneFrameElapsedInGame.RemoveListener(UpdateHookTether);
    }

    private void PerformUltimate()
    {
        Target = Player.Instance.CurrentTarget != null ? Player.Instance.CurrentTarget : Player.Instance.GetClosestValidTarget(false);
        if (Target == null)
        {
            return;
        }
        ConsumeEnergyAndCooldownForTheAbility();
        GameObject vfx = Utils.CreateVisualEffect(new(this), "SentientShadow_Ultimate");
        vfx.transform.SetParent(Target.transform);
        vfx.transform.localPosition = Vector2.zero;
        if (Target.Actions.CurrentAbilityBeingPerformed != null && Target.Actions.CurrentAbilityBeingPerformed.Is(Property.Unstoppable))
        {
            Effect e = Target.GetEffect(typeof(Effect_Unstunnable));
            if (e != null)
            {
                e.EndThisEffect();
            }
        }
        for (int i = 0; i < 5; i++)
        {
            GameController.Instance.WaitAndRunMethod(0.25f + 0.1f * i, UltimateHit);
        }
    }

    private void UltimateHit()
    {
        PlayCustomSound("UltimateHit");
        new DamageInstance(Target, this, null) { AbilityDamageSource = new DamageSource(_ultimateInjuryScaling / 5, 0, Constants.DamageType.CurrentWeapon) }.CalculateAndApplyDamage();
        Target.AddEffect(new Effect_Stun(new(this)), _stunDuration / 5);
    }
}