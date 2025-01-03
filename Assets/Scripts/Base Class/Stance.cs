using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stance {
    public Constants.ItemCategory WeaponCategory;

    public Constants.DamageType DamageCategory {
        get {
            switch(WeaponCategory) {
                case Constants.ItemCategory.Heavy: return Constants.DamageType.Heavy;
                case Constants.ItemCategory.Light: return Constants.DamageType.Light;
                case Constants.ItemCategory.Ranged: return Constants.DamageType.Ranged;
                default: return Constants.DamageType.None;
            }
        }
    }
    public Constants.WeaponClass WeaponClass {
        get {
            if(StanceEffectType == typeof(Stance_MindOverMatter) || StanceEffectType == typeof(Stance_PowerWithoutLimit)) {
                return Constants.WeaponClass.Magic;
            }
            if(SaveFile.Instance.EquippedHeavyWeapon == null || SaveFile.Instance.EquippedLightWeapon == null || SaveFile.Instance.EquippedRangedWeapon == null) {
                return Constants.WeaponClass.None;
            }
            switch(DamageCategory) {
                case Constants.DamageType.Heavy: return SaveFile.Instance.EquippedHeavyWeapon.WeaponClass;
                case Constants.DamageType.Light: return SaveFile.Instance.EquippedLightWeapon.WeaponClass;
                case Constants.DamageType.Ranged: return SaveFile.Instance.EquippedRangedWeapon.WeaponClass;
                default: return Constants.WeaponClass.None;
            }
        }
    }
    [NonSerialized]
    public Transform UIStanceDisplay;
    [NonSerialized]
    public Image StanceCooldownDisplay;
    public List<EquippedAbility> Abilities;
    public Type StanceEffectType;

    private Effect_Stance _stanceEffect;
    public Effect_Stance StanceEffect {
        get => _stanceEffect;
        set {
            if(_stanceEffect != null && !(_stanceEffect is Stance_None)) {
                Player.Instance.EndEffect(_stanceEffect);
            }
            _stanceEffect = value;
            StanceEffectType = value.GetType();
            _stanceEffect.AssignedStance = this;
            if(Player.Instance != null && _stanceEffect != null && !(_stanceEffect is Stance_None)) {
                Player.Instance.AddEffect(_stanceEffect);
            }
        }
    }
    public Item Weapon
    {
        get {
            if (WeaponCategory == Constants.ItemCategory.Heavy )
            {
                return SaveFile.Instance.EquippedHeavyWeapon;
            }
            else if (WeaponCategory == Constants.ItemCategory.Light)
            {
                return SaveFile.Instance.EquippedLightWeapon;
            }
            else if (WeaponCategory == Constants.ItemCategory.Ranged)
            {
                return SaveFile.Instance.EquippedRangedWeapon;
            }
            return null;
        }
    }

    public class EquippedAbility {
        public int Index;
        [SerializeField]
        private Type _type;
        public Type Type {
            get => _type;
            set {
                _type = value;
                MenuManager.Instance.transform.Find("Overview Window/Abilities/Stances/" + Category.ToString() + "/" + Index).GetComponent<AbilitySelect>().AbilityType = _type;
                MenuManager.Instance.transform.Find("Overview Window/Abilities/Stances/" + Category.ToString() + "/" + Index + "/Mask/Icon").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(_type != null ? _type.ToString() : null);
                MenuManager.Instance.transform.Find("Overview Window/Abilities/Stances/" + Category.ToString() + "/" + Index + "/Mask/Icon").GetComponent<Image>().color = _type != null ? Color.white : Color.black;
                if(Player.Instance?.CurrentStance?.DamageCategory != null && Player.Instance.CurrentStance.DamageCategory == Category) {
                    RefreshDisplayForEquippedAbility();
                }
            }
        }
        [NonSerialized]
        public Image AbilityGraphic;
        [NonSerialized]
        public Image Icon;
        [NonSerialized]
        public Image CooldownDisplay;
        [NonSerialized]
        public TextMeshProUGUI StacksCounter;
        [NonSerialized]
        public TextMeshProUGUI CooldownCounter;
        [NonSerialized]
        public TextMeshProUGUI CostLabel;

        public Constants.DamageType Category;

        public EquippedAbility(Type ability_type, int number, Constants.DamageType category) {
            Category = category;
            Index = number;
            Type = ability_type;
            AbilityGraphic = GameController.Instance.transform.Find("UI Canvas/Stance Display " + Settings.Instance.ControlScheme + "/Abilities/" + Index).GetComponent<Image>();
            Icon = AbilityGraphic.transform.Find("Image").GetComponent<Image>();
            CooldownDisplay = AbilityGraphic.transform.Find("Cooldown").GetComponent<Image>();
            CooldownCounter = AbilityGraphic.transform.Find("Cooldown Counter").GetComponent<TextMeshProUGUI>();
            StacksCounter = AbilityGraphic.transform.Find("Stacks Counter").GetComponent<TextMeshProUGUI>();
            CostLabel = AbilityGraphic.transform.Find("Cost/Text").GetComponent<TextMeshProUGUI>();
        }
        
        public void Reload()
        {
            AbilityGraphic = GameController.Instance.transform.Find("UI Canvas/Stance Display " + Settings.Instance.ControlScheme + "/Abilities/" + Index).GetComponent<Image>();
            Icon = AbilityGraphic.transform.Find("Image").GetComponent<Image>();
            CooldownDisplay = AbilityGraphic.transform.Find("Cooldown").GetComponent<Image>();
            CooldownCounter = AbilityGraphic.transform.Find("Cooldown Counter").GetComponent<TextMeshProUGUI>();
            StacksCounter = AbilityGraphic.transform.Find("Stacks Counter").GetComponent<TextMeshProUGUI>();
            CostLabel = AbilityGraphic.transform.Find("Cost/Text").GetComponent<TextMeshProUGUI>();
            Type = Type;
        }

        public void RefreshDisplayForEquippedAbility() {
            if(Icon == null) {
                Reload();
            }
            Icon.color = Type == null ? Color.black : Color.white;
            Icon.sprite = Utils.GetGraphicForAbility(Type != null ? Type.ToString() : null);
            if (Type?.GetMethod("GetAbilitySpecificEnergyCostText") != null) {
                CostLabel.text = Type == null ? "" : (string)Type.GetMethod("GetAbilitySpecificEnergyCostText").Invoke(null, null);
            }
            else {
                CostLabel.text = Type == null ? "" : Utils.GetFormattedFloat(Ability.GetEnergyCost(Type));
            }
            AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(!SaveFile.Instance.UnlockedAbilities.Contains(Type));
            MethodInfo check = Type == null ? null : Type.GetMethod("CheckIfAbilityUsableDependingOnCombat", BindingFlags.Public | BindingFlags.Static);
            if(SaveFile.Instance.UnlockedAbilities.Contains(Type) && check != null)
            {
                bool can_use = (bool)check.Invoke(null, new object[] { Player.Instance.InCombat });
                AbilityGraphic.transform.Find("Disabled").gameObject.SetActive(can_use == false);
            }
            Energy.MarkAbilitiesWithNotEnoughEnergy();
            StacksCounter.text = Type?.GetField("IsStacksBasedTechnique") == null ? "" : (Player.Instance.PreparingForUltimate ? Player.Instance.CurrentUltimateTechniqueStacks[Type].ToString() : Player.Instance.CurrentTechniqueStacks[Type].ToString());
        }
    }

    public EquippedAbility GetEquippedAbility(Type ability_type) {
        return Abilities.FirstOrDefault(ability => ability.Type == ability_type);
    }

    public Image GetDisplayOfGivenAbility(Type ability_type) {
        return Abilities.FirstOrDefault(ability => ability.Type == ability_type)?.AbilityGraphic;
    }
}