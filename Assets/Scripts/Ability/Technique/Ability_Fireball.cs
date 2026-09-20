using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Fireball : Technique {

    public static float EnergyCost = 40;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public static float MagicInjuryScalingExplosion = 250;
    public static float MagicBurnScalingExplosion = 60;
    public static float MagicInjuryScalingMasteryB = 200;
    public static float MagicStaggerScalingMasteryB = 400;
    public static float MagicInjuryScalingUltimate = 1000;
    public static float MagicBurnScalingUltimate = 200;
    public static float UpgradeAStunMinDuration = 2;
    public static float UpgradeAStunMaxDuration = 5;
    private List<Projectile> _fireBalls = new();
    private List<Vector3> _intendedDestinations = new();
    private List<float> _distances = new();
    private int _counter = 0;
    private List<Unit> _enemiesAffectedByExplosion = new();

    public Ability_Fireball(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Fire/Fire2", 0.2f);
        AddCustomSound("Explosion", "Fire/Fire10", 0.6f);
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        NameOfAnimationToAutoPlay = Is(Property.UpgradeB) ? "Fireball_MasteryB" : Player.Instance.PreparingForUltimate ? "Fireball_Ultimate" : "Fireball";
        if(Is(Property.UpgradeB)) {
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
            DamageSources.Add(new DamageSource(MagicInjuryScalingMasteryB, MagicStaggerScalingMasteryB, Constants.DamageType.Magic, "Fireball") {KnockbackInMeters = 3f});
            DamageSources.Add(new DamageSource(MagicInjuryScalingExplosion, 0, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 1f});
        }
        else if(Player.Instance.PreparingForUltimate) {
            DamageSources.Add(new DamageSource(MagicInjuryScalingUltimate, MagicBurnScalingUltimate, Constants.DamageType.Magic, "Fireball_Ultimate") {KnockbackInMeters = 1.5f});
        }
        else {
            DamageSources.Add(new DamageSource(MagicInjuryScalingExplosion, 0, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 3f});
        }
        EffectsAffectingUserDuringAbility = new List<Effect> {new Effect_Unstunnable(new(this))};
    }

    public override void CallAbilityEvent1()
    {
        _fireBalls.Add(Utils.CreateProjectile(new(this), "Fireball"));
        _fireBalls[0].GetComponent<AttachObjectToBodyPart>().Initialize(User);
        _fireBalls[0].CleanUpAfter(6);
    }

    public override void CallAbilityEvent2()
    {
        ConsumeEnergyAndCooldownForTheAbility();
        if(Player.Instance.CurrentTarget != null) {
            _intendedDestinations.Add(Player.Instance.CurrentTarget.transform.position);
        }
        else {
            _intendedDestinations.Add(Settings.Instance.ControlScheme == "Keyboard" ? GameController.Instance.PlayerControls.CurrentWorldspacePointerPosition : new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y) + GameController.Instance.PlayerControls.CurrentLeftStickPosition * 3);
        }
        Vector3 relativePosition = _intendedDestinations[0] - Player.Instance.transform.position;
        if(User.Actions.IsFlipped && relativePosition.x > -3.5f) {
            relativePosition.x = -3.5f;
        }
        if(!User.Actions.IsFlipped && relativePosition.x < 3.5f) {
            relativePosition.x = 3.5f;
        }
        if((User.Actions.IsFlipped && relativePosition.x > 0) || (!User.Actions.IsFlipped && relativePosition.x < 0)) {
            relativePosition.x *= -1;
        }
        if(Math.Abs(relativePosition.y) > Math.Abs(relativePosition.x)) {
            float adjustment = Math.Abs(relativePosition.y) - Math.Abs(relativePosition.x);
            _intendedDestinations[0] = new Vector2(_intendedDestinations[0].x, _intendedDestinations[0].y + relativePosition.y > 0 ? -adjustment : adjustment);
        }
        if(Player.Instance.CurrentTarget == null) {
            _intendedDestinations[0] = Player.Instance.transform.position + relativePosition;
        }
        if(Is(Property.Ultimate)) {
            for(int i = 0; i < 4; i++) {
                _fireBalls.Add(Utils.CreateProjectile(new(this), "Fireball"));
                MonoBehaviour.Destroy(_fireBalls[i+1].GetComponent<AttachObjectToBodyPart>());
            }
            _intendedDestinations.Add(_intendedDestinations[0] + new Vector3(-3, -3));
            _intendedDestinations.Add(_intendedDestinations[0] + new Vector3(3, -3));
            _intendedDestinations.Add(_intendedDestinations[0] + new Vector3(-3, 3));
            _intendedDestinations.Add(_intendedDestinations[0] + new Vector3(3, 3));
        }
        for(int i = 0; i < _fireBalls.Count; i++) {
            _distances.Add(Vector2.Distance(_fireBalls[i].transform.position, _intendedDestinations[i]) / 2);
            _fireBalls[i].transform.SetParent(User.transform.parent);
            _fireBalls[i].IsFlying = true;
            _fireBalls[i].transform.eulerAngles = Vector3.zero;
            _fireBalls[i].DealingDamage = true;
            if(Is(Property.UpgradeB) == false) {
                _fireBalls[i].transform.up = (_intendedDestinations[i] + new Vector3(0, _distances[i]) - _fireBalls[i].transform.position).normalized;
                GameController.Instance.WaitAndRunMethod(0.3f / Player.Instance.MagicAttackSpeed.Current, Explode);
                AdjustFireballAngle(i);
            }
            else {
                if (User.CurrentTarget != null)
                {
                    _fireBalls[i].transform.up = CombatMath.GetDirectionVector(User.ProjectileSpawnLocation.transform.position, User.CurrentTarget.transform.position, User.Actions.IsFlipped, 45);
                }
                else
                {
                    _fireBalls[i].transform.up = CombatMath.GetDirectionVector(Vector2.zero, User.Actions.SavedAimDirection != Vector2.zero ? User.Actions.SavedAimDirection : User.Actions.GetCurrentAimVector(), User.Actions.IsFlipped, 45);
                }
            }
        }
    }

    public void Explode() {
        for(int i = 0; i < _fireBalls.Count; i++) {
            if(_fireBalls[i] != null && _fireBalls[i].gameObject != null && _fireBalls[i].gameObject.IsDestroyed() == false) {
                AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FireballExplosion" + (Is(Property.UpgradeA) ? "_MasteryA" : ""));
                PlayCustomSound("Explosion", 0.6f, aoe.transform.parent.GetComponent<AudioSource>());
                aoe.transform.parent.position = _fireBalls[i].transform.position;
                MonoBehaviour.Destroy(_fireBalls[i].gameObject);
            }
        }
    } 

    public void AdjustFireballAngle(int index) {
        if(_counter < 10 && _fireBalls[index] != null && _fireBalls[index].gameObject != null && _fireBalls[index].gameObject.IsDestroyed() == false) {
            if(index == 0) {
                _counter++;
            }
            _fireBalls[index].transform.up = (_intendedDestinations[index] + new Vector3(0, _distances[index] - _distances[index] * _counter / 5) - _fireBalls[index].transform.position).normalized;
            GameController.Instance.WaitAndRunMethod(0.03f / Player.Instance.MagicAttackSpeed.Current, AdjustFireballAngle, index);
        }
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        if(damage.DamagingObject.gameObject.name == "AoE") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(MagicBurnScalingExplosion * User.MagicStagger.Current / 100, new(this)));
        }
        else {
            AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "FireballExplosion" + (Is(Property.UpgradeA) ? "_MasteryA" : ""));
            PlayCustomSound("Explosion", 0.6f, aoe.transform.parent.GetComponent<AudioSource>());
            aoe.transform.parent.position = damage.DamagingObject.transform.position;
            damage.DamagingObject.MakeObjectDisappear(0);
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        if(Is(Property.UpgradeA)) {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), Utils.GetValueBasedOnMinAndMax(Vector2.Distance(damage.TargetOfDamage.transform.position, damage.DamagingObject.transform.position), 0, 3, UpgradeAStunMaxDuration, UpgradeAStunMinDuration));
        }
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(Is(Property.Ultimate) && _enemiesAffectedByExplosion.Contains(unit_getting_attacked)) {
            return;
        }
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> {(Player.Instance.MagicInjury.Current * MagicInjuryScalingExplosion / 100).ToString(), MagicInjuryScalingExplosion.ToString(), (Player.Instance.MagicStagger.Current * MagicBurnScalingExplosion / 100).ToString(), MagicBurnScalingExplosion.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> {UpgradeAStunMinDuration.ToString(), UpgradeAStunMaxDuration.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { (Player.Instance.MagicInjury.Current * MagicInjuryScalingMasteryB / 100).ToString(), MagicInjuryScalingMasteryB.ToString(), (Player.Instance.MagicStagger.Current * MagicStaggerScalingMasteryB / 100).ToString(), MagicStaggerScalingMasteryB.ToString()};
    }
}