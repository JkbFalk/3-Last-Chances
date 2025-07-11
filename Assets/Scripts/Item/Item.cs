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
    public List<ItemEffect> FirstItemEffects;
    public List<ItemEffect> SecondItemEffects;
    public string IconPath;
    public Sprite Icon;
    public bool CanOnlyBuyOnce = true;
    public int BuyPrice = 0;
    public int SellPrice = 0;
    public List<Effect> ActiveEffects = new List<Effect>();
    public bool RemoveAtEndOfMission = false;
    public int[] UpgradePrice;
    [NonSerialized]
    public InventoryTile TileInInventory;
    [NonSerialized]
    public InventoryTile TileInEquipment;
    [NonSerialized]
    public GameObject ToolObject;
    public int MaxAmount {
        get {
            return Type == ItemType.Tool ? SaveFile.Instance.ToolMaxAmounts[GetType()] : GetType() == typeof(Quest_UpgradeMaterials) ? 99 : GetType() == typeof(Quest_ToolMaterials) ? 999 : 1;
        }
    }
    [NonSerialized]
    public Ability ItemUseAbility;
    [NonSerialized]
    public String CustomAnimation;
    public ItemSetEnum Set = ItemSetEnum.Unique;
    public enum ItemSetEnum {Duelist, WeaponMaster, Jailer, Ancient, BattleBorn, Knight, Judge, Gunslinger, Arbiter, IronBlooded, Unbreakable, Mercenary, Survivor, Assassin, Executioner, Artisan, Alacrity, Enforcer, Sage, RoyalGuard, ShadowGifted, Unique};
    protected int _amount = 1;
    [NonSerialized]
    public List<Ability.DamageSource> DamageSources;
    public Sprite GetIcon() {
        GameController.Instance.GetComponent<SpriteResolver>().SetCategoryAndLabel(Type.ToString() + " Icons", GetType().ToString().Split('_')[1]);
        GameController.Instance.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
        return GameController.Instance.GetComponent<SpriteRenderer>().sprite;
    }

    public int Amount
    {
        get { return _amount; }
        set
        {
            if(Type != ItemType.Tool && Type != ItemType.Quest)
            {
                return;
            }
            _amount = value < 0 ? 0 : value > MaxAmount ? MaxAmount : value;
            if(TileInInventory != null)
            {
                TileInInventory.AmountDisplay.text = Type == ItemType.Tool ? Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()] : Amount.ToString();
            }
            if (TileInEquipment != null)
            {
                TileInEquipment.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
            }
            if (SaveFile.Instance.EquippedItem1 == this)
            {
                UIManager.Objects.Items.transform.Find("1/Uses/Text").GetComponent<TextMeshProUGUI>().text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                MenuManager.Instance.Item1EquipmentSlot.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                UIManager.Objects.Items.transform.Find("1/Disabled").gameObject.SetActive(_amount <= 0);
            }
            if (SaveFile.Instance.EquippedItem2 == this)
            {
                UIManager.Objects.Items.transform.Find("2/Uses/Text").GetComponent<TextMeshProUGUI>().text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                MenuManager.Instance.Item2EquipmentSlot.AmountDisplay.text = Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[GetType()];
                UIManager.Objects.Items.transform.Find("2/Disabled").gameObject.SetActive(_amount <= 0);
            }
            if(_amount == 0 && Type != ItemType.Tool) {
                SaveFile.Instance.RemoveItem(this);
            }
        }
    }
    public enum ItemGrade { Regular, Excellent, Masterful, Flawless, Ultimate, None };
    private ItemGrade _grade;
    public ItemGrade Grade {
        get { return Type == ItemType.Tool ? SaveFile.Instance.ToolGrades[GetType()] : _grade; }
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

    private Constants.ItemType _type;
    public Constants.ItemType Type {
        get => _type;
        set {
            _type = value;
            UpgradePrice = (_type == ItemType.Heavy || _type == ItemType.Light || _type == ItemType.Ranged || _type == ItemType.Outfit) ? new int[]{ 12000, 40000, 120000, 500000 } : new int[]{ 6000, 20000, 60000, 250000 };
            if(_type == ItemType.Tool)
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
            else if(_type == ItemType.Heavy || _type == ItemType.Light || _type == ItemType.Ranged || _type == ItemType.Outfit) {
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
            if(_type == ItemType.Tool) {
                CanOnlyBuyOnce = false;
            }
        }
    }

    public Type OnUseAbility;
    public float BaseInjury = 0;
    public float BaseStagger = 0;
    public float BaseAttackSpeed;

    public float GetItemFirstEffectPB() {
        switch(Grade) {
            case ItemGrade.Regular: return IsMajorItem() ? PB.MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER1_PB : PB.MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER1_PB;
            case ItemGrade.Excellent: return IsMajorItem() ? PB.MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER2_PB : PB.MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER2_PB;
            case ItemGrade.Masterful: return IsMajorItem() ? PB.MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER3_PB : PB.MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER3_PB;
            case ItemGrade.Flawless: return IsMajorItem() ? PB.MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER4_PB : PB.MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER4_PB;
            case ItemGrade.Ultimate: return IsMajorItem() ? PB.MAJOR_ITEM_FIRST_EFFECT_LINEAR_TIER5_PB : PB.MINOR_ITEM_FIRST_EFFECT_LINEAR_TIER5_PB;
            default: return 0;
        }
    }

    public float GetItemSecondEffectPB() {
        switch(Grade) {
            case ItemGrade.Regular: return IsMajorItem() ? PB.MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER1_PB : PB.MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER1_PB;
            case ItemGrade.Excellent: return IsMajorItem() ? PB.MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER2_PB : PB.MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER2_PB;
            case ItemGrade.Masterful: return IsMajorItem() ? PB.MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER3_PB : PB.MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER3_PB;
            case ItemGrade.Flawless: return IsMajorItem() ? PB.MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER4_PB : PB.MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER4_PB;
            case ItemGrade.Ultimate: return IsMajorItem() ? PB.MAJOR_ITEM_SECOND_EFFECT_LINEAR_TIER5_PB : PB.MINOR_ITEM_SECOND_EFFECT_LINEAR_TIER5_PB;
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

    public bool IsMajorItem() {
        return Type == ItemType.Heavy || Type == ItemType.Light || Type == ItemType.Ranged || Type == ItemType.Outfit;
    }

    public string GetUpgradeMaterialIcon() {
        return Grade == ItemGrade.Excellent ? "[MasterfulMaterials]" : Grade == ItemGrade.Masterful ? "[FlawlessMaterials]" : Grade == ItemGrade.Flawless ? "[UltimateMaterials]" : Grade == ItemGrade.Regular ? "[ExcellentMaterials]" : "";
    }

    public string GetUpgradeMaterialAmount() {
        return IsMajorItem() ? "x2" : "";
    }

    public bool CheckIfEnoughUpgradeMaterials() {
        int amount_required = IsMajorItem() ? 2 : 1;
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
        return (Type == ItemType.Heavy || Type == ItemType.Light || Type == ItemType.Ranged || Type == ItemType.Gloves || Type == ItemType.Helmet || Type == ItemType.Outfit || Type == ItemType.Boots || Type == ItemType.Tool) && Grade != ItemGrade.Ultimate;
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
        return Label.Get(GetType().ToString()) + " (" + Label.Get("ItemGrade_" + Grade.ToString() + "_Colored") + ")";
    }

    public virtual string GetDescription(bool detailed = false)
    {
        string description = "";
        if(Type == Constants.ItemType.Heavy || Type == Constants.ItemType.Light || Type == Constants.ItemType.Ranged)
        {
            description += "<link=\"Stat_" + Type +"Injury_Description\"><sprite name=\"" + Type.ToString() + "Injury\"></link>" + Utils.GetFormattedFloat(BaseInjury) + " <link=\"Stat_" + Type +"Stagger_Description\"><sprite name=\"" + Type.ToString() + "Stagger\"></link>" + Utils.GetFormattedFloat(BaseStagger) +" <link=\"Stat_" + Type +"AttackSpeed_Description\"><sprite name=\"" + WeaponCategory.ToString() + "AttackSpeed\"></link>" + Utils.GetFormattedFloat(BaseAttackSpeed, 2) + "\n\n";
        }
        return description;
    }

    public virtual void ExtraBehaviourOnEquip() {}
    public virtual void ExtraBehaviourOnUnequip() {}

    public void Equip(int item_slot = 0)
    {
        switch (Type) 
        {
            case ItemType.Heavy: {SaveFile.Instance.EquippedHeavyWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHeavyWeapon, this); break;}
            case ItemType.Light: {SaveFile.Instance.EquippedLightWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedLightWeapon, this); break;}
            case ItemType.Ranged: {SaveFile.Instance.EquippedRangedWeapon = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedRangedWeapon, this); break;}
            case ItemType.Gloves: {SaveFile.Instance.EquippedGloves = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedGloves, this); break;}
            case ItemType.Helmet: {SaveFile.Instance.EquippedHelmet = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHelmet, this); break;}
            case ItemType.Outfit: {SaveFile.Instance.EquippedOutfit = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedOutfit, this); break;}
            case ItemType.Boots: {SaveFile.Instance.EquippedBoots = this; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedBoots, this); break;}
        }
        if (Type == ItemType.Tool)
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
        ActivateItemEffects();
        if(Type == ItemType.Heavy)
        {
            Player.Instance.HeavyInjury.Base = BaseInjury;
            Player.Instance.HeavyStagger.Base = BaseStagger;
            Player.Instance.HeavyAttackSpeed.Base = BaseAttackSpeed;
        }
        else if (Type == ItemType.Light)
        {
            Player.Instance.LightInjury.Base = BaseInjury;
            Player.Instance.LightStagger.Base = BaseStagger;
            Player.Instance.LightAttackSpeed.Base = BaseAttackSpeed;
        }
        else if (Type == ItemType.Ranged)
        {
            Player.Instance.RangedInjury.Base = BaseInjury;
            Player.Instance.RangedStagger.Base = BaseStagger;
            Player.Instance.RangedAttackSpeed.Base = BaseAttackSpeed;
        }
        if (Type != ItemType.Tool)
        {
            Utils.CopyItemAppearanceForPlayer(Type, GetType().ToString() + "_" + Grade.ToString());
        }
        ExtraBehaviourOnEquip();
    }

    public void ActivateItemEffects() {
        if(Type == ItemType.Tool || Type == ItemType.Quest) {
            return;
        }
        bool IsWeapon = Type == ItemType.Heavy || Type == ItemType.Light || Type == ItemType.Ranged;
        List<Effect> firstEffects = new List<Effect>();
        foreach(ItemEffect itemEffect in FirstItemEffects) {
            firstEffects = firstEffects.Concat(EffectList.GetEffect(itemEffect.EffectName, itemEffect.PortionOfPowerBudget * GetItemFirstEffectPB(), ToString())).ToList();
        }
        foreach (Effect mod in firstEffects)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponType == Type) {
                mod.PowerBudget = GetItemFirstEffectPB();
                mod.IsRemovable = false;
                mod.ShowsInMenu = false;
                Player.Instance.AddEffect(mod);
            }
        }
        if(GetItemSecondEffectPB() == 0) {
            ActiveEffects = firstEffects;
            return;
        }
        List<Effect> secondEffects = new List<Effect>();
        foreach(ItemEffect itemEffect in SecondItemEffects) {
            secondEffects = secondEffects.Concat(EffectList.GetEffect(itemEffect.EffectName, itemEffect.PortionOfPowerBudget * GetItemSecondEffectPB(), ToString())).ToList();
        }
        foreach (Effect mod in secondEffects)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponType == Type) {
                mod.PowerBudget = GetItemSecondEffectPB();
                mod.IsRemovable = false;
                mod.ShowsInMenu = false;
                Player.Instance.AddEffect(mod);
            }
        }
        ActiveEffects = firstEffects.Concat(secondEffects).ToList();
    }

    public void DeactivateItemEffects() {
        bool IsWeapon = Type == ItemType.Heavy || Type == ItemType.Light || Type == ItemType.Ranged;
        foreach (Effect mod in ActiveEffects)
        {
            if(!IsWeapon || mod.RemainsActiveInOtherStances || Player.Instance?.CurrentStance?.WeaponType == Type) {
                Player.Instance.EndEffect(mod);
            }
        }
        ActiveEffects.Clear();
    }

    public void Unequip(int item_slot = 0)
    {
        if(Player.Instance == null) {
            return;
        }
        IsEquipped = false;
        switch (Type) 
        {
            case ItemType.Heavy: {SaveFile.Instance.EquippedHeavyWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHeavyWeapon, null); break;}
            case ItemType.Light: {SaveFile.Instance.EquippedLightWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedLightWeapon, null); break;}
            case ItemType.Ranged: {SaveFile.Instance.EquippedRangedWeapon = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedRangedWeapon, null); break;}
            case ItemType.Gloves: {SaveFile.Instance.EquippedGloves = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedGloves, null); break;}
            case ItemType.Helmet: {SaveFile.Instance.EquippedHelmet = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedHelmet, null); break;}
            case ItemType.Outfit: {SaveFile.Instance.EquippedOutfit = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedOutfit, null); break;}
            case ItemType.Boots: {SaveFile.Instance.EquippedBoots = null; EventManager.ItemEquipped.Invoke(SaveFile.Instance.EquippedBoots, null); break;}
        }
        if(Type == ItemType.Tool)
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
        DeactivateItemEffects();
        if(Type != ItemType.Heavy && Type != ItemType.Light && Type != ItemType.Ranged && Type != ItemType.Tool)
        {
            Utils.CopyItemAppearanceForPlayer(Type, "Default");
            if(Type == ItemType.Helmet && Area.ComponentInstance != null && Area.ComponentInstance.ApplyPlayerCamouflage) {
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
        return IsEquipped == false && Type != ItemType.Quest;
    }

    public bool CheckIfItemHasSeparateItemIcon()
    {
        return Type != Constants.ItemType.Heavy && Type != Constants.ItemType.Light && Type != Constants.ItemType.Ranged;
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

    public class ItemEffect {
        public string EffectName;
        public float PortionOfPowerBudget;
        public ItemEffect(string effect_name, float portion_of_power_budget = 1) {
            EffectName = effect_name;
            PortionOfPowerBudget = portion_of_power_budget;
        }
    }
}
