using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Fortify : Technique
{
    public static float EnergyCost = 1;
    public static float Cooldown = 2;
    private Effect_ChangeStat _armorBuff;
    private Effect_Unstunnable _unstunnableBuff;
    private Effect_ChangeStat _upgradeAEnergyGain;
    private Effect_ChangeStat _upgradeAHealing;
    private static float _baseArmorGained = 150;
    private static float _percentageArmorGained = 150;
    private static float _upgradeAEnergyGainAmount = 200;
    private static float _upgradeAPercentageMissingHealthRestoredPerSecond = 15;
    private static float _upgradeBBarrierGainedPer1StackHealthScaling = 0.5f;
    private static float _upgradeBBarrierGainedPer1StackStaggerBarScaling = 0.5f;
    private static float _ultimatePercentageOfDamageReflected = 50f;
    public static AbilityFamily Family = AbilityFamily.Molis;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.None;
    public static bool IsStacksBasedTechnique = true;
    public static int MaxStacks
    {
        get
        {
            return 100;
        }
    }
    public static int UltimateMaxStacks
    {
        get
        {
            return 20;
        }
    }

    public Ability_Fortify(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("Use", "Earth/Earth_Crack5", 0.4f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { MaxStacks.ToString(), (_baseArmorGained + Player.Instance.Armor.Current * _percentageArmorGained / 100).ToString(), _baseArmorGained.ToString(), _percentageArmorGained.ToString()};
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeAEnergyGainAmount.ToString(), _upgradeAPercentageMissingHealthRestoredPerSecond.ToString()};
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { };
    }

    public override void CallAbilityEvent1()
    {
        Player.Instance.Actions.ConsumeEnergyAndCooldownForTheAbility();
        _armorBuff = new Effect_ChangeStat(Player.Instance.Armor, new(this))
        {
            FlatAmount = GetEffectiveArmor()
        };
        _unstunnableBuff = new Effect_Unstunnable(new(this));
        Player.Instance.AddEffect(_armorBuff);
        Player.Instance.AddEffect(_unstunnableBuff);
        EventManager.OneTenthSecondElapsedInGame.AddListener(Activate);
        if (Is(Property.UpgradeA))
        {
            RefreshUpgradeABuff(Player.Instance);
            EventManager.UnitHealthChanged.AddListener(RefreshUpgradeABuff);
        }
        if (Is(Property.UpgradeB))
        {
            float barrierAmount = Player.Instance.CurrentTechniqueStacks[typeof(Ability_Fortify)] * Player.Instance.Health.Maximum * _upgradeBBarrierGainedPer1StackHealthScaling / 100 + Player.Instance.CurrentTechniqueStacks[typeof(Ability_Fortify)] * Player.Instance.StaggerBar.Maximum * _upgradeBBarrierGainedPer1StackStaggerBarScaling / 100;
            Player.Instance.AddEffect(new Effect_Barrier(barrierAmount, new(this)));
        }
        if (Is(Property.Ultimate))
        {
            EventManager.DamageDealt.AddListener(ReflectDamageTaken);
        }
    }

    public void RefreshUpgradeABuff(Unit unit)
    {
        if (unit == Player.Instance)
        {
            if (_upgradeAEnergyGain != null && _upgradeAEnergyGain.EffectEnded == false)
            {
                _upgradeAEnergyGain.EndThisEffect();
            }
            if (_upgradeAHealing != null && _upgradeAHealing.EffectEnded == false)
            {
                _upgradeAHealing.EndThisEffect();
            }
            if (Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f)
            {
                _upgradeAEnergyGain = new Effect_ChangeStat(Player.Instance.EnergyGain, new(this)) { FlatAmount = _upgradeAEnergyGainAmount };
                Player.Instance.AddEffect(_upgradeAEnergyGain);
            }
            else
            {
                _upgradeAHealing = new Effect_ChangeStat(Player.Instance.Health, new(this)) { RegenerationFlatAmount = (Player.Instance.Health.Maximum - Player.Instance.Health.Current) * _upgradeAPercentageMissingHealthRestoredPerSecond / 100 };
                Player.Instance.AddEffect(_upgradeAHealing);
            }
        }
    }

    public static void OnEquip()
    {
        EventManager.DamageDealt.AddListener(AddStacks);
    }

    public static void OnUnequip()
    {
        EventManager.DamageDealt.RemoveListener(AddStacks);
    }

    public static void AddStacks(Damage damage)
    {
        if (damage.TargetOfDamage == Player.Instance && SaveFile.Instance.ActiveUpgrades.Contains("Ability_Fortify_UpgradeB"))
        {
            Player.Instance.UpdateTechniqueStacksAmount(typeof(Ability_Fortify), Player.Instance.CurrentTechniqueStacks[typeof(Ability_Fortify)] + 1);
        }
    }

    public override void OnAbilityButtonRelease()
    {
        base.OnAbilityButtonRelease();
        EndThisAbility();
    }

    public void Activate()
    {
        if (Is(Property.Ultimate) && (Player.Instance.CurrentUltimateTechniqueStacks[typeof(Ability_Fortify)] < 1 || Player.Instance.Energy.Current < 1))
        {
            EndThisAbility();
        }
        else if (IsNot(Property.Ultimate) && (Player.Instance.CurrentTechniqueStacks[typeof(Ability_Fortify)] < 1 || Player.Instance.Energy.Current < 1))
        {
            EndThisAbility();
        }
        else
        {
            Player.Instance.Actions.ConsumeEnergyAndCooldownForTheAbility();
        }
    }

    public override void CallAbilityEvent2()
    {
        Player.Instance.PlayAnimation("Fortify", 0, 0.25f);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if (_armorBuff != null && _armorBuff.EffectEnded == false)
        {
            _armorBuff.EndThisEffect();
        }
        if (_unstunnableBuff != null && _unstunnableBuff.EffectEnded == false)
        {
            _unstunnableBuff.EndThisEffect();
        }
        EventManager.OneTenthSecondElapsedInGame.RemoveListener(Activate);
        if (Is(Property.UpgradeA))
        {
            EventManager.UnitHealthChanged.RemoveListener(RefreshUpgradeABuff);
        }
        if (Is(Property.Ultimate))
        {
            EventManager.DamageDealt.RemoveListener(ReflectDamageTaken);
        }
    }

    public float GetEffectiveArmor()
    {
        return _baseArmorGained + Player.Instance.Armor.Current * _percentageArmorGained / 100;
    }

    public void ReflectDamageTaken(Damage damage)
    {
        if (damage.TargetOfDamage == Player.Instance)
        {
            Damage reflection = new Damage(damage.SourceOfDamage.User, this, null);
            reflection.Injury = damage.PreMitigationInjury * _ultimatePercentageOfDamageReflected / 100;
            reflection.Stagger = damage.PreMitigationStagger * _ultimatePercentageOfDamageReflected / 100;
            reflection.CalculateAndApplyDamage();
        }
    }
}