using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

public class Stance_MindOverMatter : Effect_Stance
{
    private Image _cooldownDisplay;
    private TextMeshProUGUI _healthPercentage;
    public Effect_ChangeStat EnergyGainDoubled;
    public Stance_MindOverMatter(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners.Add(EventManager.DamageDealt);
        Listeners.Add(EventManager.UnitStatCurrentAmountChanged);
        EnergyGainDoubled = new Effect_ChangeStat(Player.Instance.EnergyGain, SourceOfEffect) {PercentageModifier = 0, ShowsInMenu=false};
    }

    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Proprius;

    public override void CreateStanceDisplay() {
        if(UnlockedUpgrade3) {
            base.CreateStanceDisplay();
            _cooldownDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Cooldown").GetComponent<Image>();
            _healthPercentage = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Amount").GetComponent<TextMeshProUGUI>();
        }
    }

    public override void OnStanceActivated() {
        if(UnlockedUpgrade3 && _healthPercentage != null) {
            _healthPercentage.text = Player.Instance.Energy.Current > 200 ? "100%" : Utils.GetFormattedFloat(Player.Instance.Energy.Current / 2) + "%";
        }
        if(UnlockedUpgrade1) {
            DoubleEnergyGain();
        }
    }

    public override void OnStanceDeactivated() {
        if(UnlockedUpgrade1) {
            DoubleEnergyGain();
        }
    }

    public override void OnStanceEquipped() {
        if(UnlockedUpgrade1) {
            Player.Instance.AddEffect(EnergyGainDoubled);
        }
    }

    public override void OnStanceUnequipped() {
        if(UnlockedUpgrade1) {
            Player.Instance.EndEffect(EnergyGainDoubled);
        }
    }

    public override void OnInvokeUnitStatCurrentAmountChanged(Stat stat, float amount_changed)
    {
        if(IsActive && UnlockedUpgrade3 && stat.Owner is Player && stat is Energy) {
            _healthPercentage.text = Player.Instance.Energy.Current > 200 ? "100%" : Utils.GetFormattedFloat(Player.Instance.Energy.Current / 2) + "%";
        }
        if(stat.Owner is Player && stat is EnergyGain) {
            DoubleEnergyGain();
        }
    }

    public void DoubleEnergyGain() {
        if(UnlockedUpgrade1 && Player.Instance.EnergyGain.ShouldInvoke) {
            Player.Instance.EnergyGain.ShouldInvoke = false;
            EnergyGainDoubled.PercentageModifier = 0;
            if(Player.Instance.EnergyGain.Current > 1 && IsActive) {
                Player.Instance.EnergyGain.ShouldInvoke = false;
                EnergyGainDoubled.PercentageModifier = (int)Math.Round((Player.Instance.EnergyGain.Current - 1) * 100);
            }
        }
    }

    public override void OnInvokeDamageDealt(Damage damage) {
        if(IsActive && UnlockedUpgrade2 && damage.SourceOfDamage.User == Player.Instance) {
            damage.TargetOfDamage.AddEffect(new Effect_Analysis(10, SourceOfEffect));
        }
        if(IsActive && UnlockedUpgrade3 && damage.TargetOfDamage == Player.Instance && damage.OverkillInjury > 0 && Player.Instance.Energy.Current > 2 && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.Type == typeof(Stance_MindOverMatter)) == null) {
            Player.Instance.Health.Current = Player.Instance.Health.Maximum / 100 * Player.Instance.Energy.Current / 2;
            Player.Instance.Energy.Current = 0;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "Ability/Stance_MindOverMatterSave", 2f);
            Utils.CreateVisualEffect(SourceOfEffect, "MindOverMatterSave", Player.Instance.SpriteRenderers["Upper Body"].Bone.position.x, Player.Instance.SpriteRenderers["Upper Body"].Bone.position.y);
            Player.Instance.AddCooldown(new Cooldown(typeof(Stance_MindOverMatter), 60, Player.Instance) {CooldownDisplay = _cooldownDisplay});
        }
    }
}
