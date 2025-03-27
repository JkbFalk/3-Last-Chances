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
    public string ScalingUsed;
    public string[] MultipliersAndPenalties;
    public float[] SplitPowerBudget;
    public bool IsSkillTreeSpeciality = true;
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
        PowerUpEffects.AddRange(GetPassivePowerUpEffects(power_up_name, GetCalculatedPowerBudgetForTile()));
        SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] = SaveFile.Instance.UnlockedPowerUps.Where(p => p.StartsWith(Tree)).Count();
        MenuManager.Instance.transform.Find("Skill Tree Window/Category Selection/" + Tree + "/Label").GetComponent<TextMeshProUGUI>().text = Tree + " (" + SaveFile.Instance.PointsPutIntoEachSkillTree[Tree] + ")";
    }

    public void ApplyPowerUpOfThisTile() {
        string[] power_ups = PowerUp.Split("+");
        PowerUpEffects = new List<Effect>();
        for(int i = 0; i < power_ups.Length; i++) {
            AddPassivePowerUp(power_ups[i], GetCalculatedPowerBudgetForTile());
        }
        foreach(Effect e in PowerUpEffects) {
            e.IsRemovable = false;
            e.ShowsInMenu=false;
            Player.Instance.AddEffect(e);
        }
    }

    public int GetCalculatedPowerBudgetForTile() {
        float amount = 0;
        if(Row == "1" || Row == "2" || Row == "3") {
            amount = IsSkillTreeSpeciality ? Constants.SPECIALITY_SKILL_TREE_TIER1_PB : Constants.NON_SPECIALITY_SKILL_TREE_TIER1_PB;
        }
        else if(Row == "4" || Row == "5" || Row == "6") {
            amount = IsSkillTreeSpeciality ? Constants.SPECIALITY_SKILL_TREE_TIER2_PB : Constants.NON_SPECIALITY_SKILL_TREE_TIER2_PB;
        }
        else if(Row == "7" || Row == "8" || Row == "9") {
            amount = IsSkillTreeSpeciality ? Constants.SPECIALITY_SKILL_TREE_TIER3_PB : Constants.NON_SPECIALITY_SKILL_TREE_TIER3_PB;
        }
        else if(Row == "10" || Row == "11" || Row == "12") {
            amount = IsSkillTreeSpeciality ? Constants.SPECIALITY_SKILL_TREE_TIER4_PB : Constants.NON_SPECIALITY_SKILL_TREE_TIER4_PB;
        }
        else {
            Debug.LogError("Incorrect row name for passive power up tile: " + Utils.GetGameObjectPath(gameObject));
            return 0;
        }
        Debug.Log("CALCULATED PB1: " + amount + " -> " + ((int)amount));
        FieldInfo scaling = typeof(Constants).GetField(ScalingUsed, BindingFlags.Public | BindingFlags.Static);
        if(String.IsNullOrWhiteSpace(ScalingUsed) || scaling == null) {
            Debug.LogError($"Incorrect scaling ({ScalingUsed}) used  for passive power up tile: {Utils.GetGameObjectPath(gameObject)}");
            return 0;
        }
        amount = (float)scaling.GetValue(null) * amount;
        Debug.Log("CALCULATED PB2: " + amount + " -> " + ((int)amount));
        if(MultipliersAndPenalties != null && MultipliersAndPenalties.Length > 0) {
            foreach(string multiName in MultipliersAndPenalties) {
                FieldInfo multi = typeof(Constants).GetField(multiName, BindingFlags.Public | BindingFlags.Static);
                if(multi == null) {
                    Debug.LogError($"Incorrect multiplier name {multiName} for passive power up tile: {Utils.GetGameObjectPath(gameObject)}");
                    return 0;
                }
                amount *= (float)multi.GetValue(null);
            }
        }
        Debug.Log("CALCULATED PB3: " + amount + " -> " + ((int)amount));
        return (int)amount;
    }

    public static List<Effect> GetPassivePowerUpEffects(string power_up_name, float power_budget = 0) {
        List<Effect> PowerUpEffects = new List<Effect>();
        if (power_up_name == "Health" || power_up_name == "StaggerBar")
        {
        PowerUpEffects.Add(new Effect_ChangeStat(Utils.GetPlayerStatForGivenName(power_up_name), new(power_up_name)) {FlatAmount = power_budget}  );
        }
        else if (Utils.GetPlayerStatForGivenName(power_up_name) != null)
        {
            PowerUpEffects.Add(new Effect_ChangeStat(Utils.GetPlayerStatForGivenName(power_up_name), new(power_up_name)) {PercentageAmount = power_budget}  );
        }
        else if (power_up_name == "Injury")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new(power_up_name)) {PercentageAmount = power_budget} );
        }
        else if (power_up_name == "Stagger")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, new(power_up_name)) {PercentageAmount = power_budget}   );
        }
        else if (power_up_name == "AttackSpeed")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new(power_up_name)) {PercentageAmount = power_budget}   );
        }
        else if (power_up_name == "Damage")
        {
            PowerUpEffects.Add(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Damage, new(power_up_name)) {PercentageAmount = power_budget}   );
        }




















        // Anima
        else if(power_up_name == "DamageReductionAgainstCounterable") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter)))});
        }
        else if (power_up_name == "BasicAttackDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)))});
        }
        else if (power_up_name == "WeaponTechniqueDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage)});
        }
        else if (power_up_name == "WeaponDamage")
        {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyInjury, new(power_up_name)) {PercentageAmount = power_budget} );
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyStagger, new(power_up_name)) {PercentageAmount = power_budget} );
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.LightInjury, new(power_up_name)) {PercentageAmount = power_budget} );
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.LightStagger, new(power_up_name)) {PercentageAmount = power_budget} );
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.RangedInjury, new(power_up_name)) {PercentageAmount = power_budget} );
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.RangedStagger, new(power_up_name)) {PercentageAmount = power_budget} );
        }      
        else if(power_up_name == "SharpAmount") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "SharpDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Sharp), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -power_budget, new(power_up_name)));
        }
        else if(power_up_name == "SharpDamageReduction") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance &&     Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))))});
        }
        else if(power_up_name == "DamageReductionAgainstUnstoppable") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Unstoppable)))});
        }
        else if(power_up_name == "DamageReductionAgainstUnstoppable") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Unstoppable)))});
        }
        else if (power_up_name == "RiposteDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte)))});
        }
        else if (power_up_name == "SharpEmpowersBasicAttacks")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp)))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                    damage.ExtraInjuryDealtPercentage += sharp.DecayingAmount * effect.InjuryPercentageChange / 100;
                    damage.ExtraStaggerDealtPercentage += sharp.DecayingAmount * effect.StaggerPercentageChange / 100;
            })});
        }
        else if (power_up_name == "DamageReductionAfterRiposteOrCounter")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => (ability.User == Player.Instance && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter)))), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Effect extraDR = new Effect_ChangeStat(Player.Instance.DamageReduction, new(ability)) {PercentageAmount = power_budget, ShowsInUI = true, BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, Identifier="DamageReductionAfterRiposteOrCounter", EffectIndicatorText=power_budget + "%"};
                    Player.Instance.AddEffect(extraDR, ability.Is(Ability.AbilityProperty.Counter) ? 10 : 5);
            })});
        }
        else if (power_up_name == "CounterDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter)))});
        }
        else if (power_up_name == "StrongBasicAttackDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.StrongBasicAttack)))});
        }
        else if (power_up_name == "SharpEmpowersWeaponTechniques")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.IsWeaponDamage && Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Sharp sharp = (Effect_Sharp)Player.Instance.GetEffect(typeof(Effect_Sharp));
                    damage.ExtraInjuryDealtPercentage += sharp.DecayingAmount * effect.InjuryPercentageChange / 100;
                    damage.ExtraStaggerDealtPercentage += sharp.DecayingAmount * effect.StaggerPercentageChange / 100;
            })});
        }
        else if(power_up_name == "DamageReductionDuringBasicAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null &&  Player.Instance.Actions.CurrentAbilityBeingPerformed.Is(Ability.AbilityProperty.BasicAttack)))});
        }
        else if(power_up_name == "HealFromBasicAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }
        else if(power_up_name == "HealFromRipostesAndCounters") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && (damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }

















        
        // Ignis
        else if(power_up_name == "DamageToStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered)))});
        }
        else if (power_up_name == "BasicAttackStagger")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)))});
        }
        else if (power_up_name == "TechniqueStagger")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)))});
        }
        else if(power_up_name == "BurnAmount") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "BurnDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -power_budget, new(power_up_name)));
        }
        else if(power_up_name == "BurnDamageReduction") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Burn))))});
        }
        else if (power_up_name == "HeavyStrongBasicAttackStagger")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.StrongBasicAttack) && damage.DamageType == Constants.DamageType.Heavy)});
        }
        else if (power_up_name == "ExtraMagicStaggerToEmptyStaggerBar")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic && damage.TargetOfDamage.StaggerBar.Current < 1)});
        }
        else if(power_up_name == "BurnAmountToNonBurning") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, power_budget, new(power_up_name)) {
                ConditionForEffectPowerChange = new Func<Unit, bool>(target => target.CheckIfUnderEffect(typeof(Effect_Burn)) == false)});
        }
        else if(power_up_name == "DamageReductionAgainstBosses") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.IsBoss))});
        }
        else if (power_up_name == "BurnEmpowersStagger")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Burn)))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Burn burn = (Effect_Burn)damage.TargetOfDamage.GetEffect(typeof(Effect_Burn));
                    damage.ExtraStaggerDealtPercentage += burn.DecayingAmount * effect.StaggerPercentageChange / 100;
            })});
        }
        else if (power_up_name == "BurnExplosionDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.BurnExplosion)))});
        }
        else if(power_up_name == "DamageReductionAgainstNonBosses") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && !damage.SourceOfDamage.User.IsBoss))});
        }
        else if(power_up_name == "HealFromStackingEffects") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && (damage.Properties.Contains(Damage.DamageProperty.Burn) || damage.Properties.Contains(Damage.DamageProperty.Freeze) || damage.Properties.Contains(Damage.DamageProperty.Incision))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }
        else if(power_up_name == "HealFromStaggerToNonStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && !damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }


















        //Glacies
        else if (power_up_name == "RangedDamageLowersTenacity")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = -power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Ranged), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {PathToEffectGraphic="UI/Control", PercentageAmount = power_budget, ShowsInUI = true, BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, Identifier="RangedDamageLowersTenacity", EffectIndicatorText=power_budget + "%"};
                    damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
            })});
        }
        else if (power_up_name == "MagicDamageLowersTenacity")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = -power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect decreasedTenacity = new Effect_ChangeStat(damage.TargetOfDamage.Tenacity, new(damage.SourceOfDamage)) {PathToEffectGraphic="UI/Control", PercentageAmount = power_budget, ShowsInUI = true, BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, Identifier="MagicDamageLowersTenacity", EffectIndicatorText=power_budget + "%"};
                    damage.TargetOfDamage.AddEffect(decreasedTenacity, 10);
            })});
        }
        else if (power_up_name == "StaggeringLowersTenacity")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = -power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.SourceOfEffect.User == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect decreasedTenacity = new Effect_ChangeStat(effect_started.TargetOfEffect.Tenacity, effect_started.SourceOfEffect) {PathToEffectGraphic="UI/Control", PercentageAmount = power_budget, ShowsInUI = true, BehaviourWhenDuplicateEffect = Effect.BehaviourWhenDuplicateEffectEnum.EndShorterDuplicateWithSameIdentifier, Identifier="StaggeringLowersTenacity", EffectIndicatorText=power_budget + "%"};
                    effect_started.TargetOfEffect.AddEffect(decreasedTenacity, 30);
            })});
        }
        else if (power_up_name == "MovementSpeedWhileInRangedStance")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = -power_budget, 
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => ability.GetType().IsSubclassOf(typeof(Ability_StanceSwitch))), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect_ChangeStat msBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "MovementSpeedWhileInRangedStance");
                    if(Player.Instance.CurrentStance.DamageCategory != Constants.DamageType.Ranged && msBuff != null) {
                        msBuff.EndThisEffect();
                    }
                    else {
                        msBuff = new Effect_ChangeStat(Player.Instance.MovementSpeed, new("MovementSpeedWhileInRangedStance")) {PercentageAmount = effect.PercentageAmount, Identifier="MovementSpeedWhileInRangedStance", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"};
                        Player.Instance.AddEffect(msBuff);
                    }
            })});
        }
        else if (power_up_name == "StaggeringEnemyGivesAmmo")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = -power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect.GetType().IsSubclassOf(typeof(Effect_Staggered)) && effect.SourceOfEffect.User == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Player.Instance.Ammo++;
            })});
        }
        else if (power_up_name == "ControlWhileNoAmmo")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForAmmoAmountChanged = new Func<bool>(() => true), 
                ActionOnAmmoAmountChanged = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                    Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "ControlWhileNoAmmo");
                    if(Player.Instance.Ammo > 0 && controlBuff != null) {
                        controlBuff.EndThisEffect();
                    }
                    else if(Player.Instance.Ammo == 0 && controlBuff == null) {
                        controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileNoAmmo")) {PercentageAmount = effect.PercentageAmount, Identifier="ControlWhileNoAmmo", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"};
                        Player.Instance.AddEffect(controlBuff);
                    }
            })});
        }
        else if (power_up_name == "ControlWhileLowHealth")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => stat.Owner == Player.Instance && stat is Health), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    Effect_ChangeStat controlBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "ControlWhileLowHealth");
                    if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.25f && controlBuff != null) {
                        controlBuff.EndThisEffect();
                    }
                    else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.25f && controlBuff == null) {
                        controlBuff = new Effect_ChangeStat(Player.Instance.Control, new("ControlWhileLowHealth")) {PercentageAmount = effect.PercentageAmount, Identifier="ControlWhileLowHealth", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"};
                        Player.Instance.AddEffect(controlBuff);
                    }
            })});
        }
        else if(power_up_name == "DamageToCrowdControlled") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl))});
        }
        else if(power_up_name == "FinalAmmoDealsMoreDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.IsFinalAmmo)))});
        }
        else if(power_up_name == "DamageToFrozen") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Frozen))))});
        }
        else if(power_up_name == "HealFromApplyingCC") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect.GetType().IsSubclassOf(typeof(Effect_HardCrowdControl)) && effect.SourceOfEffect.User == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum / 100 * effect.PercentageAmount * effect_started.RemainingDuration;
            })});
        }
        else if(power_up_name == "HealFromDamageToStaggered") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Staggered))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }


















        //Molis
        else if(power_up_name == "BarrierDamageReduction") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Barrier))))});
        }
        else if(power_up_name == "ConvertStaggerBarToHealth") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.Health, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[R]"});
        }
        else if(power_up_name == "ConvertHealthToStaggerBar") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.StaggerBar, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[P]"});
        }
        else if(power_up_name == "ConvertDamageReductionToTenacity") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.DamageReduction, Player.Instance.Tenacity, power_budget, 10000, new(power_up_name) ));
        }
        else if(power_up_name == "ConvertTenacityToDamageReduction") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Tenacity, Player.Instance.DamageReduction, power_budget, 10000, new(power_up_name) ));
        }
        else if(power_up_name == "BarrierAmount") {
        PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "BarrierDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Barrier), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -power_budget, new(power_up_name)));
        }
        else if (power_up_name == "MoreTenacityWhileBarrier")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect tenacityBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "MoreTenacityWhileBarrier"));
                    if(tenacityBuff == null) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Tenacity, new (power_up_name)) {PercentageAmount = effect.PercentageAmount, Identifier="MoreTenacityWhileBarrier", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"});
                    }}), 
                ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect tenacityBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "MoreTenacityWhileBarrier"));
                    if(tenacityBuff != null) {
                        tenacityBuff.EndThisEffect();
                    }
            })});
        }
        else if (power_up_name == "MoreDamageReductionWhileCCed")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentActionBeingPerformed == Constants.ActionType.UnderHardCrowdControl))});
        }
        else if(power_up_name == "ConvertHealthToInjury") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.HeavyInjury, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[R]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.LightInjury, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[R]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.RangedInjury, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[R]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.Health, Player.Instance.MagicInjury, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[R]", StatToConvertIntoColor = "[R]"});
        }
        else if(power_up_name == "EnergyGainFromHealthLost") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.HealthLost});
        }
        else if (power_up_name == "HealthRegenWhileBarrier")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { FlatAmount = power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "HealthRegenWhileBarrier"));
                    if(regenBuff == null) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new (power_up_name)) {RegenerationFlatAmount = effect.FlatAmount, Identifier="HealthRegenWhileBarrier", IsRemovable=false, ShowsInUI=true, PathToEffectGraphic="UI/HealthRegeneration", EffectIndicatorText=effect.FlatAmount.ToString()});
                    }}), 
                ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "HealthRegenWhileBarrier"));
                    if(regenBuff != null) {
                        regenBuff.EndThisEffect();
                    }
            })});
        }
        else if (power_up_name == "DamageReductionWhileLowHealth")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget,
                ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => stat.Owner == Player.Instance && stat is Health), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "DamageReductionWhileLowHealth");
                    if(Player.Instance.Health.Current > Player.Instance.Health.Maximum * 0.5f && drBuff != null) {
                        drBuff.EndThisEffect();
                    }
                    else if(Player.Instance.Health.Current <= Player.Instance.Health.Maximum * 0.5f && drBuff == null) {
                        drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, new("DamageReductionWhileLowHealth")) {PercentageAmount = effect.PercentageAmount, Identifier="DamageReductionWhileLowHealth", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"};
                        Player.Instance.AddEffect(drBuff);
                    }
            })});
        }
        else if(power_up_name == "ConvertStaggerBarToStagger") {
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.HeavyStagger, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[P]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.LightStagger, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[P]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.RangedStagger, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[P]"});
            PowerUpEffects.Add(new Effect_GainPercentageOfStatAsAnotherStat(Player.Instance.StaggerBar, Player.Instance.MagicStagger, power_budget, 10000, new(power_up_name) ) {StatToTakePercentageFromColor = "[P]", StatToConvertIntoColor = "[P]"});
        }
        else if(power_up_name == "EnergyGainFromBlocking") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.Block});
        }
        else if (power_up_name == "StaggerBarRegenWhileBarrier")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { FlatAmount = -power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "StaggerBarRegenWhileBarrier"));
                    if(regenBuff == null) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.StaggerBar, new (power_up_name)) {RegenerationFlatAmount = effect.FlatAmount, Identifier="StaggerBarRegenWhileBarrier", IsRemovable=false, ShowsInUI=true, PathToEffectGraphic="UI/StaggerBarRegeneration", EffectIndicatorText=effect.FlatAmount.ToString()});
                    }}), 
                ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => effect is Effect_Barrier && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect regenBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "StaggerBarRegenWhileBarrier"));
                    if(regenBuff != null) {
                        regenBuff.EndThisEffect();
                    }
            })});
        }
        else if (power_up_name == "DamageReductionWhileHighStaggerBar")
        {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForUnitStatCurrentAmountChanged = new Func<Stat, float, bool>((stat, amount) => stat.Owner == Player.Instance && stat is Health), 
                ActionOnUnitStatCurrentAmountChanged = new Action<Stat, float, Effect_CustomizableEffectOnEvent> ((stat, amount, effect) =>  {
                    Effect_ChangeStat drBuff = (Effect_ChangeStat)Player.Instance.CurrentEffects.FirstOrDefault(effect => effect.Identifier == "DamageReductionWhileHighStaggerBar");
                    if(Player.Instance.StaggerBar.Current < Player.Instance.StaggerBar.Maximum * 0.5f && drBuff != null) {
                        drBuff.EndThisEffect();
                    }
                    else if(Player.Instance.StaggerBar.Current >= Player.Instance.StaggerBar.Maximum * 0.5f && drBuff == null) {
                        drBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, new("DamageReductionWhileHighStaggerBar")) {PercentageAmount = effect.PercentageAmount, Identifier="DamageReductionWhileHighStaggerBar", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"};
                        Player.Instance.AddEffect(drBuff);
                    }
            })});
        }
        else if(power_up_name == "HealMissingHealth") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForOneFifthSecondElapsedNotRealtime = new Func<bool>(() => Player.Instance.Health.Current < Player.Instance.Health.Maximum), 
                ActionOnOneFifthSecondElapsedNotRealtime = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                    Player.Instance.Health.Current += (Player.Instance.Health.Maximum - Player.Instance.Health.Current) * effect.PercentageAmount / 100 / 5;
            })});
        }
        else if(power_up_name == "HealFromStaggerTaken") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.TargetOfDamage == Player.Instance && damage.StaggerDealt > 0 && !Player.Instance.CheckIfUnderEffect(typeof(Effect_PlayerStaggered))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }


















        //Salutis
        else if(power_up_name == "SalutisTechniqueCooldownReduction") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="TechniqueCooldownReduction", PercentageAmount = power_budget});
        }
        else if(power_up_name == "TechniqueCooldownReduction") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="TechniqueCooldownReduction", PercentageAmount = power_budget});
        }
        else if(power_up_name == "ToolCooldownReduction") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="ToolCooldownReduction", PercentageAmount = power_budget});
        }
        else if(power_up_name == "EffectCooldownReduction") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="EffectCooldownReduction", PercentageAmount = power_budget});
        }
        else if(power_up_name == "InjuryToIncised") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Incision))))});
        }
        else if(power_up_name == "IncisionAmount") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "IncisionDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -power_budget, new(power_up_name)));
        }
        else if(power_up_name == "BackstabInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)))});
        }
        else if(power_up_name == "TechniqueInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                    (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)))});
        }
        else if(power_up_name == "BackstabsReduceCooldowns") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => (ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.Backstab))), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    foreach(Cooldown cd in Player.Instance.TechniqueCooldowns) {
                        cd.RemainingDuration = cd.RemainingDuration - cd.RemainingDuration * effect.PercentageAmount / 100;
                    }
                    foreach(Cooldown cd in Player.Instance.EffectCooldowns) {
                        cd.RemainingDuration = cd.RemainingDuration - cd.RemainingDuration * effect.PercentageAmount / 100;
                    }
                    Player.Instance.ToolCooldown.RemainingDuration = Player.Instance.ToolCooldown.RemainingDuration - Player.Instance.ToolCooldown.RemainingDuration * effect.PercentageAmount / 100;
            })});
        }
        else if(power_up_name == "BackstabsGiveEnergy") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { FlatAmount = power_budget, 
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => (ability.User == Player.Instance && ability.Is(Ability.AbilityProperty.Backstab))), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.Energy.GenerateEnergy(effect.FlatAmount);
            })});
        }
        else if(power_up_name == "BackstabsDealMoreDamageBasedOnIncision") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab) && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Incision))), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Incision incision = (Effect_Incision)damage.TargetOfDamage.GetEffect(typeof(Effect_Incision));
                    damage.ExtraInjuryDealtPercentage += incision.DecayingAmount * effect.InjuryPercentageChange / 100;
                    damage.ExtraStaggerDealtPercentage += incision.DecayingAmount * effect.StaggerPercentageChange / 100;
            })});
        }
        else if(power_up_name == "BasicAttackInjury") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)))});
        }
        else if(power_up_name == "BackstabsScaleWithOnslaughtSharpAndAnalysis") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Sharp))) {
                        Effect_Sharp sharp = (Effect_Sharp)damage.TargetOfDamage.GetEffect(typeof(Effect_Sharp));
                        damage.ExtraInjuryDealtPercentage += sharp.DecayingAmount * effect.InjuryPercentageChange / 100;
                        damage.ExtraStaggerDealtPercentage += sharp.DecayingAmount * effect.StaggerPercentageChange / 100;
                    }
                    if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis))) {
                        Effect_Analysis analysis = (Effect_Analysis)damage.TargetOfDamage.GetEffect(typeof(Effect_Analysis));
                        damage.ExtraInjuryDealtPercentage += analysis.DecayingAmount * effect.InjuryPercentageChange / 100;
                        damage.ExtraStaggerDealtPercentage += analysis.DecayingAmount * effect.StaggerPercentageChange / 100;
                    }
                    if(Player.Instance.CheckIfUnderEffect(typeof(Effect_Onslaught))) {
                        Effect_Onslaught onslaught = (Effect_Onslaught)damage.TargetOfDamage.GetEffect(typeof(Effect_Onslaught));
                        damage.ExtraInjuryDealtPercentage += onslaught.DecayingAmount * effect.InjuryPercentageChange / 100;
                        damage.ExtraStaggerDealtPercentage += onslaught.DecayingAmount * effect.StaggerPercentageChange / 100;
                    }
            })});
        }
        else if(power_up_name == "BackstabsApplyIncision") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget,
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    damage.TargetOfDamage.AddEffect(new Effect_Incision(damage.InjuryDealt * effect.CustomParam + damage.StaggerDealt * effect.CustomParam, new(damage.SourceOfDamage)));
            })});
        }
        else if(power_up_name == "HealFromBackstabs") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget,     
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                        Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.InjuryPercentageChange + damage.StaggerDealt / 100 * effect.StaggerPercentageChange;
            })});
        }
        else if(power_up_name == "HealFromTakedowns") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
            ConditionCheckForUnitKnockedOut = new Func<Damage, bool>((damage) => damage.TargetOfDamage.IsHostile), 
            ActionOnUnitKnockedOut = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
            }),
            ConditionCheckForHealthBarBroken = new Func<Damage, bool>((damage) => damage.TargetOfDamage.IsHostile), 
            ActionOnHealthBarBroken = new Action<Damage, Effect_CustomizableEffectOnEvent> ((damage, effect) =>  {
                Player.Instance.Health.Current += Player.Instance.Health.Maximum * effect.PercentageAmount / 100;
            })
            });
        }
























        //Tonitrui
        else if(power_up_name == "SuperchargeAmount") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "SuperchargeDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Supercharge), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -power_budget, new(power_up_name)));
        }
        else if(power_up_name == "SuperchargeMovementSpeed") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "SuperchargeMovementSpeed"));
                    if(msBuff == null) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.MovementSpeed, new (power_up_name)) {PercentageAmount = effect.PercentageAmount, Identifier="SuperchargeMovementSpeed", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"});
                    }}), 
                ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => effect is Effect_Supercharge && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "SuperchargeMovementSpeed"));
                    if(msBuff != null) {
                        msBuff.EndThisEffect();
                    }
            })});
        }
        else if(power_up_name == "SuperchargeDamageReduction") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Supercharge))))});
        }
        else if(power_up_name == "SuperchargeInjuryDealt") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Supercharge)))});
        }
        else if(power_up_name == "SpendingSuperchargeGeneratesBarrier") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="SpendingSuperchargeGeneratesBarrier", PercentageAmount = power_budget});
        }
        else if(power_up_name == "SuperchargeInjuryFromBasicAttacks") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.SourceOfDamage.User == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Supercharge)))});
        }
        else if(power_up_name == "ExtraDamageReductionBasedOnMovementSpeed") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = Player.Instance.MovementSpeed.Current * power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.TargetOfDamage == Player.Instance && Player.Instance.MovementSpeed.Current > 1)});
        }
        else if(power_up_name == "HealFromDistanceTravelled") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForOneFifthSecondElapsedNotRealtime = new Func<bool>(() => Player.Instance.Health.Current < Player.Instance.Health.Maximum), 
                ActionOnOneFifthSecondElapsedNotRealtime = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                    if(Player.Instance.PlayerSavedPosition == null) {
                        Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
                    }
                    Player.Instance.Health.Current += Vector2.Distance(Player.Instance.PlayerSavedPosition, Player.Instance.transform.position) * effect.PercentageAmount / 100 / 5;
                    Player.Instance.PlayerSavedPosition = Player.Instance.transform.position;
            })});
        }
        else if(power_up_name == "HealMoreFromHealthPotions") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => (ability.User == Player.Instance && ability is Ability_Heal)), 
                ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, effect) =>  {
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new(power_up_name)) {RegenerationPercentageAmount = effect.PercentageAmount / 2 }, 2);
            })});
        }
        
        





























        //Proprius
        else if(power_up_name == "EnergyGainFromBasicAttacks") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.BasicAttack});
        }
        else if(power_up_name == "EnergyGainFromDodges") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.Dodge});
        }
        else if(power_up_name == "EnergyGainFromRipostesAndCounters") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.Riposte});
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.Counter});
        }
        else if(power_up_name == "EnergyGainFromStaggering") {
            PowerUpEffects.Add(new Effect_GainMoreEnergyFromSpecifiedSource(new(power_up_name)) {IncreaseAmount = power_budget, SpecifiedSource = Constants.EnergyGainSource.InflictedStaggered});
        }
        else if (power_up_name == "TechniqueDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique))});
        }
        else if(power_up_name == "AnalysisAmount") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, power_budget, new(power_up_name)));
        }
        else if(power_up_name == "AnalysisDecay") {
            PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Analysis), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Player, -power_budget, new(power_up_name)));
        }
        else if(power_up_name == "AnalysisBoostsAllDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget,         
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis)))),
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Effect_Analysis analysis = (Effect_Analysis)Player.Instance.GetEffect(typeof(Effect_Analysis));
                    damage.ExtraInjuryDealtPercentage += analysis.DecayingAmount * effect.InjuryPercentageChange / 100;
                    damage.ExtraStaggerDealtPercentage += analysis.DecayingAmount * effect.StaggerPercentageChange / 100;
            })});
        }
        else if (power_up_name == "MagicTechniqueDamage")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = power_budget, StaggerPercentageChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.DamageType == Constants.DamageType.Magic && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique))});
        }
        else if(power_up_name == "EnergyGainPerSecond") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { FlatAmount = power_budget, 
                ConditionCheckForOneFifthSecondElapsedNotRealtime = new Func<bool>(() => Player.Instance.Energy.Current < Player.Instance.Energy.Maximum), 
                ActionOnOneFifthSecondElapsedNotRealtime = new Action<Effect_CustomizableEffectOnEvent> ((effect) =>  {
                    Player.Instance.Energy.GenerateEnergy(effect.FlatAmount * Player.Instance.EnergyGain.Current / 5);
            })});
        }
        else if(power_up_name == "AnalysisDamageReduction") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = power_budget, 
                ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => (damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Analysis))))});
        }
        else if (power_up_name == "TechniqueDamageGivesAnalysis")
        {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.AddEffect(new Effect_Analysis(damage.InjuryDealt * effect.CustomParam / 100 + damage.StaggerDealt * effect.CustomParam / 100, new(power_up_name)));
            })});
        }
        else if(power_up_name == "PropriusTechniqueCooldownReduction") {
            PowerUpEffects.Add(new Effect_Description(new(power_up_name)) {Identifier="PropriusTechniqueCooldownReduction", PercentageAmount = power_budget});
        }
        else if(power_up_name == "AnalysisEnergyGain") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForEffectStarted = new Func<Effect, bool>((effect) => effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectStarted = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "AnalysisEnergyGain"));
                    if(msBuff == null) {
                        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.EnergyGain, new (power_up_name)) {PercentageAmount = effect.PercentageAmount, Identifier="AnalysisEnergyGain", IsRemovable=false, ShowsInUI=true, EffectIndicatorText=effect.PercentageAmount + "%"});
                    }}), 
                ConditionCheckForEffectEnded = new Func<Effect, bool>((effect) => effect is Effect_Analysis && effect.TargetOfEffect == Player.Instance), 
                ActionOnEffectEnded = new Action<Effect, Effect_CustomizableEffectOnEvent> ((effect_started, effect) =>  {
                    Effect msBuff = Player.Instance.GetEffect(new Func<Effect, bool>(effect => effect.Identifier == "AnalysisEnergyGain"));
                    if(msBuff != null) {
                        msBuff.EndThisEffect();
                    }
            })});
        }
        else if(power_up_name == "HealFromTechniqueDamage") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, 
                ConditionCheckOnDamageDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)), 
                Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                    Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.CustomParam + damage.StaggerDealt / 100 * effect.CustomParam;
            })});
        }
        else if(power_up_name == "HealFromEnergyUsed") {
            PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = power_budget, 
                ConditionCheckForAbilityEnergyConsumed = new Func<Ability, float, bool>((ability, amount) => ability.User == Player.Instance && amount > 0), 
                ActionOnAbilityEnergyConsumed = new Action<Ability, float, Effect_CustomizableEffectOnEvent> ((ability, amount, effect) =>  {
                    Player.Instance.Health.Current += Player.Instance.Health.Maximum * amount * power_budget / 100;
            })});
        }






/*

    else if(power_up_name == "DealExtraInjuryBasedOnMovementSpeed") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = percentage_value,
            ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) => damage.SourceOfDamage.User == Player.Instance && Player.Instance.MovementSpeed.Current > 1),
            Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
                damage.ExtraInjuryDealtPercentage += (Player.Instance.MovementSpeed.Current - 1) * effect.CustomParam;
            }
        )});
    }
    else if (power_up_name == "BackstabDamage")
    {
        PowerUpEffects.Add(new Effect_IncreaseDamageFromGivenAbilityType(percentage_value, new(power_up_name)) { AbilityType = typeof(Backstab)});
    }
    else if (power_up_name == "BasicAttackDamage")
    {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack)))});
    }
    else if (power_up_name == "AbilityDamage")
    {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)))});
    }
    else if (power_up_name == "StrongBasicAttackDamage")
    {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && damage.SourceOfDamage.Is(Ability.AbilityProperty.StrongBasicAttack) == true))});
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
                (damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) 
            && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
            })});
    }
    else if(power_up_name == "HealFromRipostesAndCounters") {
        PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                ((damage.SourceOfDamage.Is(Ability.AbilityProperty.Riposte) || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))
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
    else if(power_up_name == "HealFromIncision") {
        PowerUpEffects.Add(new Effect_HealWhenDamageDealt(new(power_up_name)) { HealAmount = percentage_value, ConditionCheck= new Func<Damage, bool>((damage) => 
                (damage.SourceOfDamage.User == Player.Instance && damage.Properites != null && damage.Properites.Contains(Damage.DamageProperty.Incision)
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
                (damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique)
            && damage.SourceOfDamage.User == Player.Instance)), ActionOnDamage = new Action<Damage, Effect_HealWhenDamageDealt> ((damage, effect) =>  {
                Player.Instance.Health.Current += damage.InjuryDealt / 100 * effect.HealAmount + damage.StaggerDealt / 100 * effect.HealAmount;
            })});
    }
    else if (power_up_name == "MagicStaggerToEmptyStaggerBar")
    {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.DamageType == Constants.DamageType.Magic && damage.TargetOfDamage.StaggerBar.Current < 1))});
    }

    else if(power_up_name == "ReducedDamageAfterRiposteOrCounter") {
        PowerUpEffects.Add(new Effect_CustomizableEffectOnEvent(new(power_up_name)) { PercentageAmount = percentage_value, ConditionCheckForAbilityUsed = new Func<Ability, bool>((ability) => 
                (ability.User is Player && (ability.Is(Ability.AbilityProperty.Riposte) || ability.Is(Ability.AbilityProperty.Counter)))), 
            ActionOnAbilityUsed = new Action<Ability, Effect_CustomizableEffectOnEvent> ((ability, customizableEffect) =>  {
                if(ability.Is(Ability.AbilityProperty.Riposte)) {
                    if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterRiposte") != null) {
                        Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterRiposte").EndThisEffect();
                    }
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(power_up_name)) {PercentageAmount=percentage_value, ExtraInfo = "ReducedDamageAfterRiposte"}, 10);
                }
                else {
                    if(Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterCounter") != null) {
                        Player.Instance.CurrentEffects.FirstOrDefault(e => e.ExtraInfo == "ReducedDamageAfterCounter").EndThisEffect();
                    }
                    Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, new(power_up_name)) {PercentageAmount=percentage_value * 2, ExtraInfo = "ReducedDamageAfterCounter"}, 10);
                }
        })});
    }
    else if(power_up_name == "ReducedDamageDuringBasicAttacks") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.TargetOfDamage == Player.Instance && Player.Instance.Actions.CurrentAbilityBeingPerformed != null && Player.Instance.Actions.CurrentAbilityBeingPerformed.Is(Ability.AbilityProperty.BasicAttack)))});
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
    else if(power_up_name == "ReducedDamageFromMissedCounters") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter)))});
    }
    else if(power_up_name == "ReducedDamageFromUncounterable") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.IsUncounterable))});
    }
    else if(power_up_name == "SharpAlsoEmpowersBackstabs") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.SourceOfDamage.GetType().IsSubclassOf(typeof(Backstab)) && damage.SourceOfDamage?.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Sharp)))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            float empowerAmount = ((Effect_Sharp)damage.TargetOfDamage.GetEffect(typeof(Effect_Sharp))).DecayingAmount;
            damage.ExtraInjuryDealtPercentage += empowerAmount;
            damage.ExtraStaggerDealtPercentage += empowerAmount;
        })});
    }
    else if(power_up_name == "SharpAlsoEmpowersBasicAttacks") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.SourceOfDamage.Is(Ability.AbilityProperty.BasicAttack) && damage.SourceOfDamage?.User == Player.Instance && damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_Sharp)))),
        Action = new Action<Damage, Effect_CustomizableDamageChange> ((damage, effect) =>  {
            float empowerAmount = ((Effect_Sharp)damage.TargetOfDamage.GetEffect(typeof(Effect_Sharp))).DecayingAmount;
            damage.ExtraInjuryDealtPercentage += empowerAmount * effect.CustomParam / 100;
            damage.ExtraStaggerDealtPercentage += empowerAmount * effect.CustomParam / 100;
        })});
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
    else if(power_up_name == "BackstabCooldown") {
        PowerUpEffects.Add(new Effect_LowerBackstabImmunity(new(power_up_name)) { ImmunityDurationReductionInSeconds = 5f });
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
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Heavy))});
    }
    else if(power_up_name == "LightAbilityInjury") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Light))});
    }
    else if(power_up_name == "RangedAbilityInjury") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Ranged))});
    }
    else if(power_up_name == "HeavyAbilityInjury") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Heavy))});
    }
    else if(power_up_name == "LightAbilityStagger") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Light))});
    }
    else if(power_up_name == "RangedAbilityStagger") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.AbilityDamageSource?.DamageType == Constants.DamageType.Ranged))});
    }
    else if(power_up_name == "UltimateAbilityDamage") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && damage.SourceOfDamage.GetType().GetField("IsUltimate", BindingFlags.Public | BindingFlags.Static).GetValue(null) != null))});
    }
    else if(power_up_name == "PropriusAbilityDamage") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { InjuryPercentageChange = percentage_value, StaggerPercentageChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                (damage.SourceOfDamage.User == Player.Instance && damage.SourceOfDamage.Is(Ability.AbilityProperty.Technique) && ((Ability.AbilityFamily)damage.SourceOfDamage.GetType().GetField("Family", BindingFlags.Public | BindingFlags.Static).GetValue(null)) == Ability.AbilityFamily.Proprius))});
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


    else if(power_up_name == "FreezeAmount") {
        PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, percentage_value, new(power_up_name)));
    }
    else if(power_up_name == "FreezeDecay") {
        PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Freeze), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -percentage_value, new(power_up_name)));
    }
    else if(power_up_name == "FreezeDamageReduction") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Freeze))))});
    }

    else if(power_up_name == "BarrierDamageReduction") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.TargetOfDamage == Player.Instance && Player.Instance.CheckIfUnderEffect(typeof(Effect_Barrier))))});
    }
    else if(power_up_name == "IncisionAmount") {
        PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, percentage_value, new(power_up_name)));
    }
    else if(power_up_name == "IncisionDecay") {
        PowerUpEffects.Add(new Effect_ChangeEffectPower(typeof(Effect_Incision), Effect_ChangeEffectPower.ChangeTypeEnum.AffectDecaySpeed, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, -percentage_value, new(power_up_name)));
    }
    else if(power_up_name == "IncisionDamageReduction") {
        PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { DamageReductionChange = percentage_value, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
            (damage.TargetOfDamage == Player.Instance && damage.SourceOfDamage.User.CheckIfUnderEffect(typeof(Effect_Incision))))});
    }

*/



















        else if(power_up_name == "IgnisManor_WeaponTraining") {
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyInjury, new(power_up_name)) {PercentageAmount = 10 });
            PowerUpEffects.Add(new Effect_ChangeStat(Player.Instance.HeavyStagger, new(power_up_name)) {PercentageAmount = 10 });
        }
        else if(power_up_name == "IgnisManor_BackstabPowerUp") {
            PowerUpEffects.Add(new Effect_CustomizableDamageChange(new(power_up_name)) { CustomParam = power_budget, ConditionCheckOnHitDealt = new Func<Damage, Effect_CustomizableDamageChange, bool>((damage, effect) =>
                damage.SourceOfDamage.Is(Ability.AbilityProperty.Backstab) && damage.SourceOfDamage?.User == Player.Instance && damage.IsDamageOverTime == false),
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
                (damage.TargetOfDamage == Player.Instance && damage.Properties.Contains(Damage.DamageProperty.Burn)))});
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
