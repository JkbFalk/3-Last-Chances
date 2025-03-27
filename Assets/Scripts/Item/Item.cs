using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;
using static Constants;
using static Item;
using static MenuManager;

public abstract class Item
{
    public string IconPath;
    public Sprite Icon;
    public bool CanOnlyBuyOnce = true;
    public int BuyPrice = 0;
    public int SellPrice = 0;
    public bool RemoveAtEndOfMission = false;
    [NonSerialized]
    public List<Effect> FirstModifier = new List<Effect>();
    [NonSerialized]
    public List<Effect> SecondModifier = new List<Effect>();
    public int[] UpgradePrice;
    [NonSerialized]
    public InventoryTile TileInInventory;
    [NonSerialized]
    public InventoryTile TileInEquipment;
    [NonSerialized]
    public GameObject ToolObject;
    public Dictionary<string, List<string>> ModifierDescriptions = new();
    public int MaxAmount {
        get {
            return Category == ItemCategory.Tool ? SaveFile.Instance.ToolMaxAmounts[GetType()] : GetType() == typeof(Quest_UpgradeMaterials) ? 99 : GetType() == typeof(Quest_ToolMaterials) ? 999 : 1;
        }
    }
    [NonSerialized]
    public Ability ItemUseAbility;
    [NonSerialized]
    public String CustomAnimation;
    public ItemSetEnum Set = ItemSetEnum.Unique;
    public ItemSetEnum ItemSet;
    public enum ItemSetEnum {WeaponMaster, Duelist, Jailer, Ancient, BattleBorn, Knight, Judge, Gunslinger, Arbiter, IronBlooded, Unbreakable, Mercenary, Survivor, Assassin, Executioner, Artisan, Alacrity, Enforcer, Sage, RoyalGuard, ShadowGifted, Unique};
    protected int _amount = 1;
    [NonSerialized]
    public List<Ability.DamageSource> DamageSources;
    public Sprite GetIcon() {
        GameController.Instance.GetComponent<SpriteResolver>().SetCategoryAndLabel(Category.ToString() + " Icons", GetType().ToString().Split('_')[1]);
        GameController.Instance.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
        return GameController.Instance.GetComponent<SpriteRenderer>().sprite;
    }

    public static Sprite GetIcon(Constants.ItemCategory category, Type type) {
        GameController.Instance.GetComponent<SpriteResolver>().SetCategoryAndLabel(category.ToString() + " Icons", type.ToString().Split('_')[1]);
        GameController.Instance.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
        return GameController.Instance.GetComponent<SpriteRenderer>().sprite;
    }
    public int Amount
    {
        get { return _amount; }
        set
        {
            if(Category != ItemCategory.Tool && Category != ItemCategory.Quest)
            {
                return;
            }
            _amount = value < 0 ? 0 : value > MaxAmount ? MaxAmount : value;
            if(TileInInventory != null)
            {
                TileInInventory.AmountDisplay.text = Category == ItemCategory.Tool ? Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()] : Amount.ToString();
            }
            if (TileInEquipment != null)
            {
                TileInEquipment.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
            }
            if (SaveFile.Instance.EquippedItem1 == this)
            {
                CanvasElements.UICanvas.Items.transform.Find("1/Uses/Text").GetComponent<TextMeshProUGUI>().text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                MenuManager.Instance.Item1EquipmentSlot.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                CanvasElements.UICanvas.Items.transform.Find("1/Disabled").gameObject.SetActive(_amount <= 0);
            }
            if (SaveFile.Instance.EquippedItem2 == this)
            {
                CanvasElements.UICanvas.Items.transform.Find("2/Uses/Text").GetComponent<TextMeshProUGUI>().text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                MenuManager.Instance.Item2EquipmentSlot.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                CanvasElements.UICanvas.Items.transform.Find("2/Disabled").gameObject.SetActive(_amount <= 0);
            }
            if(_amount == 0 && Category != ItemCategory.Tool) {
                SaveFile.Instance.RemoveItem(this);
            }
        }
    }
    public enum ItemGrade { Regular, Excellent, Masterful, Flawless, Ultimate, None };
    private ItemGrade _grade;
    public ItemGrade Grade {
        get { return Category == ItemCategory.Tool ? SaveFile.Instance.ToolGrades[GetType()] : _grade; }
        set { _grade = value; }
    }
    public int GradeIndex
    {
        get
        {
            switch (Grade) { 
                case ItemGrade.Regular: return 0;
                case ItemGrade.Excellent: return 1;
                case ItemGrade.Masterful: return 2;
                case ItemGrade.Flawless: return 3;
                case ItemGrade.Ultimate: return 4;
                case ItemGrade.None: return 0;
                default: return 0;
            }
        }
    }

    public ItemGrade GetGradeForIndex(int index) {
            switch (index) { 
                case 0: return ItemGrade.Regular;
                case 1: return ItemGrade.Excellent;
                case 2: return ItemGrade.Masterful;
                case 3: return ItemGrade.Flawless;
                case 4: return ItemGrade.Ultimate;
                case 5: return ItemGrade.None;
                default: return ItemGrade.Regular;
            }
    }

    public static int GetIndexForItemGrade(ItemGrade grade) {
        switch (grade) { 
            case ItemGrade.Regular: return 0;
            case ItemGrade.Excellent: return 1;
            case ItemGrade.Masterful: return 2;
            case ItemGrade.Flawless: return 3;
            case ItemGrade.Ultimate: return 4;
            case ItemGrade.None: return 0;
            default: return 0;
        }
    }

    public Item(ItemGrade grade)
    {
        Grade = grade;
    }

    public float GetMultiplierForGrade()
    {
        return GetMultiplierForGrade(Grade);
    }

    public static float GetMultiplierForGrade(Item.ItemGrade grade)
    {
        switch(grade)
        {
            case ItemGrade.Regular: return 20;
            case ItemGrade.Excellent: return 40;
            case ItemGrade.Masterful: return 80;
            case ItemGrade.Flawless: return 130;
            case ItemGrade.Ultimate: return 200;
            default: return 1;
        }
    }

    public bool IsEquipped = false;
    public bool IsEquippedInSlot1 = false;
    public bool IsEquippedInSlot2 = false;

    public Constants.WeaponClass WeaponClass = Constants.WeaponClass.None;
    public Constants.DamageType WeaponCategory {
        get {
            if(WeaponClass == WeaponClass.Greatsword || WeaponClass == WeaponClass.Polearm || WeaponClass == WeaponClass.Longblade) {
                return Constants.DamageType.Heavy;
            }
            else if(WeaponClass == WeaponClass.TwinBlades || WeaponClass == WeaponClass.Daggers || WeaponClass == WeaponClass.Gauntlets) {
                return Constants.DamageType.Light;
            }
            else if(WeaponClass == WeaponClass.Gun || WeaponClass == WeaponClass.Bow || WeaponClass == WeaponClass.Cannon) {
                return Constants.DamageType.Ranged;
            }
            return DamageType.None;
        }
    }

    private Constants.ItemCategory _category;
    public Constants.ItemCategory Category {
        get => _category;
        set {
            _category = value;
            UpgradePrice = (_category == ItemCategory.Heavy || _category == ItemCategory.Light || _category == ItemCategory.Ranged || _category == ItemCategory.Armor) ? new int[]{ 12000, 40000, 120000, 500000 } : new int[]{ 6000, 20000, 60000, 250000 };
            if(_category == ItemCategory.Tool)
            {
                switch (Grade)
                {
                    case ItemGrade.Regular: SellPrice = 100; break;
                    case ItemGrade.Excellent: SellPrice = 300; break;
                    case ItemGrade.Masterful: SellPrice = 1000; break;
                    case ItemGrade.Flawless: SellPrice = 3000; break;
                    case ItemGrade.Ultimate: SellPrice = 10000; break;
                    default: SellPrice = 0; break;
                }
            } 
            else if(_category == ItemCategory.Heavy || _category == ItemCategory.Light || _category == ItemCategory.Ranged || _category == ItemCategory.Armor) {
                switch(Grade)
                {
                    case ItemGrade.Regular: SellPrice = 2000; break;
                    case ItemGrade.Excellent: SellPrice = 6000; break;
                    case ItemGrade.Masterful: SellPrice = 20000; break;
                    case ItemGrade.Flawless: SellPrice = 60000; break;
                    case ItemGrade.Ultimate: SellPrice = 200000; break;
                    default: SellPrice = 0; break;
                }
            }
            else {
                switch(Grade)
                {
                    case ItemGrade.Regular: SellPrice = 1000; break;
                    case ItemGrade.Excellent: SellPrice = 3000; break;
                    case ItemGrade.Masterful: SellPrice = 10000; break;
                    case ItemGrade.Flawless: SellPrice = 30000; break;
                    case ItemGrade.Ultimate: SellPrice = 100000; break;
                    default: SellPrice = 0; break;
                }
            }
            if(_category == ItemCategory.Tool) {
                CanOnlyBuyOnce = false;
            }
        }
    }

    public float Duration;
    public Type OnUseAbility;
    public float BaseInjury = 0;
    public float BaseStagger = 0;
    public float BaseAttackSpeed;

    public float GetFirstModifierEffectValue(bool linear_scaling = true) {
        switch(Grade) {
            case ItemGrade.Regular: return linear_scaling ? (IsDoubleValue() ? 32 : 16) : (IsDoubleValue() ? 80 : 40);
            case ItemGrade.Excellent: return linear_scaling ? (IsDoubleValue() ? 64 : 32) : (IsDoubleValue() ? 100 : 50);
            case ItemGrade.Masterful: return linear_scaling ? (IsDoubleValue() ? 80 : 40) : (IsDoubleValue() ? 120 : 60);
            case ItemGrade.Flawless: return linear_scaling ? (IsDoubleValue() ? 120 : 60) : (IsDoubleValue() ? 140 : 70);
            case ItemGrade.Ultimate: return linear_scaling ? (IsDoubleValue() ? 160 : 80) : (IsDoubleValue() ? 160 : 80);
            default: return 0;
        }
    }

    public virtual List<Effect> GetFirstModifier() {
        return new();
    }   

    public virtual List<Effect> GetSecondModifier() {
        return new();
    }

    public float GetSecondModifierEffectValue(bool linear_scaling = true) {
        switch(Grade) {
            case ItemGrade.Regular: return 0;
            case ItemGrade.Excellent: return 0;
            case ItemGrade.Masterful: return linear_scaling ? (IsDoubleValue() ? 20f : 10f) : (IsDoubleValue() ? 40f : 20f);
            case ItemGrade.Flawless: return linear_scaling ? (IsDoubleValue() ? 40 : 20f) : (IsDoubleValue() ? 60f : 30f);
            case ItemGrade.Ultimate: return linear_scaling ? (IsDoubleValue() ? 80 : 40) : (IsDoubleValue() ? 80f : 40f);
            default: return 0;
        }
    }

    public void ClearFromInventory() {
        if(TileInEquipment?.Item != null) {
            TileInEquipment.Item = null;
        }
        if(TileInInventory?.Item != null) {
            TileInInventory.Item = null;
        }
        TileInEquipment = null;
        TileInInventory = null;
    }

    public bool IsDoubleValue() {
        return Category == ItemCategory.Heavy || Category == ItemCategory.Light || Category == ItemCategory.Ranged || Category == ItemCategory.Armor;
    }

    public string GetUpgradeMaterialIcon() {
        return Grade == ItemGrade.Excellent ? "[MasterfulMaterials]" : Grade == ItemGrade.Masterful ? "[FlawlessMaterials]" : Grade == ItemGrade.Flawless ? "[UltimateMaterials]" : Grade == ItemGrade.Regular ? "[ExcellentMaterials]" : "";
    }

    public string GetUpgradeMaterialAmount() {
        return IsDoubleValue() ? "x2" : "";
    }

    public bool CheckIfEnoughUpgradeMaterials() {
        int amount_required = IsDoubleValue() ? 2 : 1;
        if(Grade == ItemGrade.Regular && SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_UpgradeMaterials && item.Amount >= amount_required && item.Grade == ItemGrade.Excellent) != null) {
            return true;
        }
        else if(Grade == ItemGrade.Excellent && SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_UpgradeMaterials && item.Amount >= amount_required && item.Grade == ItemGrade.Masterful) != null) {
            return true;
        }
        else if(Grade == ItemGrade.Masterful && SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_UpgradeMaterials && item.Amount >= amount_required && item.Grade == ItemGrade.Flawless) != null) {
            return true;
        }
        else if(Grade == ItemGrade.Flawless && SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_UpgradeMaterials && item.Amount >= amount_required && item.Grade == ItemGrade.Ultimate) != null) {
            return true;
        }
        return false;
    }

    public bool CheckIfEnoughToolMaterialsForGradeUpgrade() {
        int cost =
        Grade == Item.ItemGrade.Regular ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_EXCELLENT :
        Grade == Item.ItemGrade.Excellent ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_MASTERFUL :
        Grade == Item.ItemGrade.Masterful ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_FLAWLESS :
        Grade == Item.ItemGrade.Flawless? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_ULTIMATE :
        0;
        return SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_ToolMaterials && item.Amount >= cost) != null;
    }

    public bool CheckIfEnoughToolMaterialsForAmountUpgrade() {
        int cost =
        MaxAmount == 1 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_2 :
        MaxAmount == 2 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_3 :
        MaxAmount == 3 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_4 :
        MaxAmount == 4 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_5 :
        MaxAmount == 5 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_6 :
        0;
        return SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_ToolMaterials && item.Amount >= cost) != null;
    }
    
    public string GetUpgradePrice() {
        if(CheckIfCanUpgrade() == false) {
            return "---";
        }
        return  Utils.InsertLabelsIntoText("[Money]" + Utils.GetFormattedInteger(UpgradePrice[GradeIndex]) + ", " + GetUpgradeMaterialIcon() + GetUpgradeMaterialAmount());
    }

    public string GetToolGradeUpgradePrice() {
        int cost =
        Grade == Item.ItemGrade.Regular ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_EXCELLENT :
        Grade == Item.ItemGrade.Excellent ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_MASTERFUL :
        Grade == Item.ItemGrade.Masterful ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_FLAWLESS :
        Grade == Item.ItemGrade.Flawless? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_ULTIMATE :
        0;
        return Utils.InsertLabelsIntoText("[ToolMaterials]" + cost);
    }

    public string GetToolAmountUpgradePrice() {
        int cost =
        MaxAmount == 1 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_2 :
        MaxAmount == 2 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_3 :
        MaxAmount == 3 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_4 :
        MaxAmount == 4 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_5 :
        MaxAmount == 5 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_6 :
        0;
        return Utils.InsertLabelsIntoText("[ToolMaterials]" + cost);
    }

    public bool CheckIfCanUpgrade()
    {
        return (Category == ItemCategory.Heavy || Category == ItemCategory.Light || Category == ItemCategory.Ranged || Category == ItemCategory.Gloves || Category == ItemCategory.Helmet || Category == ItemCategory.Armor || Category == ItemCategory.Boots || Category == ItemCategory.Tool) && Grade != ItemGrade.Ultimate;
    }

    public static float GetCooldown(Type tool_type, ItemGrade grade = ItemGrade.None) {
        float cd;
        FieldInfo field = tool_type.GetField("CooldownPerGrade", BindingFlags.Public | BindingFlags.Static);
        if(field != null) {
            cd = ((float[])field.GetValue(null))[GetIndexForItemGrade(grade)];
        }
        else {
            FieldInfo field2 = tool_type.GetField("Cooldown", BindingFlags.Public | BindingFlags.Static);
            if(field2 == null) {
                return 0;
            }
            cd = (float)field2.GetValue(null);
        }
        return cd;
    }

    public virtual string GetFlavorText()
    {
        return Label.Get(GetType() + "_FlavorText");
    }

    public virtual string GetItemName()
    {
        return Label.Get(GetType().ToString()) + " (" + Label.Get("ItemGrade_" + Grade.ToString() + "_Colored") + " " + Label.Get("ItemCategory_" + Category.ToString()) + ")";
    }

    public virtual string GetDescription(bool detailed = false)
    {
        string description = "";
        if(Category == Constants.ItemCategory.Heavy || Category == Constants.ItemCategory.Light || Category == Constants.ItemCategory.Ranged)
        {
            description += "<sprite name=\"" + Category.ToString() + "Injury\">" + Utils.GetFormattedFloat(BaseInjury) + " <sprite name=\"" + Category.ToString() + "Stagger\">" + Utils.GetFormattedFloat(BaseStagger) +" <sprite name=\"" + WeaponCategory.ToString() + "AttackSpeed\">" + Utils.GetFormattedFloat(BaseAttackSpeed, 2, true) + "\n\n";
        }
        /*else if(Category == Constants.ItemCategory.Gloves)
        {
            description += "<sprite name=\"MagicInjury\">" + Utils.GetFormattedFloat(BaseInjury) + " <sprite name=\"MagicStagger\">" + Utils.GetFormattedFloat(BaseStagger) + " <sprite name=\"AttackSpeed\">" + Utils.GetFormattedFloat(BaseAttackSpeed, 2, true) + "\n\n";
        }*/
        return description;
    }

    public virtual void ExtraBehaviourOnEquip() {}
    public virtual void ExtraBehaviourOnUnequip() {}

    public void Equip(int item_slot = 0)
    {
        switch (Category) 
        {
            case ItemCategory.Heavy: {SaveFile.Instance.EquippedHeavyWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHeavyWeapon, this); break;}
            case ItemCategory.Light: {SaveFile.Instance.EquippedLightWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedLightWeapon, this); break;}
            case ItemCategory.Ranged: {SaveFile.Instance.EquippedRangedWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedRangedWeapon, this); break;}
            case ItemCategory.Gloves: {SaveFile.Instance.EquippedGloves = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedGloves, this); break;}
            case ItemCategory.Helmet: {SaveFile.Instance.EquippedHelmet = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHelmet, this); break;}
            case ItemCategory.Armor: {SaveFile.Instance.EquippedArmor = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedArmor, this); break;}
            case ItemCategory.Boots: {SaveFile.Instance.EquippedBoots = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedBoots, this); break;}
        }
        if (Category == ItemCategory.Tool)
        {
            if (item_slot == 1)
            {
                SaveFile.Instance.EquippedItem1 = this;
                IsEquippedInSlot1 = true;
                EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedItem1, this);
            }
            if (item_slot == 2)
            {
                SaveFile.Instance.EquippedItem2 = this;
                IsEquippedInSlot1 = true;
                EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedItem2, this);
            }
        }
        else {
            IsEquipped = true;
        }
        if(Player.Instance == null) {
            return;
        }
        bool IsWeapon = Category == ItemCategory.Heavy || Category == ItemCategory.Light || Category == ItemCategory.Ranged;
        FirstModifier = GetFirstModifier();
        foreach (Effect mod in FirstModifier)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponCategory == Category) {
                mod.NonLinearEffectValue = GetFirstModifierEffectValue(false);
                mod.LinearEffectValue = GetFirstModifierEffectValue();
                mod.OnEffectValueChanged();
                mod.IsRemovable = false;
                mod.ShowsInMenu=false;
                Player.Instance.AddEffect(mod);
            }
        }
        SecondModifier = GetSecondModifier();
        foreach (Effect mod in SecondModifier)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponCategory == Category) {
                mod.NonLinearEffectValue = GetSecondModifierEffectValue(false);
                mod.LinearEffectValue = GetSecondModifierEffectValue();
                mod.OnEffectValueChanged();
                mod.IsRemovable = false;
                mod.ShowsInMenu=false;
                Player.Instance.AddEffect(mod);
            }
        }
        if(Category == ItemCategory.Heavy)
        {
            Player.Instance.HeavyInjury.Base = BaseInjury;
            Player.Instance.HeavyStagger.Base = BaseStagger;
            Player.Instance.HeavyAttackSpeed.Base = BaseAttackSpeed;
        }
        else if (Category == ItemCategory.Light)
        {
            Player.Instance.LightInjury.Base = BaseInjury;
            Player.Instance.LightStagger.Base = BaseStagger;
            Player.Instance.LightAttackSpeed.Base = BaseAttackSpeed;
        }
        else if (Category == ItemCategory.Ranged)
        {
            Player.Instance.RangedInjury.Base = BaseInjury;
            Player.Instance.RangedStagger.Base = BaseStagger;
            Player.Instance.RangedAttackSpeed.Base = BaseAttackSpeed;
        }
        if (Category != ItemCategory.Tool)
        {
            Utils.CopyItemAppearanceForPlayer(Category, GetType().ToString() + "_" + Grade.ToString());
        }
        ExtraBehaviourOnEquip();
    }

    public void Unequip(int item_slot = 0)
    {
        if(Player.Instance == null) {
            return;
        }
        IsEquipped = false;
        switch (Category) 
        {
            case ItemCategory.Heavy: {SaveFile.Instance.EquippedHeavyWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHeavyWeapon, null); break;}
            case ItemCategory.Light: {SaveFile.Instance.EquippedLightWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedLightWeapon, null); break;}
            case ItemCategory.Ranged: {SaveFile.Instance.EquippedRangedWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedRangedWeapon, null); break;}
            case ItemCategory.Gloves: {SaveFile.Instance.EquippedGloves = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedGloves, null); break;}
            case ItemCategory.Helmet: {SaveFile.Instance.EquippedHelmet = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHelmet, null); break;}
            case ItemCategory.Armor: {SaveFile.Instance.EquippedArmor = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedArmor, null); break;}
            case ItemCategory.Boots: {SaveFile.Instance.EquippedBoots = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedBoots, null); break;}
        }
        if(Category == ItemCategory.Tool)
        {
            if(SaveFile.Instance.EquippedItem1 == this && item_slot == 1)
            {
                SaveFile.Instance.EquippedItem1 = null;
                EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedItem1, null);
            }
            if (SaveFile.Instance.EquippedItem2 == this && item_slot == 2)
            {
                SaveFile.Instance.EquippedItem2 = null;
                EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedItem2, null);
            }
        }
        bool IsWeapon = Category == ItemCategory.Heavy || Category == ItemCategory.Light || Category == ItemCategory.Ranged;
        foreach (Effect mod in FirstModifier)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponCategory == Category) {
                Player.Instance.EndEffect(mod);
            }
        }
        foreach (Effect mod in SecondModifier)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponCategory == Category) {
                Player.Instance.EndEffect(mod);
            }
        }
        if(Category != ItemCategory.Heavy && Category != ItemCategory.Light && Category != ItemCategory.Ranged && Category != ItemCategory.Tool)
        {
            Utils.CopyItemAppearanceForPlayer(Category, "Default");
            if(Category == ItemCategory.Helmet && Area.ComponentInstance != null && Area.ComponentInstance.ApplyPlayerCamouflage) {
                Player.Instance.UnitColorChange.Hair = Colors.GetColorFromCode("#DBA600");
                Player.Instance.UnitColorChange.Eye = Colors.GetColorFromCode("#359C34");
                Player.Instance.UnitColorChange.UpdateMaterialProperties();
            }
        }
        ExtraBehaviourOnUnequip();
    }

    public virtual void OnEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {

    }

    public virtual void OnUse()
    {

    }

    public virtual void OnEndUse()
    {

    }

    public void SetBaseWeaponStats(float base_health_damage, float base_stagger_damage, float base_attack_speed)
    {
        BaseInjury = base_health_damage;
        BaseStagger = base_stagger_damage;
        BaseAttackSpeed = base_attack_speed;
    }

    public virtual bool CheckIfCanEquipItem()
    {
        return IsEquipped == false && Category != ItemCategory.Quest;
    }

    public bool CheckIfItemHasSeparateItemIcon()
    {
        return Category != Constants.ItemCategory.Heavy && Category != Constants.ItemCategory.Light && Category != Constants.ItemCategory.Ranged;
    }

    public void RemoveItemFromInventory()
    {
        MonoBehaviour.Destroy(TileInInventory.gameObject);
        SaveFile.Instance.Inventory.Remove(this);
    }

    public static ItemGrade GetRandomizedGradeForGivenLevel(int level)
    {
        int weightedChanceTotal = 0;
        int[] chances = new int[5] { 20 - Math.Abs(level - 1), 20 - Math.Abs(level - 20), 20 - Math.Abs(level - 30), 20 - Math.Abs(level - 45), 20 - Math.Abs(level - 60) };
        foreach(int chance in chances)
        {
            weightedChanceTotal += chance > 0 ? chance : 0;
        }
        int random = UnityEngine.Random.Range(1, weightedChanceTotal);
        int total_so_far = 0;
        for(int i = 0; i < chances.Length; i++)
        {
            if (chances[i] > 0)
            {
                total_so_far += chances[i];
                if(random <= total_so_far)
                {
                    return i == 0 ? ItemGrade.Regular : i == 1 ? ItemGrade.Excellent : i == 2 ? ItemGrade.Masterful : i == 3 ? ItemGrade.Flawless : ItemGrade.Ultimate;
                }
            }
        }
        return ItemGrade.Regular;
    }
}
