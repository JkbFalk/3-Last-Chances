// FILE: Assets\Scripts\Ability\Technique\Ability_TempestStrikes.cs
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Ability_TempestStrikes : Technique
{
    public static float EnergyCost = 1;
    public static AbilityFamily Family = AbilityFamily.Anima;

    // Base Values
    public static float InjuryScaling = 250;
    public static float StaggerBarReductionPercentage = 2;
    public static float MaxStaggerBarReductionPercentage = 60f;
    public static float ExtraInjuryToStaggeredScaling = 250;

    // Mastery A (NEW)
    public static float MasteryAStaggerPerSharpScaling = 150f;
    public static float MasteryAFiveSharpBonusStaggerScaling = 800f;

    // Mastery B
    public static float MasteryBExtraStaggerScaling = 600f;

    // Ultimate
    public static int UltimateMaxStrikes = 20;
    public static float UltimateSpeedIncreasePerStrike = 5;
    public static float UltimateMaxSpeedIncrease = 100;
    public static float UltimateExtraDamagePerUniqueParryScaling = 25;
    public static List<Type> UniqueParriedAttacks = new List<Type>();

    private int _slashCounter = 0;
    private int _mostRecentAttack = 1;
    private int _sharpConsumedThisCast = 0; // Tracks Sharp consumed in a single uninterrupted flurry
    private List<GameObject> _weaponVfxs = new List<GameObject>();

    public Ability_TempestStrikes(Unit ability_user) : base(ability_user)
    {
        DamageSources.Add(new DamageSource(InjuryScaling, 0, User.CurrentWeaponDamageType));

        AddCustomSound("Random1", "Ability/Ability_ThreefoldDance_1", 0.7f);
        AddCustomSound("Random2", "Ability/Ability_ThreefoldDance_2", 0.7f);
        AddCustomSound("Random3", "Ability/Ability_ThreefoldDance_3", 0.7f);
        AddCustomSound("Random4", "Ability/Ability_ThreefoldDance_4", 0.7f);

        TransitionIntoAnimationDuration = 0f;
        _mostRecentAttack = UnityEngine.Random.Range(1, 6);
        NameOfAnimationToAutoPlay = "TempestStrikes_" + User.CurrentWeaponClass + _mostRecentAttack;

        if (User.CurrentWeaponType == Constants.ItemType.Ranged) {
            DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        }

        EffectsAffectingUserDuringAbility = new List<Effect>();
        
        if (Is(Property.UpgradeB)) {
            EffectsAffectingUserDuringAbility.Add(new Effect_TempestStrikesAutoCounter(new SourceOfEffect(this)));
        }
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { 
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * InjuryScaling / 100f), 
            Utils.GetFormattedFloat(InjuryScaling), 
            Utils.GetFormattedFloat(StaggerBarReductionPercentage),
            Utils.GetFormattedFloat(MaxStaggerBarReductionPercentage),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * ExtraInjuryToStaggeredScaling / 100f),
            Utils.GetFormattedFloat(ExtraInjuryToStaggeredScaling)
        };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { 
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * MasteryAStaggerPerSharpScaling / 100f),
            Utils.GetFormattedFloat(MasteryAStaggerPerSharpScaling),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * MasteryAFiveSharpBonusStaggerScaling / 100f),
            Utils.GetFormattedFloat(MasteryAFiveSharpBonusStaggerScaling)
        };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { 
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * MasteryBExtraStaggerScaling / 100f),
            Utils.GetFormattedFloat(MasteryBExtraStaggerScaling)
        };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { 
            UltimateMaxStrikes.ToString(),
            Utils.GetFormattedFloat(UltimateSpeedIncreasePerStrike),
            Utils.GetFormattedFloat(UltimateMaxSpeedIncrease),
            Utils.GetFormattedFloat(Player.Instance.CurrentWeaponStagger.Current * UltimateExtraDamagePerUniqueParryScaling / 100f),
            Utils.GetFormattedFloat(UltimateExtraDamagePerUniqueParryScaling)
        };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        ConsumeEnergyAndCooldownForTheAbility();
        _slashCounter = 0;
        _sharpConsumedThisCast = 0; 
        
        if (Is(Property.Ultimate)) {
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Ability_Quickdraw_Counter", 0.9f);
        }

        string suffix = Is(Property.Ultimate) ? "Ultimate" : "";
        
        if (User.CurrentWeaponDamageType == Constants.DamageType.Heavy)
        {
            AttachWeaponVFX("TempestStrikes_WeaponHeavy" + suffix, "Heavy");
        }
        else if (User.CurrentWeaponDamageType == Constants.DamageType.Light)
        {
            AttachWeaponVFX("TempestStrikes_WeaponLight" + suffix, "Light Right");
            AttachWeaponVFX("TempestStrikes_WeaponLight" + suffix, "Light Left");
        }
        else if (User.CurrentWeaponDamageType == Constants.DamageType.Ranged)
        {
            AttachWeaponVFX("TempestStrikes_WeaponRanged" + suffix, "Ranged");
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        User.Actions.TurnOffWeaponCollision("Heavy");
        User.Animator.SetFloat("Technique Speed", 1f);

        // --- NEW: Clean up Weapon VFX ---
        foreach(GameObject vfx in _weaponVfxs)
        {
            if (vfx != null)
            {
                TemporaryObject tempObj = vfx.GetComponent<TemporaryObject>();
                if (tempObj != null) 
                {
                    // Fades out smoothly over 0.25 seconds
                    tempObj.MakeObjectDisappear(0.25f); 
                } 
                else 
                {
                    MonoBehaviour.Destroy(vfx);
                }
            }
        }
        _weaponVfxs.Clear();
        // --------------------------------
    }

    public override void CallAbilityEvent1()
    {
        CanAlwaysBeInterruptedBy.Clear();
        _slashCounter++;

        bool canContinue = false;

        // Evaluate continuation based on Ultimate vs Standard
        if (Is(Property.Ultimate)) 
        {
            canContinue = _slashCounter < UltimateMaxStrikes && HoldingTechniqueButton;
        } 
        else 
        {
            canContinue = HoldingTechniqueButton && Player.Instance.Energy.Current >= EnergyCost;
        }

        if (canContinue) 
        {
            // Standard version needs to pay the energy cost for each subsequent strike
            if (!Is(Property.Ultimate)) 
            {
                ConsumeEnergyAndCooldownForTheAbility();
            }

            float speedBonus = 0f;
            if (Is(Property.Ultimate)) 
            {
                speedBonus = Mathf.Min(_slashCounter * UltimateSpeedIncreasePerStrike, UltimateMaxSpeedIncrease);
            }
            
            User.Animator.SetFloat("Technique Speed", 1f + (speedBonus / 100f));
            _mostRecentAttack = GetNextAnimationNumber();
            User.PlayAnimation("TempestStrikes_" + User.CurrentWeaponClass + _mostRecentAttack, 0.04f);
        }
    }

    public override void CallAbilityEvent2()
    {
        PlayCustomSound("Random" + UnityEngine.Random.Range(1, 5));
    } 

    public int GetNextAnimationNumber() 
    {
        switch(_mostRecentAttack) {
            case 1: return new List<int> {3, 4, 5, 4, 5}[UnityEngine.Random.Range(0, 5)];
            case 2: return new List<int> {3, 4, 5, 4, 5}[UnityEngine.Random.Range(0, 5)];
            case 3: return new List<int> {1, 2, 4, 5}[UnityEngine.Random.Range(0, 4)];
            case 4: return new List<int> {1, 2, 1, 2, 3}[UnityEngine.Random.Range(0, 5)];
            case 5: return new List<int> {1, 2, 1, 2, 3}[UnityEngine.Random.Range(0, 5)];
            default: return 3;
        }
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        base.ExtraBehaviourOnHit(damage);
        
        // Base: Reduce Max Stagger Bar using existing Effect_ChangeStat
        Effect_ChangeStat sbDebuff = (Effect_ChangeStat)damage.TargetOfDamage.GetEffectWithGivenId("TempestStrikes_SBDebuff");
        if (sbDebuff != null) 
        {
            // Calculate current reduction (stored as a negative number, e.g., -2, -4)
            float currentReduction = Mathf.Abs(sbDebuff.PercentageAmount);
            
            // Only increase the debuff if we haven't hit the 60% cap
            if (currentReduction < MaxStaggerBarReductionPercentage)
            {
                // Clamp the new reduction so it doesn't overshoot 60%
                float newReduction = Mathf.Min(currentReduction + StaggerBarReductionPercentage, MaxStaggerBarReductionPercentage);
                
                sbDebuff.PercentageAmount = -newReduction; 
                sbDebuff.UIText = Utils.GetFormattedFloat(newReduction, 0) + "%";
            }
        }
        else 
        {
            sbDebuff = new Effect_ChangeStat(damage.TargetOfDamage.StaggerBar, new SourceOfEffect(this)) 
            {
                Id = "TempestStrikes_SBDebuff",
                PercentageAmount = -StaggerBarReductionPercentage,
                Type = Effect.EffectType.Debuff,
                ShowsInUI = true,
                PathToUIGraphic = "Ability/TempestStrikes",
                PersistsBetweenPhaseChanges = true
            };
            // Adding effect with duration 0 makes it infinite
            damage.TargetOfDamage.AddEffect(sbDebuff, 0);
            sbDebuff.UIText = Utils.GetFormattedFloat(StaggerBarReductionPercentage, 0) + "%";
        }

        // ... [Rest of ExtraBehaviourOnHit logic remains the same] ...

        // Base: Deal extra Injury to Staggered enemies
        if (damage.TargetOfDamage.IsStaggered) {
            damage.InjuryDealtPercentageModifier += ExtraInjuryToStaggeredScaling;
        }

        // Mastery A: Consume [Sharp] for massive Stagger bursts
        if (Is(Property.UpgradeA)) 
        {
            Effect_Sharp playerSharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
            if (playerSharp != null && playerSharp.CurrentStacks > 0)
            {
                playerSharp.CurrentStacks -= 1;
                _sharpConsumedThisCast++;

                // Add the base Stagger bonus
                damage.StaggerDealtPercentageModifier += MasteryAStaggerPerSharpScaling;

                // Cash Out: Massive bonus on the 5th Sharp stack consumed in a row
                if (_sharpConsumedThisCast % 5 == 0)
                {
                    damage.StaggerDealtPercentageModifier += MasteryAFiveSharpBonusStaggerScaling;
                    
                    Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/LongSharp_CriticalHit1", 1.2f);
                    CameraController.Instance.ShakeScreen(0.3f, 0.15f);
                    Utils.CreateVisualEffect(new SourceOfEffect(this), "PowerfulHit", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
                }
                else
                {
                    Utils.PlaySoundEffect(Player.Instance.AudioSource, "Hit/LongSharp_Hit1", 0.8f);
                }

                if (playerSharp.CurrentStacks <= 0)
                {
                    playerSharp.EndThisEffect();
                }
            }
        }

        // Ultimate: Extra Damage based on unique parried/countered attacks this combat
        if (Is(Property.Ultimate)) {
            float bonus = UniqueParriedAttacks.Count * UltimateExtraDamagePerUniqueParryScaling;
            damage.DamageDealtPercentageModifier += bonus;
        }
    }

    public static void OnEquip()
    {
        EventManager.AbilityWasRipostedOrCountered.AddListener(OnParryOrCounter);
        EventManager.EnterCombat.AddListener(OnEnterCombat);
    }

    public static void OnUnequip()
    {
        EventManager.AbilityWasRipostedOrCountered.RemoveListener(OnParryOrCounter);
        EventManager.EnterCombat.RemoveListener(OnEnterCombat);
    }

    public static void OnEnterCombat(Unit unit)
    {
        if (unit is Player) {
            UniqueParriedAttacks.Clear();
        }
    }

    public static void OnParryOrCounter(Ability ability, bool wasCountered)
    {
        if (ability == null) return;
        
        Type attackType = ability.GetType();
        if (!UniqueParriedAttacks.Contains(attackType)) {
            UniqueParriedAttacks.Add(attackType);
        }
    }

    private void AttachWeaponVFX(string vfxName, string boneName)
    {
        if (!User.SpriteRenderers.ContainsKey(boneName)) return;
        
        // Note: Utils.CreateVisualEffect automatically prepends "VisualEffect_"
        GameObject vfx = Utils.CreateVisualEffect(new SourceOfEffect(this), vfxName);
        if (vfx != null)
        {
            vfx.transform.SetParent(User.SpriteRenderers[boneName].Bone);
            vfx.transform.localPosition = Vector2.zero;
            vfx.transform.localRotation = Quaternion.identity;
            _weaponVfxs.Add(vfx);
        }
    }
}