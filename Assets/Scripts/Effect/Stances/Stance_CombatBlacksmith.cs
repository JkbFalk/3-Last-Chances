using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Stance_CombatBlacksmith : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Tonitrui;
    public bool Upgrade3BuffActivated = false;
    public float TimeSpentInThisStance = 0;
    public List<Effect> Upgrade3CurrentWeaponEffects = new();
    public static float CooldownOfRestoring1ToolUse
    {
        get
        {
            return 12 - 12 * EffectList.CalculatePB(StancePB * 0.5f, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { }) / 100;
        }
    }
    public static float ExtraToolPower
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.TOOL_POWER_INCREASE_PER_PB, new List<float> { });
        }
    }
    public static float Upgrade1ProneAppliedOnDealingDamage
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.PRONE_PER_PB, PB.HAPPENS_UPON__DEALING_DAMAGE, PB.REQUIRES__3_SECOND_COOLDOWN });
        }
    }
    public static float Upgrade1ProneEffectivnessForTools
    {
        get
        {
            return EffectList.CalculatePB(PB.PB_VALUE_OF_MAKING_PRONE_NOT_WORK_ON_NON_TOOL_DAMAGE, PB.PRONE_PERCENTAGE_EFFECTIVNESS_PER_PB, new List<float> { PB.AFFECTS_ONLY__TOOL_DAMAGE });
        }
    }
    public static float Upgrade2HealthRestoredOnUsingATool
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.FLAT_HEALTH_RESTORED_PER_PB, new List<float> { PB.HAPPENS_UPON__USING_TOOL });
        }
    }
    public static float Upgrade2CooldownsReducedOnUsingATool
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.5f, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__USING_TOOL });
        }
    }
    public static float Upgrade3TimeInSecondsRequiredToApplyWeaponEffectOntoOthers
    {
        get
        {
            return 35 - 35 * EffectList.CalculatePB(StanceUpgrade2PB * 0.6f, PB.REDUCE_DURATION_NEEDED_FOR_EFFECT_TO_ACTIVATE, new List<float> { PB.REQUIRES__REMAINING_IN_THE_STANCE }) / 100;
        }
    }
    public static float Upgrade3StanceAndItemPower
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.4f, (PB.STANCE_POWER_INCREASE_PER_PB + PB.ITEM_POWER_INCREASE_PER_PB) / 2, new List<float> { PB.REQUIRES__REMAINING_IN_THE_STANCE });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> { Utils.GetFormattedFloat(CooldownOfRestoring1ToolUse), Utils.GetFormattedFloat(ExtraToolPower) };
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade1ProneAppliedOnDealingDamage), Utils.GetFormattedFloat(Upgrade1ProneEffectivnessForTools) };
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade2HealthRestoredOnUsingATool), Utils.GetFormattedFloat(Upgrade2CooldownsReducedOnUsingATool) };
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> { Utils.GetFormattedFloat(Upgrade3TimeInSecondsRequiredToApplyWeaponEffectOntoOthers), Utils.GetFormattedFloat(Upgrade3StanceAndItemPower) };
    }

    public Stance_CombatBlacksmith(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.StanceSwitched, EventManager.CooldownEnded, EventManager.ToolUsed, EventManager.ExitCombat, EventManager.DamageDealt, EventManager.OneTenthSecondElapsedInGame };
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        CheckIfShouldRefill1ToolUse();
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
        if (IsActive)
        {
            Player.Instance.ToolPower.AddFlatModifier(this, ExtraToolPower);
            if (UnlockedUpgrade3 && Upgrade3BuffActivated)
            {
                Player.Instance.StancePower.AddFlatModifier(this, Upgrade3StanceAndItemPower);
                Player.Instance.ItemPower.AddFlatModifier(this, Upgrade3StanceAndItemPower);
                WeaponTheStanceIsAttachedTo.DeactivateItemEffects();
            }
        }
        else if (!IsActive)
        {
            Player.Instance.ToolPower.RemoveFlatModifier(this, ExtraToolPower);
            if (UnlockedUpgrade3 && Upgrade3BuffActivated)
            {
                Player.Instance.StancePower.RemoveFlatModifier(this, Upgrade3StanceAndItemPower);
                Player.Instance.ItemPower.RemoveFlatModifier(this, Upgrade3StanceAndItemPower);
                WeaponTheStanceIsAttachedTo.ActivateItemEffects();
            }
        }
    }

    public override void OnInvokeCooldownEnded(Cooldown cooldown)
    {
        CheckIfShouldRefill1ToolUse();
        base.OnInvokeCooldownEnded(cooldown);
    }

    public override void OnInvokeToolUsed(Item tool)
    {
        CheckIfShouldRefill1ToolUse();
        if (IsActive && UnlockedUpgrade2)
        {
            Player.Instance.Health.Current += Upgrade2HealthRestoredOnUsingATool;
            Player.Instance.ReduceAllRemainingCooldowns(Upgrade2CooldownsReducedOnUsingATool);
        }
        base.OnInvokeToolUsed(tool);
    }

    public void CheckIfShouldRefill1ToolUse()
    {
        if (IsActive && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_CombatBlacksmith_Refill1ToolUse"))
        {
            List<Type> toolsWithoutAllCharges = new();
            foreach (Type tool in SaveFile.Instance.UnlockedTools)
            {
                if (SaveFile.Instance.ToolRemainingAmounts[tool] != SaveFile.Instance.ToolMaxAmounts[tool])
                {
                    toolsWithoutAllCharges.Add(tool);
                }
            }
            if (toolsWithoutAllCharges.Count > 0)
            {
                SaveFile.Instance.ToolRemainingAmounts[toolsWithoutAllCharges[UnityEngine.Random.Range(0, toolsWithoutAllCharges.Count)]]++;
                Player.Instance.AddCooldown(this, CooldownOfRestoring1ToolUse, "Stance_CombatBlacksmith_Refill1ToolUse");
            }
        }
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade1 && damage.SourceOfDamage.User == Player.Instance && !Player.Instance.CheckIfEffectWithGivenIdIsOnCooldown("Stance_CombatBlacksmith_ApplyProneOnDealingDamage"))
        {
            damage.TargetOfDamage.AddEffect(new Effect_Prone(Upgrade1ProneAppliedOnDealingDamage, new(this)));
            Player.Instance.AddCooldown(this, 3, "Stance_CombatBlacksmith_Refill1ToolUse");
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeExitCombat(Unit unit)
    {
        Upgrade3BuffActivated = false;
        TimeSpentInThisStance = 0;
        base.OnInvokeExitCombat(unit);
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        if (IsActive && Player.Instance.InCombat && UnlockedUpgrade3 && !Upgrade3BuffActivated)
        {
            TimeSpentInThisStance += 0.1f;
            if (TimeSpentInThisStance >= Upgrade3TimeInSecondsRequiredToApplyWeaponEffectOntoOthers)
            {
                Upgrade3BuffActivated = true;
                Player.Instance.StancePower.AddFlatModifier(this, Upgrade3StanceAndItemPower);
                Player.Instance.ItemPower.AddFlatModifier(this, Upgrade3StanceAndItemPower);
            }
        }
        base.OnInvokeOneTenthSecondElapsedInGame();
    }
}
