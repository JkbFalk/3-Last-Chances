using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassivePowerUpTile : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    public string Id;
    public string Tree;
    public string Row;
    public string PowerUp;
    public string[] Params;
    public bool IsAvailable = false;
    public List<Effect> PowerUpEffects = new List<Effect>();

    private void Start() {
        Tree = transform.parent.parent.parent.parent.parent.name;
        Row = transform.parent.name;
        Id = Tree + "-" + Row + "-" + gameObject.name;
        IsAvailable = Row == "1";
        transform.parent.Find("Disabled").gameObject.SetActive(!IsAvailable);
    }

    public void OnClick() {
        if(SaveFile.Instance.UnlockedPowerUps.Contains(Id)) {
            RefundTile();
        }
        else if(CheckIfCanUnlockTile()) {
            UnlockTile();
        }
    }

    public bool CheckIfCanUnlockTile() {
        int row = int.Parse(Row);
        if(row >= 10 && SaveFile.Instance.Level < 30) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {30}));
        }
        else if(row >= 7 && SaveFile.Instance.Level < 20) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {20}));
        }
        else if(row >= 4 && SaveFile.Instance.Level < 10) {
            NotificationController.ShowTextNotification(string.Format(Label.Get("HigherLevelRequired"), new object[] {10}));
        }
        else if(SaveFile.Instance.MaxPassivePowerUps == 0 && SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(powerup => powerup.StartsWith(Tree + "-" + Row)) == null) {
            NotificationController.ShowTextNotification(Label.Get("PowerUpPointsRequired"));
        }
        else {
            return true;
        }
        return false;
    }

    public void UnlockTile() {
        if(IsAvailable == false) {
            NotificationController.ShowTextNotification(Label.Get("PreviousPowerUpTierRequired"));
            return;
        }
        if(Row == "12" && SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(power_up => power_up.Contains("12") && !power_up.Contains(Tree)) != null) {
            NotificationController.ShowTextNotification(Label.Get("OnlyOneHealingPowerUp"));
            return;
        }
        foreach(PassivePowerUpTile power_up in transform.parent.GetComponentsInChildren<PassivePowerUpTile>()) {
            if(!power_up.RefundTile(true)) {
                return;
            }
        }
        SaveFile.Instance.UnlockedPowerUps.Add(Id);
        ApplyPowerUpOfThisTile();
        SaveFile.Instance.UsedPassivePowerUps++;
        SaveFile.Instance.MaxPassivePowerUps--;
        CheckIfRowIsActive(transform.parent.Find("Active"));
        GetComponent<Image>().color = Colors.Gold;
        MenuManager.Instance.ShowPowerUpDetails(this, true);
    }

    public bool RefundTile(bool swap_power_up = false) {
        if(SaveFile.Instance.UnlockedPowerUps.Contains(Id)) {
            if(swap_power_up == false && CheckIfAlreadyUnlockedHigherTier()) {
                NotificationController.ShowTextNotification("CannotRefundPowerUpWhenHigherTierUnlocked");
                return false;
            }
            SaveFile.Instance.UnlockedPowerUps.Remove(Id);
            foreach(Effect effect in PowerUpEffects) {
                Player.Instance.EndEffect(effect);
            }
            SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] = SaveFile.Instance.UnlockedPowerUps.Where(p => p.StartsWith(Tree)).Count();
            MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Label").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
            SaveFile.Instance.UsedPassivePowerUps--;
            SaveFile.Instance.MaxPassivePowerUps++;
            CheckIfRowIsActive(transform.parent.Find("Active"));
            GetComponent<Image>().color = Color.black;
            MenuManager.Instance.ShowPowerUpDetails(this, true);
        }
        return true;
    }

    public bool CheckIfAlreadyUnlockedHigherTier() {
        int row = int.Parse(Row);
        for(int i = row + 1; i < 12; i++) {
            if(SaveFile.Instance.UnlockedPowerUps.FirstOrDefault(power_up => power_up.StartsWith(Tree + "-" + i)) != null) {
                return true;
            }
        }
        return false;
    }

    public void AddPassivePowerUp(string power_up_name, float percentage_value = 0, float flat_value = 0) {
        PowerUpEffects.AddRange(GetPassivePowerUpEffects(power_up_name, percentage_value, flat_value));
        SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] = SaveFile.Instance.UnlockedPowerUps.Where(p => p.StartsWith(Tree)).Count();
        MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Label").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
    }

    public void ApplyPowerUpOfThisTile() {
        string[] power_ups = PowerUp.Split("+");
        PowerUpEffects = new List<Effect>();
        for(int i = 0; i < power_ups.Length; i++) {
            AddPassivePowerUp(power_ups[i], Params[i].Contains("%") ? float.Parse(Params[i].Replace("%" , "")) : 0, Params[i].Contains("%") ? 0 : float.Parse(Params[i]));
        }
        foreach(Effect e in PowerUpEffects) {
            e.IsRemovable = false;
            e.ShowsInMenu=false;
            Player.Instance.AddEffect(e);
        }
    }

    public static List<Effect> GetPassivePowerUpEffects(string power_up_name, float percentage_value = 0, float flat_value = 0) {
        List<Effect> PowerUpEffects = new List<Effect>();
        if (power_up_name != "DamageReduction" && Utils.GetPlayerStatForGivenName(power_up_name) != null)
        {
            PowerUpEffects.Add(new Effect_ChangeStat(Utils.GetPlayerStatForGivenName(power_up_name), new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value}  );
        }
        else if (power_up_name == "StealthAttackDamage")
        {
            PowerUpEffects.Add(new Effect_IncreaseDamageFromGivenAbilityType(percentage_value, new(power_up_name)) { AbilityType = typeof(StealthAttack)});
        }
        else if (power_up_name == "BasicAttackDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack))});
        }
        else if (power_up_name == "AbilityDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique))});
        }
        else if (power_up_name == "StrongBasicAttackDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsBasicAttack && damage.SourceOfDamage.IsStrongBasicAttack == true))});
        }
        else if(power_up_name == "HealFromInflictingStagger") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    (damage.StaggerDealt > 0 
                && damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered)))), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealFromBurnDamage") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    (damage.Properites.Contains(Damage.DamageProperty.Burn) && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealFromBasicAttacks") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    (damage.SourceOfDamage.IsBasicAttack 
                && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealFromRipostesAndCounters") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    ((damage.SourceOfDamage.IsRiposte || damage.SourceOfDamage.IsCounter)
                && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealWhenUsingItems") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User == Player.Instance && ability.ItemBeingUsed != null)), ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum / 100 * effect.PercentageAmount;
                })});
        }
        else if(power_up_name == "HealFromMovement") {
            PowerUpEffects.Add(new Effect_HealFromMovement(new(power_up_name)) { HealAmount = percentage_value});
        }
        else if(power_up_name == "HealWhenApplyingStaggered") {
            PowerUpEffects.Add(new Effect_HealWhenEffectApplied(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Effect, bool>((effect) => 
                    (effect.GetType().IsSubclassOf(typeof(Effect_Staggered))
                && effect?.SourceOfEffect?.User == Player.Instance)), ActionOnDamage = new Action<Effect, Effect_HealWhenEffectApplied> ((damage, effect) =>  {
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealFromBleed") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.Properites != null && damage.Properites.Contains(Damage.DamageProperty.Bleed)
                )), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount;
                })});
        }
        else if(power_up_name == "HealMissingHealth") {
            PowerUpEffects.Add(new Effect_HealMissingHealth(new(power_up_name)) { HealAmount = percentage_value});
        }
        else if(power_up_name == "HealWhenApplyingCrowdControl") {
            PowerUpEffects.Add(new Effect_HealWhenEffectApplied(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Effect, bool>((effect) => 
                    ((effect.IsHardCrowdControl() || effect.IsSoftCrowdControl())&& !effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) 
                && effect?.SourceOfEffect?.User == Player.Instance)), ActionOnDamage = new Action<Effect, Effect_HealWhenEffectApplied> ((appliedEffect, triggeredEffect) =>  {
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum / 100 * triggeredEffect.HealAmount * appliedEffect.BaseDuration * (appliedEffect.TargetOfEffect.IsBoss ? 2 : 1) * (appliedEffect.IsHardCrowdControl() ? 1 : 0.5f);
                })});
        }
        else if(power_up_name == "HealFromEnergySpent") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User == Player.Instance)), ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    float cost = Ability.GetEnergyCost(ability.GetType());
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum / 1000 * effect.PercentageAmount * cost;
                })});
        }
        else if(power_up_name == "HealFromAbilities") {
            PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                    (damage.SourceOfDamage.IsTechnique
                && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
                })});
        }
        else if (power_up_name == "Injury")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value} );
        }
        else if (power_up_name == "Stagger")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value}   );
        }
        else if (power_up_name == "AttackSpeed")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value}   );
        }
        else if(power_up_name == "InjuryAndStagger") {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value} );
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value} );
        }
        else if(power_up_name == "EnergyGainFromBasicAttacks") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.BasicAttack});
        }
        else if(power_up_name == "EnergyGainFromStaggered") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.InflictedStaggered});
        }
        else if(power_up_name == "EnergyGainFromHealthLost") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.HealthLost});
        }
        else if(power_up_name == "EnergyGainFromRipostesAndCounters") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.Riposte});
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.Counter});
        }
        else if(power_up_name == "EnergyGainFromDodges") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = percentage_value, SpecifiedSource = Constants.EnergyGainSource.Dodge});
        }
        else if(power_up_name == "ReducedDamageAfterRiposteOrCounter") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User is Player && (ability.IsRiposte || ability.IsCounter))), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, customizableEffect) =>  {
                    if(ability.IsRiposte) {
                        if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterRiposte") != null) {
                            Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterRiposte").EndThisEffect();
                        }
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(power_up_name)) {PercentageAmount=percentage_value, ExtraInfo = "ReducedDamageAfterRiposte"}, 10);
                    }
                    else {
                        if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterCounter") != null) {
                            Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterCounter").EndThisEffect();
                        }
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(power_up_name)) {PercentageAmount=percentage_value * 2, ExtraInfo = "ReducedDamageAfterRiposte"}, 10);
                    }
            })});
        }
        else if(power_up_name == "ReducedDamageDuringBasicAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null && Player.Instance.Actions.CurrentAbilityBeingPerformed.IsBasicAttack))});
        }
        else if(power_up_name == "DamageToStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))))});
        }
        else if(power_up_name == "BurnDamageToStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.Properites.Contains(Damage.DamageProperty.Burn) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))))});
        }
        else if(power_up_name == "BurnDamageToNonStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.Properites.Contains(Damage.DamageProperty.Burn) && !damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))))});
        }
        else if(power_up_name == "BurnAlsoAddsOnslaught") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect.SourceOfEffect?.User == Player.Instance && effect.GetType() == typeof(Effect_Burn))), ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarted, effect) =>  {
                        effectStarted.TargetOfEffect.AddEffect(new Effect_Onslaught(effectStarted.DecayingAmount * effect.PercentageAmount / 100, effectStarted.SourceOfEffect));
                })});
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectEmpowered = new Func<Effect, Effect, bool>((effect1, effect2) => 
                    (effect2.SourceOfEffect?.User == Player.Instance && effect2.GetType() == typeof(Effect_Burn))), ActionOnEffectEmpowered = new Action<Effect, Effect, Effect_CustomizableEffectOnEvent> ((effect1, effect2, effect) =>  {
                        effect2.TargetOfEffect.AddEffect(new Effect_Onslaught(effect2.DecayingAmount * effect.PercentageAmount / 100, effect2.SourceOfEffect));
            })});
        }
        else if(power_up_name == "BurnAlsoAddsFreeze") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect.SourceOfEffect?.User == Player.Instance && effect.GetType() == typeof(Effect_Burn))), ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effectStarted, effect) =>  {
                        effectStarted.TargetOfEffect.AddEffect(new Effect_Freeze(effectStarted.DecayingAmount * effect.PercentageAmount / 100, effectStarted.SourceOfEffect));
                })});
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectEmpowered = new Func<Effect, Effect, bool>((effect1, effect2) => 
                    (effect2.SourceOfEffect?.User == Player.Instance && effect2.GetType() == typeof(Effect_Burn))), ActionOnEffectEmpowered = new Action<Effect, Effect, Effect_CustomizableEffectOnEvent> ((effect1, effect2, effect) =>  {
                        effect2.TargetOfEffect.AddEffect(new Effect_Freeze(effect2.DecayingAmount * effect.PercentageAmount / 100, effect2.SourceOfEffect));
            })});
        }
        else if(power_up_name == "BurnAlsoPenetrates") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { PenetrationChange = percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && damage.Properites.Contains(Damage.DamageProperty.Burn)))});
        }
        else if(power_up_name == "SlowPower") {

        }
        else if(power_up_name == "FreezeDuration") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect?.SourceOfEffect?.User == Player.Instance && (effect is Effect_Freeze || effect is Effect_FreezeInPlace))), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((triggeringEffect, customizableEffect) =>  {
                    triggeringEffect.BaseDuration *= (1 + customizableEffect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "SlowDuration") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect?.SourceOfEffect?.User == Player.Instance && effect is Effect_Slow)), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((triggeringEffect, customizableEffect) =>  {
                    triggeringEffect.BaseDuration *= (1 + customizableEffect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "DamageToFarEnemies") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.SourceOfDamage.User == Player.Instance && Vector2.Distance(Player.Instance.transform.position, damage.TargetOfDamage.transform.position) > 3))});
        }
        else if(power_up_name == "DamageFromSlowed") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value,   ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => 
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Slow)))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury *= effect.InjuryPercentageChange == 0 ? 1 : (1 - effect.InjuryPercentageChange / 100);
                    damage.Stagger *= effect.StaggerPercentageChange == 0 ? 1 : (1 - effect.StaggerPercentageChange / 100);
                })});
        }
        else if(power_up_name == "HealthRestored") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                    (stat.Owner == Player.Instance && stat is Health && amount > 0)), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    stat.ChangeCurrentValueWithoutInvoking(amount + amount * effect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "StaggerBarRestored") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => 
                    (stat.Owner == Player.Instance && stat is StaggerBar && amount < 0)), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    stat.ChangeCurrentValueWithoutInvoking(amount + amount * effect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "PronePower") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Prone), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, percentage_value, new(power_up_name)));
        }
        else if(power_up_name == "ProneDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Prone), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -percentage_value, new(power_up_name)));
        }
        else if(power_up_name == "ReducedDamageFromProne") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Prone))))});
        }
        else if(power_up_name == "ReducedDamageFromMissedCounters") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.IsCounterable))});
        }
        else if(power_up_name == "ReducedDamageFromUncounterable") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.IsUncounterable))});
        }
        else if(power_up_name == "ProneAlsoEmpowersStealthAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.GetType().IsSubclassOf(typeof(StealthAttack)) && damage.SourceOfDamage?.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Prone)))),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                float empowerAmount = ((Effect_Prone)damage.TargetOfDamage.GetEffect(typeof(Effect_Prone))).DecayingAmount;
                damage.ExtraInjuryDealtPercentage += empowerAmount;
                damage.ExtraStaggerDealtPercentage += empowerAmount;
            })});
        }
        else if(power_up_name == "ProneAlsoEmpowersBasicAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.IsBasicAttack && damage.SourceOfDamage?.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Prone)))),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                float empowerAmount = ((Effect_Prone)damage.TargetOfDamage.GetEffect(typeof(Effect_Prone))).DecayingAmount;
                damage.ExtraInjuryDealtPercentage += empowerAmount * effect.CustomParam / 100;
                damage.ExtraStaggerDealtPercentage += empowerAmount * effect.CustomParam / 100;
            })});
        }
        else if(power_up_name == "BurnPower") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, percentage_value, new(power_up_name)));
        }
        else if(power_up_name == "BurnDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -percentage_value, new(power_up_name)));
        }
        else if(power_up_name == "ReducedDamageFromBurning") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -percentage_value / 100, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))))});
        }
        else if(power_up_name == "DamageToIsolated") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.IsIsolated()))});
        }
        else if(power_up_name == "DecreasedIsolationThreshold") {
            PowerUpEffects.Add(new Effect_DecreasedIsolationThreshold(new(power_up_name)) { ThresholdMetersDecrease = 0.5f });
        }
        else if(power_up_name == "DamageFromIsolated") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value,   ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.IsIsolated())),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.Injury *= effect.InjuryPercentageChange == 0 ? 1 : (1 - effect.InjuryPercentageChange / 100);
                    damage.Stagger *= effect.StaggerPercentageChange == 0 ? 1 : (1 - effect.StaggerPercentageChange / 100);
                })});
        }
        else if(power_up_name == "StealthAttackCooldown") {
            PowerUpEffects.Add(new Effect_LowerStealthAttackImmunity(new(power_up_name)) { ImmunityDurationReductionInSeconds = 5f });
        }
        else if(power_up_name == "DamageToLowHealth") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Health.Current < damage.TargetOfDamage.Health.Maximum * 0.3f))});
        }
        else if(power_up_name == "MagicAttackSpeed") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.MagicAttackSpeed, new(power_up_name)) {PercentageAmount = percentage_value, FlatAmount = flat_value}  );
        }
        else if(power_up_name == "MagicCooldownRefund") {
            PowerUpEffects.Add(new Effect_RefundCooldownForDamageCategory(Constants.DamageType.Magic, new(power_up_name)) { RefundAmount = percentage_value});
        }
        else if(power_up_name == "MagicCrowdControlDuration") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect?.SourceOfEffect?.User == Player.Instance && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.DamageType == Constants.DamageType.Magic && (effect.IsHardCrowdControl() || effect.IsSoftCrowdControl()))), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((triggeringEffect, customizableEffect) =>  {
                    triggeringEffect.BaseDuration *= (1 + customizableEffect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "MagicEnergyRefund") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                    (ability.User == Player.Instance && ability.DamageType == Constants.DamageType.Magic)), ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.Energy.Current += Ability.GetEnergyCost(ability.GetType()) * effect.PercentageAmount / 100;
                })});
        }
        else if(power_up_name == "HeavyAbilityInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Heavy))});
        }
        else if(power_up_name == "LightAbilityInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Light))});
        }
        else if(power_up_name == "RangedAbilityInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Ranged))});
        }
        else if(power_up_name == "HeavyAbilityInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Heavy))});
        }
        else if(power_up_name == "LightAbilityStagger") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Light))});
        }
        else if(power_up_name == "RangedAbilityStagger") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Ranged))});
        }
        else if(power_up_name == "UltimateAbilityDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && damage.SourceOfDamage.GetType().GetField("IsUltimate", BindingFlags.Public | BindingFlags.Static).GetValue(null) != null))});
        }
        else if(power_up_name == "PropriusAbilityDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.IsTechnique && ((Ability.AbilityFamily)damage.SourceOfDamage.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static).GetValue(null)) == Ability.AbilityFamily.Proprius))});
        }
        else if(power_up_name == "ItemDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.ItemBeingUsed != null))});
        }
        else if(power_up_name == "StunDuration") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect?.SourceOfEffect?.User == Player.Instance && effect is Effect_Stun)), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((triggeringEffect, customizableEffect) =>  {
                    triggeringEffect.BaseDuration *= (1 + customizableEffect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "HeavyCooldownRefund") {
            PowerUpEffects.Add(new Effect_RefundCooldownForDamageCategory(Constants.DamageType.Heavy, new(power_up_name)) { RefundAmount = percentage_value});
        }
        else if(power_up_name == "LightCooldownRefund") {
            PowerUpEffects.Add(new Effect_RefundCooldownForDamageCategory(Constants.DamageType.Light, new(power_up_name)) { RefundAmount = percentage_value});
        }
        else if(power_up_name == "RangedCooldownRefund") {
            PowerUpEffects.Add(new Effect_RefundCooldownForDamageCategory(Constants.DamageType.Ranged, new(power_up_name)) { RefundAmount = percentage_value});
        }
        else if(power_up_name == "HeavyStun" || power_up_name == "LightStun" || power_up_name == "RangedStun" || power_up_name == "MagicStun") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage?.User == Player.Instance && damage.SourceOfDamage.DamageType == (power_up_name == "HeavyStun" ? Constants.DamageType.Heavy : power_up_name == "LightStun" ? Constants.DamageType.Light : power_up_name == "RangedStun" ? Constants.DamageType.Ranged : Constants.DamageType.Magic) && Player.Instance.EffectCooldowns.FirstOrDefault(cd => cd.ExtraInfo == power_up_name) == null)),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Stun(new(damage.SourceOfDamage)), 2);
                    Player.Instance.AddCooldown(new Cooldown(null, 10, Player.Instance) {ExtraInfo = power_up_name});
                })});
        }
        else if(power_up_name == "ItemDuration") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => 
                    (effect?.SourceOfEffect?.User == Player.Instance && effect.SourceOfEffect.SourceAbility != null && effect.SourceOfEffect.SourceAbility.ItemBeingUsed != null)), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((triggeringEffect, customizableEffect) =>  {
                    triggeringEffect.BaseDuration *= (1 + customizableEffect.PercentageAmount / 100);
                })});
        }
        else if(power_up_name == "DamageToStunned") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Stun))))});
        }
        else if(power_up_name == "IgnisManor_WeaponTraining") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyInjury, new(power_up_name)) {PercentageAmount = 10 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyStagger, new(power_up_name)) {PercentageAmount = 10 });
        }
        else if(power_up_name == "IgnisManor_BackstabPowerUp") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                damage.SourceOfDamage.IsStealthAttack && damage.SourceOfDamage?.User == Player.Instance && damage.IsDamageOverTime == false),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                damage.TargetOfDamage.AddEffect(new Effect_Burn(Player.Instance.HeavyStagger.Current * 0.05f, new(damage.SourceOfDamage)));
            })});
        }
        else if(power_up_name == "IgnisManor_BurningPowerUp") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(power_up_name)));
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -0.05f, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))))});
        }
        else if(power_up_name == "IgnisVolcano_BuriedEnergy") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 10, new(power_up_name)));
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -10, new(power_up_name)));
        }
        else if(power_up_name == "IgnisVolcano_FireRiver") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { MultiplierChange = -0.1f, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.TargetOfDamage == Player.Instance && damage.Properites.Contains(Damage.DamageProperty.Burn)))});
        }
        else if(power_up_name == "IgnisManorOnFire_BurningSword") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyInjury, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyStagger, new(power_up_name)) {PercentageAmount = 15 });
        }
        else if(power_up_name == "IgnisManorOnFire_MuseumSword") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 15, new(power_up_name)));
        }
        else if(power_up_name == "IgnisManorOnFire_MuseumArmour") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.Health, new(power_up_name)) {FlatAmount = -250 });
        }
        else if(power_up_name == "AnimaIsland_3MajorDuels") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyInjury, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyStagger, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.LightInjury, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.LightStagger, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.RangedInjury, new(power_up_name)) {PercentageAmount = 15 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.RangedStagger, new(power_up_name)) {PercentageAmount = 15 });
        }
        else
        {
            Debug.LogError("Power Up behavior not defined for: " + power_up_name);
        }
        return PowerUpEffects;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.ShowPowerUpDetails(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.Instance.ShowPowerUpDetails(this);
        MenuManager.Instance.SetGamepadIndicator(gameObject);
    }

    public void OnDeselect(BaseEventData eventData) {
        CanvasElements.MenuCanvas.GamepadIndicator.SetActive(false);
    }

    public void UpdateUnlockedStatus() {
        if(string.IsNullOrWhiteSpace(Tree)) {
            Start();
        }
        GetComponent<Image>().color = SaveFile.Instance.UnlockedPowerUps.Contains(Id) ? Colors.Gold : Color.black; 
        CheckIfRowIsActive(transform.parent.Find("Active"));
        MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Label").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
    }

    public void CheckIfRowIsActive(Transform row) {
        bool isActive = false;
        for(int i = 1; i < 5; i++) {
            if(transform.parent.Find(i.ToString()) != null && SaveFile.Instance.UnlockedPowerUps.Contains(transform.parent.Find(i.ToString()).GetComponent<PassivePowerUpTile>().Id)) {
                isActive = true;
            }
        }
        row.gameObject.SetActive(isActive);
        if(Row != "12" && !String.IsNullOrEmpty(Row)) {
            int nextRow = Int32.Parse(Row) + 1;
            bool highEnoughLevel = (nextRow < 4 || SaveFile.Instance.Level >= 5) && (nextRow < 7 || SaveFile.Instance.Level >= 15) && (nextRow < 10 || SaveFile.Instance.Level >= 30);
            for(int i = 1; i < 5; i++) {
                if(i == 3 && Row == "11") {
                    break;
                }
                transform.parent.parent.Find((Int32.Parse(Row) + 1) + "/" + i).GetComponent<PassivePowerUpTile>().IsAvailable = highEnoughLevel && isActive;
            }
            transform.parent.parent.Find((Int32.Parse(Row) + 1).ToString() + "/Disabled").gameObject.SetActive(!highEnoughLevel || !isActive);
        }
    }

    public void CheckIfNextTierIsUnlocked(Transform row) {

    }
}
