using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Ability_Deconstruction : Technique
{
    public static float EnergyCost = 50;
    public static float Cooldown = 30;
    private static float _maxStacks = 5;
    private static float _upgradeAMaxStacks = 7;
    private static float _upgradeADamageBuffPerUniqueDeconstruction = 20;
    public static float UpgradeBArmorPerStack = 50;
    private static float _ultimateInjuryScaling = 500;
    private static float _ultimateStaggerScaling = 500;
    private List<Ability> UniqueDeconstructions = new List<Ability>();
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
            NameOfAnimationToAutoPlay = "Deconstruction_" + User.CurrentWeaponType + "_Ultimate";
            DamageSources.Add(new DamageSource(_ultimateInjuryScaling, _ultimateStaggerScaling, User.CurrentWeaponDamageType));
        }
        else
        {
            AutoPlayAbilityAnimation = false;
        }
        AddCustomSound("Use1", "Ability/Ability_Deconstruction1", 0.5f);
        AddCustomSound("Use2", "Ability/Ability_Deconstruction2", 0.5f);
        AddCustomSound("Use3", "Ability/Ability_Deconstruction3", 0.5f);
        AddCustomSound("Swing", "Ability/Ability_DeconstructionSwing", 0.6f);
        AddCustomSound("Ultimate", "Ability/Ability_Deconstruction_Ultimate", 0.6f);
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> { _maxStacks.ToString() };
    }

    public static List<string> GetMasteryADescriptionValues()
    {
        return new List<string> { _upgradeADamageBuffPerUniqueDeconstruction.ToString(), _upgradeAMaxStacks.ToString() };
    }

    public static List<string> GetMasteryBDescriptionValues()
    {
        return new List<string> { UpgradeBArmorPerStack.ToString() };
    }

    public static List<string> GetUltimateDescriptionValues()
    {
        return new List<string> { (Player.Instance.CurrentWeaponInjury.Current * _ultimateInjuryScaling / 100).ToString(), _ultimateInjuryScaling.ToString(), (Player.Instance.CurrentWeaponStagger.Current * _ultimateStaggerScaling / 100).ToString(), _ultimateStaggerScaling.ToString(), _maxStacks.ToString() };
    }

    public override void ActionsToPerformDuringAnotherAbility() {
        GameObject vfx = Utils.CreateVisualEffect(new(this), "Deconstruction", Player.Instance.SpriteRenderers["Head"].Bone.transform.position.x, Player.Instance.SpriteRenderers["Head"].Bone.transform.position.y);
        vfx.transform.SetParent(Player.Instance.SpriteRenderers["Head"].Bone);
        vfx.transform.localPosition = new Vector2(0.23f, -0.12f);
        Type targetAbility = (Player.Instance.CurrentTarget != null && Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed != null) ? Player.Instance.CurrentTarget.Actions.CurrentAbilityBeingPerformed.GetType() : null;
        Debug.Log($"INITIAL TARGET: {targetAbility}");
        if (targetAbility == null)
        {
            foreach (Unit enemy in Utils.GetAllUnits(true, true).OrderBy(u => Vector2.Distance(Player.Instance.transform.position, u.transform.position)))
            {
                if (enemy.Actions.CurrentAbilityBeingPerformed != null)
                {
                    targetAbility = enemy.Actions.CurrentAbilityBeingPerformed.GetType();
                }
            }
        }
        Debug.Log($"FINAL TARGET: {targetAbility}");
        if (targetAbility == null)
        {
            return;
        }
        Effect_Deconstruction currentEffect = (Effect_Deconstruction)Player.Instance.GetEffect(typeof(Effect_Deconstruction));
        if (currentEffect != null && currentEffect.DeconstructionTarget == targetAbility)
        {
            currentEffect.Stacks++;
            currentEffect.UIText = currentEffect.Stacks.ToString();
            PlayCustomSound("Use" + (currentEffect.Stacks > 4 ? "3" : "2"));
        }
        else if (currentEffect != null)
        {
            currentEffect.EndThisEffect();
            Player.Instance.AddEffect(new Effect_Deconstruction(targetAbility, new(this)));
            PlayCustomSound("Use1");
        }
        else
        {
            Player.Instance.AddEffect(new Effect_Deconstruction(targetAbility, new(this)));
            PlayCustomSound("Use1");
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        Effect_Deconstruction deconstruction = (Effect_Deconstruction)Player.Instance.GetEffect(typeof(Effect_Deconstruction));
        Debug.Log($"ULTIMATE DECON: deconstruction: {deconstruction}, deconstruction.Stacks: {deconstruction.Stacks}, damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed: {damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed}, damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed.GetType(): {damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed.GetType()}, deconstruction.DeconstructionTarget: {deconstruction.DeconstructionTarget}");
        if (deconstruction == null || deconstruction.Stacks < 5 || damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed == null || damage.TargetOfDamage.Actions.CurrentAbilityBeingPerformed.GetType() != deconstruction.DeconstructionTarget)
        {
            return;
        }
        Debug.Log($"DISABLING {deconstruction.DeconstructionTarget}: {damage.TargetOfDamage.UnitAI.Actions.Count}");
        damage.TargetOfDamage.UnitAI.Actions.Remove(deconstruction.DeconstructionTarget);
        Debug.Log($"DISABLED {deconstruction.DeconstructionTarget}: {damage.TargetOfDamage.UnitAI.Actions.Count}");
    }
}