using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_TempestStrikes : Technique
{
    private GameObject _vfx;
    public static float EnergyCost = 10;
    public static float Cooldown = 1;
    private int _slashCounter = 0;
    public static float InjuryScaling = 200;
    public static float StaggerScaling = 200;
    public static float UltimateSpeedIncrease = 5;
    public static float UltimateAnalysisApplied = 6;
    public static bool IsStacksBasedTechnique = true;
    public static int MaxStacks {
        get {
            return SaveFile.Instance.ActiveUpgrades.Contains("Ability_TempestStrikes_UpgradeA") ? 10 : 5;
        }
    }
    public static int UltimateMaxStacks {
        get {
            return 20;
        }
    }
    private int _mostRecentAttack = 1;

    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.CurrentWeapon;
    public Ability_TempestStrikes(Unit ability_user) : base(ability_user)
    {
        DamageSources.Add(new DamageSource(InjuryScaling, StaggerScaling, User.CurrentWeaponDamageType));
        AddCustomSound("Random1", "Ability/Ability_ThreefoldDance_1", 0.7f);
        AddCustomSound("Random2", "Ability/Ability_ThreefoldDance_2", 0.7f);
        AddCustomSound("Random3", "Ability/Ability_ThreefoldDance_3", 0.7f);
        AddCustomSound("Random4", "Ability/Ability_ThreefoldDance_4", 0.7f);
        TransitionIntoAnimationDuration = 0f;
        EffectsAffectingUserDuringAbility = new();
        _mostRecentAttack = UnityEngine.Random.Range(1, 6);
        NameOfAnimationToAutoPlay = "TempestStrikes_" + User.CurrentWeaponClass + _mostRecentAttack;
        if(User.CurrentWeaponType == Constants.ItemType.Ranged) {
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { (Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100).ToString(), InjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100).ToString(), StaggerScaling.ToString(), EnergyCost.ToString(), Cooldown.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { (Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100).ToString(), InjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * StaggerScaling / 100).ToString(), StaggerScaling.ToString(), Cooldown.ToString(), UltimateSpeedIncrease.ToString(), UltimateAnalysisApplied.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> {};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> {};
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.ConsumeEnergyAndCooldownForTheAbility();
        if(Is(Property.Ultimate)) {
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Ability_Quickdraw_Counter", 0.9f);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityStart();
        MonoBehaviour.Destroy(_vfx);
        User.Actions.TurnOffWeaponCollision("Heavy");
        User.Animator.SetFloat("Technique Speed", 1f);
    }

    public override void CallAbilityEvent1()
    {
        CanAlwaysBeInterruptedBy.Clear();
        if(User.Energy.Current >= EnergyCost && Player.Instance.CurrentTechniqueStacks[typeof(Ability_TempestStrikes)] > 0 && (_slashCounter < 4 || (Is(Property.UpgradeA) && _slashCounter < 9) || (Is(Property.Ultimate) && _slashCounter < 19))) {
            _slashCounter++;
            if(Is(Property.Ultimate)) {
                User.Animator.SetFloat("Technique Speed", 1 + _slashCounter * UltimateSpeedIncrease / 100);
            }
            if((Is(Property.UpgradeA) && _slashCounter == 9) || (Is(Property.Ultimate) && _slashCounter == 19)) {
                DamageSources.Clear();
                DamageSources.Add(new DamageSource(InjuryScaling * (Is(Property.Ultimate) ? 5 : 3), StaggerScaling * (Is(Property.Ultimate) ? 5 : 3), User.CurrentWeaponDamageType));
                User.Animator.SetFloat("Technique Speed", Is(Property.Ultimate) ? 0.4f : 0.25f);
            }
            User.Actions.ConsumeEnergyAndCooldownForTheAbility();
            _mostRecentAttack = GetNextAnimationNumber();
            User.PlayAnimation("TempestStrikes_" + User.CurrentWeaponClass + _mostRecentAttack, 0.04f);
        }
    }

    public override void CallAbilityEvent2()
    {
        PlayCustomSound("Random" + UnityEngine.Random.Range(1, 5));
        if(Is(Property.UpgradeA) && _slashCounter == 9) {
            User.Animator.SetFloat("Technique Speed", 1f);
        }
        else if(Is(Property.Ultimate) && _slashCounter == 19) {
            User.Animator.SetFloat("Technique Speed", 2f);
        }
    } 

    public int GetNextAnimationNumber() {
        switch(_mostRecentAttack) {
            case 1: return new List<int> {3, 4, 5, 4, 5}[UnityEngine.Random.Range(0, 5)];
            case 2: return new List<int> {3, 4, 5, 4, 5}[UnityEngine.Random.Range(0, 5)];
            case 3: return new List<int> {1, 2, 4, 5}[UnityEngine.Random.Range(0, 4)];
            case 4: return new List<int> {1, 2, 1, 2, 3}[UnityEngine.Random.Range(0, 5)];
            case 5: return new List<int> {1, 2, 1, 2, 3}[UnityEngine.Random.Range(0, 5)];
            default: return 3;
        }
    }

    public static void OnEquip()
    {
        EventManager.AbilityWasRipostedOrCountered.AddListener(AddStacks);
    }

    public static void OnUnequip()
    {
        EventManager.AbilityWasRipostedOrCountered.RemoveListener(AddStacks);
    }

    public static void AddStacks(Ability ability, bool was_countered) {
        if(SaveFile.Instance.ActiveUpgrades.Contains("Ability_TempestStrikes_UpgradeB")) {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_TempestStrikes), Player.Instance.CurrentTechniqueStacks[typeof(Ability_TempestStrikes)] + (was_countered ? 2 : 1));
        }
        Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_TempestStrikes), Player.Instance.CurrentTechniqueStacks[typeof(Ability_TempestStrikes)] + (was_countered ? 2 : 1), true);
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(Is(Property.Ultimate)) {
            User.AddEffect(new Effect_Analysis(UltimateAnalysisApplied, new(this)));
        }
    }
}
