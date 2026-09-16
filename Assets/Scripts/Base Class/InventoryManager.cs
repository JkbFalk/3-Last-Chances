// FILE: Assets/Scripts/Base Class/InventoryManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance ??= new InventoryManager();

    public List<Item> Inventory => SaveFile.Instance.Inventory;

    public void AddItem(Type itemType, Item.ItemGrade grade = Item.ItemGrade.None, bool showNotification = true)
    {
        Item item = (Item)Activator.CreateInstance(itemType, new object[] { grade });
        AddItem(item, showNotification);
    }

    public void AddItem(Item item, bool showNotification = true)
    {
        Utils.CreateAuditLog("Acquired item (" + item.GetType() + "): " + item.Grade + " , amount: " + item.Amount);
        int sellPrice = CheckSellPriceIfItemIsDuplicate(item);
        if (showNotification)
        {
            NotificationController.ShowItemDropNotification(item, sellPrice);
        }
        if (sellPrice > 0)
        {
            SaveFile.Instance.Money += sellPrice;
            return;
        }

        if (item is Quest_UpgradeMaterials || item is Quest_ToolMaterials)
        {
            Item existingItem = Inventory.FirstOrDefault(i => i.GetType() == item.GetType() && i.Grade == item.Grade);
            if (existingItem != null)
            {
                existingItem.Amount += item.Amount;
                return;
            }
        }

        if (item.Type != Constants.ItemType.Tool && item.Type != Constants.ItemType.Quest && !SaveFile.Instance.FoundItemTypes.Contains(item.GetType()))
        {
            SaveFile.Instance.FoundItemTypes.Add(item.GetType());
        }

        Inventory.Add(item);
        Inventory.OrderBy(i => i.GetType()).ThenBy(i => i.Grade);
        MenuManager.Instance.AddItemToGrid(item);
    }

    public void RemoveItem(Item item)
    {
        Utils.CreateAuditLog("Removed item (" + item.GetType() + "): " + item.Grade + " , amount: " + item.Amount);
        Inventory.Remove(item);
        if (SaveFile.Instance.EquippedItem1 == item)
        {
            MenuManager.Instance.Item1EquipmentSlot.UnequipItem(1);
        }
        if (SaveFile.Instance.EquippedItem2 == item)
        {
            MenuManager.Instance.Item2EquipmentSlot.UnequipItem(2);
        }
        if (item.IsEquipped)
        {
            item.TileInInventory.UnequipItem();
        }
        if (item.TileInInventory != null)
        {
            MonoBehaviour.Destroy(item.TileInInventory.gameObject);
        }
    }

    public bool HasKey(string keyTypeName)
    {
        Type type = Type.GetType(keyTypeName);
        return Inventory.FirstOrDefault(item => item.GetType() == type) != null;
    }

    public int CheckSellPriceIfItemIsDuplicate(Item item)
    {
        Item existingItem = Inventory.FirstOrDefault(i => i.GetType() == item.GetType() && i.Grade == item.Grade);
        if (item.Type == Constants.ItemType.Tool || item.Type == Constants.ItemType.Quest)
        {
            return 0;
        }
        if (existingItem != null)
        {
            return item.SellPrice;
        }
        return 0;
    }

    public void AcquireItem(string itemType, int amount, Item.ItemGrade grade)
    {
        if (string.IsNullOrEmpty(itemType)) return;

        if (itemType == "Money")
        {
            SaveFile.Instance.Money += amount;
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if (itemType.EndsWith("_Unlock") && SaveFile.Instance.UnlockedTools.Contains(Type.GetType(itemType.Replace("_Unlock", ""))))
        {
            Item item = (Item)Activator.CreateInstance(typeof(Quest_ToolMaterials), new object[] { Item.ItemGrade.None });
            item.Amount = 10;
            AddItem(item, false);
            Item item2 = (Item)Activator.CreateInstance(Type.GetType(itemType.Replace("_Unlock", "")), new object[] { Item.ItemGrade.Regular });
            NotificationController.ShowNotificationWithGraphic(Label.Get("ToolDuplicateMessage"), item2.GetIcon(), new List<string> { Label.Get(itemType.Replace("_Unlock", "")), "10" });
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if (itemType.EndsWith("_Unlock"))
        {
            Item item = (Item)Activator.CreateInstance(Type.GetType(itemType.Replace("_Unlock", "")), new object[] { Item.ItemGrade.Regular });
            NotificationController.ShowNotificationWithGraphic(Label.Get("NewToolUnlockMessage"), item.GetIcon(), new List<string> { Label.Get(itemType.Replace("_Unlock", "")) });
            UnlockTool(Type.GetType(itemType.Replace("_Unlock", "")));
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
        else if (!string.IsNullOrWhiteSpace(itemType))
        {
            Item item = (Item)Activator.CreateInstance(Type.GetType(itemType), new object[] { grade });
            item.Amount = amount;
            AddItem(item);
            Utils.PlaySoundEffect(Player.Instance.AudioSource, "UI/ItemPickedUp", 1.2f);
        }
    }

    public void UnlockTool(Type toolType)
    {
        if (SaveFile.Instance.UnlockedTools.Contains(toolType)) return;

        SaveFile.Instance.UnlockedTools.Add(toolType);
        Item itemToAdd = (Item)Activator.CreateInstance(toolType, new object[] { SaveFile.Instance.ToolGrades[toolType] });
        itemToAdd.Amount = SaveFile.Instance.ToolMaxAmounts[toolType];
        AddItem(itemToAdd, false);
    }

    public void UpgradeTool(Type toolType)
    {
        SaveFile.Instance.ToolGrades[toolType] =
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Regular ? Item.ItemGrade.Excellent :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Excellent ? Item.ItemGrade.Masterful :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Masterful ? Item.ItemGrade.Flawless :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Flawless ? Item.ItemGrade.Ultimate : Item.ItemGrade.Ultimate;

        Inventory.FirstOrDefault(item => item is Quest_ToolMaterials).Amount -=
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Excellent ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_EXCELLENT :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Masterful ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_MASTERFUL :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Flawless ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_FLAWLESS :
            SaveFile.Instance.ToolGrades[toolType] == Item.ItemGrade.Ultimate ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_TO_ULTIMATE : 0;

        UpdateToolInventoryTile(toolType);
    }

    public void IncreaseMaxToolUses(Type toolType)
    {
        SaveFile.Instance.ToolMaxAmounts[toolType] = SaveFile.Instance.ToolMaxAmounts[toolType] == 6 ? 6 : SaveFile.Instance.ToolMaxAmounts[toolType] + 1;

        Inventory.FirstOrDefault(item => item is Quest_ToolMaterials).Amount -=
            SaveFile.Instance.ToolMaxAmounts[toolType] == 2 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_2 :
            SaveFile.Instance.ToolMaxAmounts[toolType] == 3 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_3 :
            SaveFile.Instance.ToolMaxAmounts[toolType] == 4 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_4 :
            SaveFile.Instance.ToolMaxAmounts[toolType] == 5 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_5 :
            SaveFile.Instance.ToolMaxAmounts[toolType] == 6 ? Constants.TOOL_MATERIALS_COST_FOR_UPGRADE_MAX_AMOUNT_TO_6 : 0;

        UpdateToolInventoryTile(toolType);
    }

    public void UpdateToolInventoryTile(Type toolType)
    {
        foreach (Transform child in MenuManager.Objects.InventoryItemsTools.transform)
        {
            InventoryTile tile = child.GetComponent<InventoryTile>();
            if (tile.Item.GetType() == toolType)
            {
                tile.InitializeOptions();
                tile.AmountDisplay.text = tile.Item.Amount + "/" + SaveFile.Instance.ToolMaxAmounts[toolType];
            }
        }
    }

    public Item GetItem(Type itemType) => Inventory.FirstOrDefault(item => item.GetType() == itemType);

    public Item GetItem(Type itemType, Item.ItemGrade grade) => Inventory.FirstOrDefault(item => item.GetType() == itemType && item.Grade == grade);
}