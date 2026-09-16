using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Stance_OmniMastery : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Anima;
    public static float DamageBuffDuration = 5;
    public static float OnslaughtGiven
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ONSLAUGHT_PER_PB, PB.AFFECTS_ONLY__5_SECONDS });
        }
    }
    public static float AccelerationGiven
    {
        get
        {
            return EffectList.CalculatePB(StancePB * 0.5f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.ACCELERATION_PER_PB, PB.AFFECTS_ONLY__5_SECONDS });
        }
    }
    public static float CCCleanseCooldown
    {
        get
        {
            return 75f * (1 - EffectList.CalculatePB(StanceUpgrade1PB, PB.REDUCE_SPECIFIED_EFFECT_COOLDOWN_PER_PB, new List<float> { }) / 100) / (1 + Player.Instance.CooldownReduction.GetEffectCooldownReduction(typeof(Stance_OmniMastery)) / 100);
        }
    }
    public static float CooldownReductionPercentage
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade2PB, PB.REDUCE_ALL_REMAINING_COOLDOWNS_PERCENTAGE_PER_PB, new List<float> { PB.HAPPENS_UPON__BASIC_ATTACKING, PB.AFFECTS_ONLY__COOLDOWNS_OF_TECHNIQUES_IN_OTHER_STANCES });
        }
    }
    public static float RiposteDamageScaling
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.65f, PB.DAMAGE_SCALING_PER_PB, new List<float> { });
        }
    }
    public static float SharpGainedOnRiposte
    {
        get
        {
            return EffectList.CalculatePB(StanceUpgrade3PB * 0.35f, PB.APPLY_STACKING_EFFECT_PER_PB, new List<float> { PB.SHARP_PER_PB, PB.HAPPENS_UPON__RIPOSTING_OR_COUNTERING});
        }
    }
    public static List<string> GetDescriptionValues()
    {
        return new List<string> {Utils.GetFormattedFloat(OnslaughtGiven), Utils.GetFormattedFloat(AccelerationGiven), "5"};
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> {Utils.GetFormattedFloat(CCCleanseCooldown)};
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> {Utils.GetFormattedFloat(CooldownReductionPercentage)};
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> {Utils.GetFormattedFloat(Player.Instance.CurrentWeaponInjury.Current * RiposteDamageScaling / 100 + Player.Instance.CurrentWeaponStagger.Current * RiposteDamageScaling / 100), Utils.GetFormattedFloat(RiposteDamageScaling), Utils.GetFormattedFloat(SharpGainedOnRiposte)};
    }
    public Image DamageGauge;
    public Image CleanseGauge;

    public Stance_OmniMastery(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Listeners.Add(EventManager.EffectStarted);
        Listeners.Add(EventManager.DamageDealt);
        Listeners.Add(EventManager.StanceSwitched);
    }

    public override void OnInvokeStanceSwitched(Type stance_switched_from, Type stance_switched_to)
    {
        base.OnInvokeStanceSwitched(stance_switched_from, stance_switched_to);
        if (IsActive && !Player.Instance.CheckIfUnderEffectWithGivenId("Stance_OmniMastery - EndDamageBuffs"))
        {
            Player.Instance.AddEffect(new Effect_Onslaught(OnslaughtGiven, new SourceOfEffect(Player.Instance)));
            Player.Instance.AddEffect(new Effect_Acceleration(AccelerationGiven, new SourceOfEffect(Player.Instance)));
            Player.Instance.AddEffect(new Effect_Id("Stance_OmniMastery - EndDamageBuffs", new SourceOfEffect(Player.Instance))
            {
                UICooldownDisplay = DamageGauge,
                ActionOnEnd = new Action<Effect_Id>((effect) =>
                {
                    Player.Instance.GetEffect(typeof(Effect_Onslaught))?.EndThisEffect();
                    Player.Instance.GetEffect(typeof(Effect_Acceleration))?.EndThisEffect();
                })
            }, DamageBuffDuration);
        }
        if (IsActive && UnlockedUpgrade3)
        {
            Ability ba = (BasicAttack)Activator.CreateInstance(typeof(BA_OmniMastery), new object[] { Player.Instance });
            Player.Instance.Actions.CurrentAbilityBeingPerformed = ba;
        }
    }

    public override void OnInvokeDamageDealt(DamageInstance damage)
    {
        if (IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.Property.BasicAttack) && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false)
        {
            List<Type> affected_abilities = new List<Type>();
            List<Type> unaffectable_abilities = new List<Type>();
            foreach (Stance.EquippedAbility a in Player.Instance.CurrentStance.Abilities)
            {
                unaffectable_abilities.Add(a.Type);
            }
            for (int i = 0; i < 3; i++)
            {
                foreach (Stance.EquippedAbility a in SaveFile.Instance.Stances[i].Abilities)
                {
                    if (!unaffectable_abilities.Contains(a.Type) && !affected_abilities.Contains(a.Type) && Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == a.Type) != null)
                    {
                        Cooldown cd = Player.Instance.TechniqueCooldowns.FirstOrDefault(cd => cd.Type == a.Type);
                        cd.RemainingDuration = cd.RemainingDuration * (1 - CooldownReductionPercentage / 100);
                        affected_abilities.Add(a.Type);
                    }
                }
            }
            base.OnInvokeDamageDealt(damage);
        }
    }


    public override void CreateStanceDisplay() {
        base.CreateStanceDisplay();
        DamageGauge = Player.Instance.CurrentStanceGauge.transform.Find("Damage/Fill").GetComponent<Image>();
        if (UnlockedUpgrade1 == false)
        {
            Player.Instance.CurrentStanceGauge.transform.Find("Cleanse").gameObject.SetActive(false);
        }
        else
        {
            CleanseGauge = Player.Instance.CurrentStanceGauge.transform.Find("Cleanse/Fill").GetComponent<Image>();
            Cooldown cd = Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Id == "Stance_OmniMastery1");
            if (cd == null)
            {
                CleanseGauge.fillAmount = 1;
            }
            else
            {
                CleanseGauge.fillAmount = cd.RemainingDuration / cd.TotalDuration;
            }
        }
    }
}
