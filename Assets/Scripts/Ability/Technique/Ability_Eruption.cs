using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_Eruption : Technique
{
    public static float EnergyCost = 80;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private GameObject _aoe;
    private int _ultimateExplosionCounter = 0;
    private int _circleCounter = 1;
    public static float HeavyInjuryScaling = 300;
    public static float MagicInjuryScaling = 300;
    public static float HeavyStaggerScaling = 400;
    public static float MagicStaggerBurnScaling = 50;
    public static float HeavyInjuryScalingUltimate = 300;
    public static float MagicStaggerBurnScalingUltimate = 35;
    public static float MasteryBBarrierScaling = 100;
    public static float MasteryAProneApplied = 20;


    public Ability_Eruption(Unit ability_user) : base(ability_user)
    {
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        AddCustomSound("Explosion", "Fire/FireExplosion2", 0.7f);
        AddCustomSound("Use", "Greatsword/StabIntoGround", 0.5f);
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        if (Player.Instance.PreparingForUltimate)
        {
            DamageSources.Add(new DamageSource(HeavyInjuryScalingUltimate, 0, Constants.DamageType.Heavy, "AoE Ultimate") {KnockbackInMeters = 1.2f});
        }
        else
        {
            DamageSources.Add(new DamageSource(new Dictionary<Constants.DamageType, float>() {{ Constants.DamageType.Heavy, HeavyInjuryScaling },{ Constants.DamageType.Magic, MagicInjuryScaling }}, new Dictionary<Constants.DamageType, float>() {{ Constants.DamageType.Heavy, HeavyStaggerScaling }}, Constants.DamageType.Heavy, "AoE") {KnockbackInMeters = 1.8f});
        }
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)), new Effect_Unstunnable(new(this))};
    }

    public override void CallAbilityEvent1()
    {
        if(IsNot(Property.Ultimate)) {
            _aoe = Utils.CreateAreaOfEffect(new(this), "EruptionCircle").transform.parent.parent.gameObject;
            _aoe.transform.position = User.transform.position + new Vector3(0, -0.2f);
            GameController.Instance.WaitAndRunMethod(Is(Property.UpgradeA) ? 0.5f : 1, AdvanceExplosion);
            GameController.Instance.WaitAndRunMethod(1.2f, PlaySound);
        }
    }

    public override void CallAbilityEvent2()
    {
        if(Is(Property.Ultimate)) {
            CreateExplosion();
        }
    }

    public void PlaySound() {
        PlayCustomSound("Explosion", 0.7f, _aoe.GetComponent<AudioSource>());
    }

    public void AdvanceExplosion() {
        if(_circleCounter < 4) {
            _circleCounter++;
            _aoe.transform.Find("Circle" + _circleCounter).gameObject.SetActive(true);
            if(_circleCounter < 5) {
                GameController.Instance.WaitAndRunMethod(1.2f, PlaySound);
            }
            GameController.Instance.WaitAndRunMethod(Is(Property.UpgradeA) ? 0.5f : 1 , AdvanceExplosion);
        }
    }

    public void CreateExplosion() {
        if(_ultimateExplosionCounter < 200) {
            _ultimateExplosionCounter++;
            AreaOfEffect aoe  = Utils.CreateAreaOfEffect(new(this), "Eruption", User.transform.position.x + UnityEngine.Random.Range(-2.5f, 2.5f), User.transform.position.y + UnityEngine.Random.Range(-2.5f, 2.5f));
            GameController.Instance.WaitAndRunMethod(0.1f, CreateExplosion);
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> {((Player.Instance.HeavyInjury.Current * HeavyInjuryScaling / 100) + (Player.Instance.MagicInjury.Current * MagicInjuryScaling / 100)).ToString(), HeavyInjuryScaling.ToString(), MagicInjuryScaling.ToString(), (Player.Instance.HeavyStagger.Current * HeavyStaggerScaling / 100).ToString(), HeavyStaggerScaling.ToString(), (Player.Instance.MagicStagger.Current * MagicStaggerBurnScaling / 100).ToString(), MagicStaggerBurnScaling.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> {(Player.Instance.HeavyInjury.Current * HeavyInjuryScalingUltimate / 100).ToString(), HeavyInjuryScalingUltimate.ToString(), (Player.Instance.MagicStagger.Current * MagicStaggerBurnScalingUltimate / 100).ToString(), MagicStaggerBurnScalingUltimate.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> {MasteryAProneApplied.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> {((Player.Instance.MagicStagger.Current + Player.Instance.HeavyStagger.Current) * MasteryBBarrierScaling / 100).ToString(), MasteryBBarrierScaling.ToString() };
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        if (Is(Property.Ultimate)) {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(Player.Instance.MagicStagger.Current * MagicStaggerBurnScalingUltimate / 100, new(this)));
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(Player.Instance.MagicStagger.Current * MagicStaggerBurnScaling / 100, new(this)));
        }
        if(Is(Property.UpgradeA)) {
            damage.TargetOfDamage.AddEffect(new Effect_Prone(MasteryAProneApplied, new(this)));
        }
        if(Is(Property.UpgradeB)) {
            User.AddEffect(new Effect_Barrier((Player.Instance.MagicStagger.Current + Player.Instance.HeavyStagger.Current) * MasteryBBarrierScaling / 100, new(this)));
        }
    }
}