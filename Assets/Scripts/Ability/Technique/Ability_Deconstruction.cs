using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Deconstruction : Technique
{
    public static float EnergyCost = 50;
    public static float Cooldown = 30;
    public static int MaxStacks = 5;
    public static int UpgradeAMaxStacks = 7;
    private static float _upgradeADamageIncreasePerUniqueDeconstruction = 30;
    public static float UpgradeBArmorPerStack = 50;
    private static float _ultimateInjuryScaling = 500;
    private static float _ultimateStaggerScaling = 500;
    private Effect_ChangeCompositeStat _upgradeADamageBuff;
    public static AbilityFamily Family = AbilityFamily.Anima;
    public static Constants.DamageType TechniqueDamageType = Constants.DamageType.None;
    public static bool CanBeUsedDuringOtherAbilities
    {
        get
        {
            return Player.Instance.PreparingForUltimate == false;
        }
    }

    public Ability_Deconstruction(Unit ability_user) : base(ability_user)
    {
        if (Is(Property.Ultimate))
        {
            NameOfAnimationToAutoPlay = "Deconstruction_Ultimate_" + User.CurrentWeaponClass;
            DamageSources.Add(new DamageSource(_ultimateInjuryScaling, _ultimateStaggerScaling, User.CurrentWeaponDamageType) {KnockbackInMeters = 2});
        }
        else
        {
            AutoPlayAbilityAnimation = false;
        }
        AddCustomSound("Use1", "Ability/Ability_Deconstruction1", 0.5f);
        AddCustomSound("Swing", "Ability/Ability_DeconstructionSwing", 0.6f);
        AddCustomSound("Ultimate", "Ability/Ability_Deconstruction_Ultimate", 0.6f);
        AddCustomSound("Fail", "Ability/Ability_Deconstruction_Fail", 0.7f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { MaxStacks.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeADamageIncreasePerUniqueDeconstruction.ToString(), UpgradeAMaxStacks.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { UpgradeBArmorPerStack.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { (Player.Instance.CurrentWeaponInjury.Current * _ultimateInjuryScaling / 100).ToString(), _ultimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * _ultimateStaggerScaling / 100).ToString(), _ultimateStaggerScaling.ToString(), MaxStacks.ToString() };
    }

    public override void CallAbilityEvent1()
    {
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "Deconstruction_Ultimate", Player.Instance.transform.position.x, Player.Instance.transform.position.y);
        aoe.transform.parent.eulerAngles = new Vector3(0, Player.Instance.Actions.IsFlipped ? 180 : 0, 0);
    }

    public override void ActionsToPerformDuringAnotherAbility()
    {
        Ability targetAbility = (Player.Instance.CurrentTarget != null && Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed != null) ? Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed : null;
        Type targetAbilityType = targetAbility?.GetType();
        if (targetAbilityType == null)
        {
            foreach (Unit enemy in Utils.GetAllUnits(true, true).OrderBy(u => Vector2.Distance(Player.Instance.transform.position, u.transform.position)))
            {
                if (enemy.Actions.CurrentAbilityBeingPerformed != null)
                {
                    targetAbility = enemy.Actions.CurrentAbilityBeingPerformed;
                    targetAbilityType = targetAbility?.GetType();
                }
            }
        }
        if (targetAbilityType == null || targetAbilityType.Name.Contains("AI_"))
        {
            PlayCustomSound("Fail");
            return;
        }
        ConsumeEnergyAndCooldownForTheAbility();
        GameObject vfx = Utils.CreateVisualEffect(new(this), "Deconstruction", Player.Instance.SpriteRenderers["Head"].Bone.transform.position.x, Player.Instance.SpriteRenderers["Head"].Bone.transform.position.y);
        vfx.transform.SetParent(Player.Instance.SpriteRenderers["Head"].Bone);
        vfx.transform.localPosition = new Vector2(0.23f, -0.12f);
        Effect_Deconstruction currentEffect = (Effect_Deconstruction)Player.Instance.GetEffect(typeof(Effect_Deconstruction));
        if (currentEffect != null && currentEffect.DeconstructionTarget == targetAbilityType)
        {
            currentEffect.IncrementStacks(targetAbility);
        }
        else if (currentEffect != null)
        {
            currentEffect.EndThisEffect();
            Player.Instance.AddEffect(new Effect_Deconstruction(targetAbilityType, new(this)) {UpgradeA = Is(Property.UpgradeA), UpgradeB = Is(Property.UpgradeB)});
            PlayCustomSound("Use1");
        }
        else
        {
            Player.Instance.AddEffect(new Effect_Deconstruction(targetAbilityType, new(this)));
            PlayCustomSound("Use1");
        }
        if (Is(Property.UpgradeA) && Effect_Deconstruction.UniqueDeconstructions.Contains(targetAbilityType) == false)
        {
            Effect_Deconstruction.UniqueDeconstructions.Add(targetAbilityType);
            if (Player.Instance.GetEffect(new System.Func<Effect, bool>(effect => effect.Identifier == "DeconstructionUpgradeADamageBuff")) != null)
            {
                _upgradeADamageBuff = (Effect_ChangeCompositeStat)Player.Instance.GetEffect(new System.Func<Effect, bool>(effect => effect.Identifier == "DeconstructionUpgradeADamageBuff"));
            }
            else {
                _upgradeADamageBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(this)) { ShowsInUI = true, Identifier = "DeconstructionUpgradeADamageBuff" };
                Player.Instance.AddEffect(_upgradeADamageBuff);
            }
            _upgradeADamageBuff.PercentageAmount = Effect_Deconstruction.UniqueDeconstructions.Count * _upgradeADamageIncreasePerUniqueDeconstruction;
            _upgradeADamageBuff.UIText = Utils.GetFormattedFloat(Effect_Deconstruction.UniqueDeconstructions.Count * _upgradeADamageIncreasePerUniqueDeconstruction);
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        Effect_Deconstruction deconstruction = (Effect_Deconstruction)Player.Instance.GetEffect(typeof(Effect_Deconstruction));
        if (deconstruction == null || deconstruction.Stacks < 5 || damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed == null || damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed.GetType() != deconstruction.DeconstructionTarget)
        {
            return;
        }
        damage.TargetOfDamage.AddEffect(new Effect_ExploitedWeakness(deconstruction.DeconstructionTarget, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}