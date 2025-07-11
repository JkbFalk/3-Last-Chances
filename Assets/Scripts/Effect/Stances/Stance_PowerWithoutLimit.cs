using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Reflection;

public class Stance_PowerWithoutLimit : Effect_Stance
{
    private TextMeshProUGUI _amountDisplay;
    public Effect_ChangeStat IncreasedMaxEnergy;
    public Stance_PowerWithoutLimit(SourceOfEffect source_of_effect) : base(source_of_effect) {
        IncreasedMaxEnergy = new Effect_ChangeStat(Player.Instance.Energy, SourceOfEffect);
    }

    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Proprius;

    public override void OnStanceActivated()
    {
        base.OnStanceActivated();
        EventManager.AbilityEnergyConsumed.AddListener(OnInvokeAbilityEnergyConsumed);
        EventManager.ExitCombat.AddListener(OnInvokeExitCombat);
        EventManager.UnitStatCurrentAmountChanged.AddListener(OnInvokeUnitStatCurrentAmountChanged);
        if(_amountDisplay != null) {
            _amountDisplay.text = "";
        }
        if(UnlockedUpgrade3) {
            if(_amountDisplay != null) {
                _amountDisplay.text = Utils.GetFormattedFloat(Player.Instance.Energy.Maximum);
            }
        }
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
    }

    public override void OnStanceDeactivated()
    {
        base.OnStanceDeactivated();
        Player.Instance.EndEffect(IncreasedMaxEnergy);
        EventManager.AbilityEnergyConsumed.RemoveListener(OnInvokeAbilityEnergyConsumed);
        EventManager.ExitCombat.RemoveListener(OnInvokeExitCombat);
        EventManager.UnitStatCurrentAmountChanged.RemoveListener(OnInvokeUnitStatCurrentAmountChanged);
    }

    public override void CreateStanceDisplay() {
        if(UnlockedUpgrade1 || UnlockedUpgrade3) {
            base.CreateStanceDisplay();
            _amountDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Amount").GetComponent<TextMeshProUGUI>();
        }
    }


    public override void OnInvokeAbilityEnergyConsumed(Ability ability, float energy) {
        if(IsActive && UnlockedUpgrade2 && Player.Instance.Energy.Current > 0) {
            if(ability.GetType().GetField("IsVariableEnergyTechnique", BindingFlags.Public | BindingFlags.Static) != null) {
                Player.Instance.AddEffect(new Effect_CustomizableDamageChange(SourceOfEffect) { ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        (damage.SourceOfDamage == ability)),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtPercentageModifier += 200;
                        damage.StaggerDealtPercentageModifier += 200;
                    })});
            }
            else {
                float energyConsumed = Player.Instance.Energy.Current;
                Player.Instance.Energy.Current = 0;
                Player.Instance.AddEffect(new Effect_CustomizableDamageChange(SourceOfEffect) { ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                        (damage.SourceOfDamage == ability)),
                    Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        damage.InjuryDealtPercentageModifier = energyConsumed * 4;
                        damage.StaggerDealtPercentageModifier = energyConsumed * 4;
                    })});
            }
        }
        if(IsActive && UnlockedUpgrade3) {
                IncreasedMaxEnergy.FlatAmount += energy / 5f;
                if(!IncreasedMaxEnergy.IsTurnedOn) {
                    Player.Instance.AddEffect(IncreasedMaxEnergy);
                }
                _amountDisplay.text = Utils.GetFormattedFloat(Player.Instance.Energy.Maximum);
        }
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount_changed)
    {
        if(UnlockedUpgrade1 && stat.Owner is Player && stat is Energy && amount_changed > 0 && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == GetType()) == null) {
            Player.Instance.Energy.ChangeCurrentValueWithoutInvoking(Player.Instance.Energy.Maximum + Player.Instance.Energy.Maximum / 10);
            Player.Instance.AddCooldown(new Cooldown(GetType(), 3, Player.Instance) {CooldownDisplay = _amountDisplay.transform.parent.Find("Cooldown").GetComponent<Image>()});
        }
    }

    public void OnInvokeExitCombat(Unit unit) {
        if(UnlockedUpgrade3 && unit is Player) {
            Player.Instance.EndEffect(IncreasedMaxEnergy);
            IncreasedMaxEnergy.FlatAmount = 0;
        }
    }
}
