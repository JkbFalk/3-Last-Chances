using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Stance_SingularPursuit : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Anima;
    public int CurrentFlowRank = 0;
    public float CurrentFlowStacks = 0;
    public int ConsecutiveCountersAmount = 0;
    public static float StacksGivenPerWeaponDamageDealt = 20;
    public static float StacksLostEachSecond = 2;
    public static float StacksLostForEachDamageTaken = 35;
    public static Dictionary<int, float> MaxStacksAtEachRank = new Dictionary<int, float>()
    {
        { 0, 100 },
        { 1, 200 },
        { 2, 300 },
        { 3, 400 },
        { 4, 500 }
    };

    public static Dictionary<int, float> StacksLostMultiplierAtEachRank = new Dictionary<int, float>()
    {
        { 0, 1 },
        { 1, 1.5f },
        { 2, 2 },
        { 3, 2.5f },
        { 4, 3 }
    };

    public static Dictionary<int, float> MultiplierPercentageAtEachRank = new Dictionary<int, float>()
    {
        { 0, 0 },
        { 1, 0.15f },
        { 2, 0.35f },
        { 3, 0.6f },
        { 4, 1 }
    };
    public static float DamageMultiplierAtRankS
    {
        get
        {
            return EffectList.CalculatePB(StancePB, PB.DAMAGE_MULTIPLIER_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE, PB.REQUIRES__STACKING_UP_FLOW_FOR_MAXIMUM_VALUE });
        }
    }
    public static float Upgrade1StacksIncreasedFromRipostes
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.INCREASE_FLOW_STACKS_ACQUIRED_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__RIPOSTING });
        }
    }
    public static float Upgrade1StacksIncreasedFromCounters
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade1PB * 0.5f, PB.INCREASE_FLOW_STACKS_ACQUIRED_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__COUNTERING });
        }
    }
    public static float ArmorPerRankOfFlow
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.65f, PB.ARMOR_PER_PB, new List<float> { 1 / PB.SPECIAL__EXPECTED_RANK_OF_FLOW });
        }
    }
    public static float ArmorAtRankS
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB * 0.35f, PB.ARMOR_PER_PB, new List<float> { PB.REQUIRES__FLOW_RANK_S });
        }
    }
    public static float DamageDealtIncreasePerConsecutiveCounter
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB, PB.DAMAGE_INCREASE_PER_PB, new List<float> { PB.AFFECTS_ONLY__WEAPON_DAMAGE, 1 / PB.SPECIAL__EXPECTED_AMOUNT_OF_CONSECUTIVE_COUNTERS_WITHOUT_GETTING_HIT_BY_COUNTERABLE });
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> {Utils.GetFormattedFloat(1 + DamageMultiplierAtRankS, 1)};
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> {Utils.GetFormattedFloat(Upgrade1StacksIncreasedFromRipostes), Utils.GetFormattedFloat(Upgrade1StacksIncreasedFromCounters)};
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> {Utils.GetFormattedFloat(ArmorPerRankOfFlow), Utils.GetFormattedFloat(ArmorAtRankS)};
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> {Utils.GetFormattedFloat(DamageDealtIncreasePerConsecutiveCounter)};
    }
    public TextMeshProUGUI ConsecutiveCountersDisplay;
    public Effect_ChangeStat ArmorBuff;

    public Stance_SingularPursuit(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners = new List<UnityEventBase> { EventManager.HitDealt, EventManager.DamageDealt, EventManager.OneTenthSecondElapsedInGame, EventManager.AbilityUsed, EventManager.StanceSwitched };
    }

    public override void OnInvokeHitDealt(DamageInstance damage)
    {
        if (IsActive && CurrentFlowRank != 0)
        {
            damage.DamageDealtMultiplier += MultiplierPercentageAtEachRank[CurrentFlowRank] * DamageMultiplierAtRankS;
        }
        if (IsActive && ConsecutiveCountersAmount > 0)
        {
            damage.DamageDealtPercentageModifier += ConsecutiveCountersAmount * DamageDealtIncreasePerConsecutiveCounter;
        }
        base.OnInvokeHitDealt(damage);
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && damage.SourceOfDamage.User == Player.Instance && damage.IsWeaponDamage && !damage.SourceOfDamage.TriggeredEffects.Contains(this))
        {
            if (UnlockedUpgrade3 && damage.SourceOfDamage.Is(Ability.Property.Counter))
            {
                ConsecutiveCountersAmount++;
                UpdateConsecutiveCountersDisplay();
            }
            float stacksGiven = StacksGivenPerWeaponDamageDealt;
            if (UnlockedUpgrade1 && damage.SourceOfDamage.Is(Ability.Property.Counter))
            {
                stacksGiven *= 1 + Upgrade1StacksIncreasedFromCounters / 100;
            }
            else if (UnlockedUpgrade1 && damage.SourceOfDamage.Is(Ability.Property.Riposte))
            {
                stacksGiven *= 1 + Upgrade1StacksIncreasedFromRipostes / 100;
            }
            CurrentFlowStacks += stacksGiven;
            UpdateRank();
        }
        else if (IsActive && damage.TargetOfDamage == Player.Instance)
        {
            CurrentFlowStacks -= StacksLostForEachDamageTaken * StacksLostMultiplierAtEachRank[CurrentFlowRank] / 10 * (damage.DamageWasBlocked ? 0.25f : 1);
            if (damage.SourceOfDamage.Is(Ability.Property.CounteredByBackstep) || damage.SourceOfDamage.Is(Ability.Property.CounteredByRoll) || damage.SourceOfDamage.Is(Ability.Property.CounteredByRiposte))
            {
                ConsecutiveCountersAmount = 0;
                UpdateConsecutiveCountersDisplay();
            }
            UpdateRank();
        }
        base.OnInvokeDamageDealt(damage);
    }

    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        base.OnInvokeOneTenthSecondElapsedInGame();
        CurrentFlowStacks -= StacksLostEachSecond * StacksLostMultiplierAtEachRank[CurrentFlowRank] / 10;
        UpdateRank();

    }

    public override void CreateStanceDisplay()
    {
        base.CreateStanceDisplay();
        foreach (Transform child in Player.Instance.CurrentStanceGauge.transform)
        {
            child.gameObject.SetActive(false);
        }
        Player.Instance.CurrentStanceGauge.transform.Find(CurrentFlowRank.ToString()).gameObject.SetActive(true);
        if (UnlockedUpgrade3)
        {
            Player.Instance.CurrentStanceGauge.transform.Find("Counter").gameObject.SetActive(true);
        }
        ConsecutiveCountersDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Counter").GetComponent<TextMeshProUGUI>();
        Player.Instance.CurrentStanceGauge.transform.Find(CurrentFlowRank.ToString() + "/Fill").GetComponent<Image>().fillAmount = CurrentFlowStacks / MaxStacksAtEachRank[CurrentFlowRank];
    }

    public void UpdateRank()
    {
        if(Player.Instance.CurrentStanceGauge == null) {
            return;
        }
        int previousRank = CurrentFlowRank;
        if (CurrentFlowRank == 4 && CurrentFlowStacks > MaxStacksAtEachRank[CurrentFlowRank])
        {
            CurrentFlowStacks = MaxStacksAtEachRank[CurrentFlowRank];
        }
        else if (CurrentFlowRank == 0 && CurrentFlowStacks < 0)
        {
            CurrentFlowStacks = 0;
        }
        else if (CurrentFlowStacks >= MaxStacksAtEachRank[CurrentFlowRank])
        {
            CurrentFlowStacks -= MaxStacksAtEachRank[CurrentFlowRank];
            CurrentFlowRank++;
        }
        else if (CurrentFlowStacks < 0)
        {
            CurrentFlowStacks += MaxStacksAtEachRank[CurrentFlowRank - 1];
            CurrentFlowRank--;
        }
        if (previousRank != CurrentFlowRank)
        {
            Player.Instance.CurrentStanceGauge.transform.Find(previousRank.ToString()).gameObject.SetActive(false);
            Player.Instance.CurrentStanceGauge.transform.Find(CurrentFlowRank.ToString()).gameObject.SetActive(true);
        }
        Player.Instance.CurrentStanceGauge.transform.Find(CurrentFlowRank.ToString() + "/Fill").GetComponent<Image>().fillAmount = CurrentFlowStacks / MaxStacksAtEachRank[CurrentFlowRank];
    }

    public void UpdateConsecutiveCountersDisplay()
    {
        ConsecutiveCountersDisplay.text = "x" + ConsecutiveCountersAmount.ToString();
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
        if (!IsActive && ArmorBuff != null && !ArmorBuff.EffectEnded)
        {
            ArmorBuff.EndThisEffect();
        }
        if (IsActive && UnlockedUpgrade2)
        {
            ArmorBuff = new Effect_ChangeStat(Player.Instance.Armor, new SourceOfEffect(Player.Instance))
            {
                FlatAmount = (CurrentFlowRank + 1) * ArmorPerRankOfFlow + (CurrentFlowRank == 4 ? ArmorAtRankS : 0)
            };
            Player.Instance.AddEffect(ArmorBuff);
        }
    }
}
