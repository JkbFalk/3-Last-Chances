using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryTile : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Item _item;
    public Item Item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
        }
    }
    public bool CursorHoveringOver = false;
    public bool EquipmentTile = false;
    public enum InventoryActions { Equip, EquipTo1, EquipTo2, Sell, Upgrade, Unequip, UnequipFrom1, UnequipFrom2, Use, Buy, UpgradeGradeTool, UpgradeUsesTool }
    public List<InventoryActions> AvailableActions = new List<InventoryActions>();
    public ButtonDropdown Dropdown;
    private TextMeshProUGUI _amountDisplay;
    public TextMeshProUGUI AmountDisplay
    {
        get
        {
            if (_amountDisplay == null)
            {
                _amountDisplay = transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            }
            return _amountDisplay;
        }
    }
    private GameObject _dragIndicator;

    public void Start()
    {
        if(EquipmentTile)
        {
            GetComponent<Image>().color = Colors.EquipmentTileColor;
            transform.Find("Background").GetComponent<Image>().color = Colors.EquipmentTileBackground;
        }
    }
    
    public void InitializeOptions() 
    {
        if (Item == null)
        {
            return;
        }

        AvailableActions.Clear();

        if (Item.Type == Constants.ItemType.Tool)
        {
            AvailableActions.Add(InventoryActions.Use);
        }
        if (Item.Type != Constants.ItemType.Heavy && Item.Type != Constants.ItemType.Light && Item.Type != Constants.ItemType.Ranged && Item.Type != Constants.ItemType.Tool && Item.IsEquipped)
        {
            AvailableActions.Add(InventoryActions.Unequip);
        }
        else if (Item.CheckIfCanEquipItem() && !(Item.Type == Constants.ItemType.Tool))
        {
            AvailableActions.Add(InventoryActions.Equip);
        }
        if ((Item.Type == Constants.ItemType.Tool) && SaveFile.Instance.EquippedItem1 != Item)
        {
            AvailableActions.Add(InventoryActions.EquipTo1);
        }
        if ((Item.Type == Constants.ItemType.Tool) && SaveFile.Instance.EquippedItem2 != Item)
        {
            AvailableActions.Add(InventoryActions.EquipTo2);
        }
        if (Item.IsEquipped == false && Item.Type != Constants.ItemType.Quest && Item.Type != Constants.ItemType.Tool)
        {
            AvailableActions.Add(InventoryActions.Sell);
        }
        if (SaveFile.Instance.EquippedItem1 == Item) {
            AvailableActions.Add(InventoryActions.UnequipFrom1);
        }
        if (SaveFile.Instance.EquippedItem2 == Item) {
            AvailableActions.Add(InventoryActions.UnequipFrom2);
        }
        if (Item.Type != Constants.ItemType.Quest && Item.Type != Constants.ItemType.Tool && Item.Grade != Item.ItemGrade.Ultimate) {
            AvailableActions.Add(InventoryActions.Upgrade);
        }
        if ((Item.Type == Constants.ItemType.Tool) && SaveFile.Instance.ToolGrades[Item.GetType()] != Item.ItemGrade.Ultimate)
        {
            AvailableActions.Add(InventoryActions.UpgradeGradeTool);
        }
        if ((Item.Type == Constants.ItemType.Tool) && SaveFile.Instance.ToolMaxAmounts[Item.GetType()] != 6)
        {
            AvailableActions.Add(InventoryActions.UpgradeUsesTool);
        }

        Dropdown = transform.Find("Dropdown").GetComponent<ButtonDropdown>();
        Dropdown.Item = Item;
        Dropdown.Actions = AvailableActions;

        if (!EquipmentTile || Item != null)
        {
            string colorCode = EquipmentTile
                ? (Item.Grade switch {
                    Item.ItemGrade.Regular => Colors.ItemGradeRegularDarker,
                    Item.ItemGrade.Excellent => Colors.ItemGradeExcellentDarker,
                    Item.ItemGrade.Masterful => Colors.ItemGradeMasterfulDarker,
                    Item.ItemGrade.Flawless => Colors.ItemGradeFlawlessDarker,
                    Item.ItemGrade.Ultimate => Colors.ItemGradeUltimateDarker,
                    _ => Colors.ItemGradeRegularDarker
                })
                : (Item.Grade switch {
                    Item.ItemGrade.Regular => Colors.ItemGradeRegular,
                    Item.ItemGrade.Excellent => Colors.ItemGradeExcellent,
                    Item.ItemGrade.Masterful => Colors.ItemGradeMasterful,
                    Item.ItemGrade.Flawless => Colors.ItemGradeFlawless,
                    Item.ItemGrade.Ultimate => Colors.ItemGradeUltimate,
                    _ => Colors.ItemGradeRegular
                });

            transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(colorCode);
        }
        else if (EquipmentTile && Item == null)
        {
            transform.Find("Grade Indicator").GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Item != null && eventData.button == PointerEventData.InputButton.Left && Dropdown.IsExpanded)
        {
            DeselectTile();
            Dropdown.Hide();
        }
        else if (Item != null && eventData.button == PointerEventData.InputButton.Left && Dropdown.IsExpanded == false)
        {
            MenuManager.Instance.ShowItemDetails(Item);
            if (AvailableActions.Count > 0)
            {
                Dropdown.Show();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            MenuManager.Instance.ShowItemDetails(Item);
        }
    }

    public void OptionChosen(int option)
    {
        InventoryActions action = AvailableActions[option];
        if (action == InventoryActions.Use)
        {
            if(!Utils.CheckIfItemGradeSufficientLevel(Item.Grade, "TooLowLevelToUseWarning")) {

            }
            else if(Player.Instance.ToolCooldown != null) {
                NotificationController.ShowTextNotification("ItemsOnCooldownWarning");
            }
            else {
                Player.Instance.Actions.UseAbility(Item.OnUseAbility, true, null, Item);
            }
        }
        if (action == InventoryActions.Equip)
        {
            GetEquipmentTileForItemType(Item.Type).EquipItem(Item);
        }
        else if (action == InventoryActions.Unequip)
        {
            Item.TileInInventory.UnequipItem();
        }
        else if (action == InventoryActions.EquipTo1)
        {
            MenuManager.Instance.Item1EquipmentSlot.EquipItem(Item, 1);
            MenuManager.Instance.UpdateEquippedUsableItems();
        }
        else if (action == InventoryActions.EquipTo2)
        {
            MenuManager.Instance.Item2EquipmentSlot.EquipItem(Item, 2);
            MenuManager.Instance.UpdateEquippedUsableItems();
        }
        else if (action == InventoryActions.UnequipFrom1)
        {
            MenuManager.Instance.Item1EquipmentSlot.UnequipItem(1);
            MenuManager.Instance.UpdateEquippedUsableItems();
        }
        else if (action == InventoryActions.UnequipFrom2)
        {
            MenuManager.Instance.Item2EquipmentSlot.UnequipItem(2);
            MenuManager.Instance.UpdateEquippedUsableItems();
        }
        else if (action == InventoryActions.Sell)
        {
            GameController.Instance.ShowConfirmModal(string.Format(Label.Get("SellItemConfirmation"), new List<string> { Item.GetItemName(), Item.Type == Constants.ItemType.Tool ? (Item.Amount * Item.SellPrice).ToString() : Item.SellPrice.ToString() }.ToArray()), SellItem, "InventorySell");
        }
        else if (action == InventoryActions.Upgrade)
        {
            if(!Utils.CheckIfItemGradeSufficientLevel(Item.Grade, "TooLowLevelToUpgradeWarning")) {}
            else if(SaveFile.Instance.Money < Item.UpgradePrice[Item.GradeIndex] || Item.CheckIfEnoughUpgradeMaterials() == false)
            {
                NotificationController.ShowTextNotification("NotEnoughMaterialsToUpgradeWarning");
            }
            else {
                GameController.Instance.ShowConfirmModal(string.Format(Label.Get("UpgradeItemConfirmation"), new List<string> { Item.GetItemName(), Item.GetUpgradePrice()}.ToArray()), UpgradeItem, "InventoryUpgrade");
            }
        }
        else if (action == InventoryActions.UpgradeGradeTool)
        {
            if(!Utils.CheckIfItemGradeSufficientLevel(Item.Grade, "TooLowLevelToUpgradeWarning")) {}
            else if(Item.CheckIfEnoughToolMaterialsForGradeUpgrade() == false)
            {
                NotificationController.ShowTextNotification("NotEnoughToolMaterialsToUpgradeWarning");
            }
            else {
                GameController.Instance.ShowConfirmModal(string.Format(Label.Get("ToolGradeUpgradeConfirmation"), new List<string> { Item.GetItemName(), Item.GetToolGradeUpgradePrice()}.ToArray()), UpgradeToolGrade, "InventoryUpgrade");
            }
        }
        else if (action == InventoryActions.UpgradeUsesTool)
        {
            if(Item.CheckIfEnoughToolMaterialsForAmountUpgrade() == false)
            {
                NotificationController.ShowTextNotification("NotEnoughToolMaterialsToUpgradeWarning");
            }
            else {
                GameController.Instance.ShowConfirmModal(string.Format(Label.Get("ToolAmountUpgradeConfirmation"), new List<string> { Item.GetItemName(), Item.GetToolAmountUpgradePrice()}.ToArray()), UpgradeToolAmount, "InventoryUpgrade");
            }
        }
        if(Settings.Instance.ControlScheme == "Gamepad" && !GameController.Instance.ConfirmPromptActive)
        {
            GetComponent<Button>().Select();
        }
    }

    public void SellItem()
    {
        Utils.PlaySoundEffect(null, "UI/ItemPickedUp", 0.6f);
        SaveFile.Instance.Money += Item.SellPrice;
        Item.RemoveItemFromInventory();
    }

    public void UpgradeItem()
    {
        SaveFile.Instance.Money -= Item.UpgradePrice[Item.GradeIndex];
        Item materials = SaveFile.Instance.Inventory.FirstOrDefault(item => item is Quest_UpgradeMaterials && item.Grade == Item.GetGradeForIndex(Item.GradeIndex + 1));
        materials.Amount -= Item.IsMajorItem() ? 2 : 1;
        UpdateItemAfterUpgrade();
    }

    public void UpgradeToolGrade() {
        SaveFile.Instance.UpgradeTool(Item.GetType());
        UpdateItemAfterUpgrade();
    }

    public void UpgradeToolAmount() {
        SaveFile.Instance.IncreaseMaxToolUses(Item.GetType());
        UpdateItemAfterUpgrade(true);
    }

    public void UpdateItemAfterUpgrade(bool is_max_amount_upgrade = false) {
        if(is_max_amount_upgrade == false) {
            Item upgraded_item = (Item)Activator.CreateInstance(Item.GetType(), new object[] { Item.GetGradeForIndex(Item.GradeIndex + 1) });
            if(Item.Type != Constants.ItemType.Tool && GetEquipmentTileForItemType(Item.Type).Item == Item) {
                Item.Unequip();
                upgraded_item.Equip();
            }
            if(Item.Type == Constants.ItemType.Tool && MenuManager.Instance.Item1EquipmentSlot.Item == Item) {
                MenuManager.Instance.Item1EquipmentSlot.UnequipTool(this, 1);
                upgraded_item.Equip(1);
            }
            if(Item.Type == Constants.ItemType.Tool && MenuManager.Instance.Item2EquipmentSlot.Item == Item) {
                MenuManager.Instance.Item1EquipmentSlot.UnequipTool(this, 2);
                upgraded_item.Equip(2);
            }
            upgraded_item.TileInEquipment = Item.TileInEquipment;
            upgraded_item.TileInInventory = Item.TileInInventory;
            Item = upgraded_item;
        }
        MenuManager.Instance.HideItemDetailsWindow();
        MenuManager.Instance.ShowItemDetails(Item);
        if (Item.TileInEquipment != null)
        {
            Item.TileInEquipment.InitializeOptions();
        }
        Item.TileInInventory.InitializeOptions();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        MenuManager.Instance.ShowItemDetails(Item);
        _dragIndicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ItemDragIndicator")) as GameObject;
        _dragIndicator.GetComponent<Image>().sprite = transform.Find("Image").GetComponent<Image>().sprite;
        _dragIndicator.transform.SetParent(MenuManager.Instance.transform);
        transform.Find("Image").GetComponent<Image>().enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragIndicator.transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.Find("Image").GetComponent<Image>().enabled = true;
        List<RaycastResult> results = new List<RaycastResult>();
        MenuManager.Instance.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
        if(results != null && results.Count > 0 && Item != null && Item.Type != Constants.ItemType.Tool)
        {
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "Equipment" && Item.CheckIfCanEquipItem())
                {
                    GetEquipmentTileForItemType(Item.Type).EquipItem(Item);
                }
                if(result.gameObject.name == "Inventory" && Item.IsEquipped && EquipmentTile && Item.Type != Constants.ItemType.Heavy && Item.Type != Constants.ItemType.Light && Item.Type != Constants.ItemType.Ranged)
                {
                    UnequipItem();
                }
            }
        }
        else if (results != null && results.Count > 0)
        {
            bool item_equipped_in_slot_2 = false;
            bool hit_equipment_object = false;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "Item 2")
                {
                    MenuManager.Instance.Item2EquipmentSlot.EquipItem(Item, 2);
                    MenuManager.Instance.UpdateEquippedUsableItems();
                    item_equipped_in_slot_2 = true;
                }
                else if (result.gameObject.name == "Equipment")
                {
                    hit_equipment_object = true;
                }
            }
            if (item_equipped_in_slot_2 == false && hit_equipment_object)
            {
                MenuManager.Instance.Item1EquipmentSlot.EquipItem(Item, 1);
                MenuManager.Instance.UpdateEquippedUsableItems();
            }
        }
        MonoBehaviour.Destroy(_dragIndicator.gameObject);
    }

    public void EquipItem(Item item, int item_slot = 0)
    {
        if (EquipmentTile == false)
        {
            return;
        }
        if(!Utils.CheckIfItemGradeSufficientLevel(item.Grade, "TooLowLevelToEquipWarning")) {
            return;
        }
        if (Item != null)
        {
            UnequipItem(item_slot);
        }
        Item = item;
        Item.Equip(item_slot);
        item.TileInEquipment = this;
        transform.Find("Default Image").gameObject.SetActive(false);
        transform.Find("Image").gameObject.SetActive(true);
        transform.Find("Image").GetComponent<UnityEngine.U2D.Animation.SpriteResolver>().SetCategoryAndLabel(item.Type.ToString() + " Icons", item.GetType().ToString().Split('_')[1]);
        transform.Find("Image").GetComponent<UnityEngine.U2D.Animation.SpriteResolver>().ResolveSpriteToSpriteRenderer();
        transform.Find("Image").GetComponent<Image>().sprite = transform.Find("Image").GetComponent<SpriteRenderer>().sprite;
        if(Item.TileInInventory != null)
        {
            Item.TileInInventory.InitializeOptions();
            Item.TileInInventory.transform.Find("Equipped Indicator").gameObject.SetActive(true);
        }
        InitializeOptions();
        if(item_slot != 0)
        {
            RefreshUsableItemUI(item_slot, item);
        }
    }

    public static void RefreshUsableItemUI(int item_slot, Item item) {
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().enabled = item != null;
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Uses").gameObject.SetActive(item != null);
        if(item == null || item.TileInEquipment == null || item.TileInEquipment.AmountDisplay == null) {
            return;
        }
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Uses").gameObject.SetActive(item.Type == Constants.ItemType.Tool);
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().enabled = true;
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<UnityEngine.U2D.Animation.SpriteResolver>().SetCategoryAndLabel(item.Type.ToString() + " Icons", item.GetType().ToString().Split('_')[1]);
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<UnityEngine.U2D.Animation.SpriteResolver>().ResolveSpriteToSpriteRenderer();
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().sprite = UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<SpriteRenderer>().sprite;
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().color = item == null ? Color.black : Color.white;
        UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Uses").gameObject.SetActive(item.Type == Constants.ItemType.Tool);

        item.TileInEquipment.AmountDisplay.text = item.Type != Constants.ItemType.Tool ? "" : item.Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[item.GetType()];
        item.Amount = item.Amount;
    }

    public void UnequipItem(int item_slot = 0)
    {
        if (SaveFile.Instance.EquippedItem1 == Item && item_slot == 1) {
            UIManager.Objects.Items.transform.Find("1/Icon").GetComponent<Image>().sprite = null;
            UIManager.Objects.Items.transform.Find("1/Icon").GetComponent<Image>().color = Color.black;
        }
        if (SaveFile.Instance.EquippedItem2 == Item && item_slot == 2) {
            UIManager.Objects.Items.transform.Find("2/Icon").GetComponent<Image>().sprite = null;
            UIManager.Objects.Items.transform.Find("2/Icon").GetComponent<Image>().color = Color.black;
        }
        Item.Unequip(item_slot);
        if(Item.TileInInventory != null)
        {
            Item.TileInInventory.transform.Find("Equipped Indicator").gameObject.SetActive(false);
        }
        if (item_slot == 1)
        {
            Item.TileInEquipment.transform.Find("Grade Indicator").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Uses").gameObject.SetActive(false);
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = 0;
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().enabled = false;
            Item.TileInEquipment = MenuManager.Instance.Item2EquipmentSlot;
            UnequipTool(MenuManager.Instance.Item1EquipmentSlot, 1);
        }
        else if(item_slot == 2)
        {
            Item.TileInEquipment.transform.Find("Grade Indicator").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Uses").gameObject.SetActive(false);
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Cooldown").GetComponent<Image>().fillAmount = 0;
            UIManager.Objects.Items.transform.Find(item_slot.ToString() + "/Icon").GetComponent<Image>().enabled = false;
            Item.TileInEquipment = MenuManager.Instance.Item1EquipmentSlot;
            UnequipTool(MenuManager.Instance.Item2EquipmentSlot, 2);
        }
        else
        {
            InventoryTile eqTile = Item.TileInEquipment;
            InventoryTile invTile = Item.TileInInventory;
            Item.TileInEquipment.AmountDisplay.text = "";
            Item.TileInEquipment.transform.Find("Default Image").gameObject.SetActive(true);
            Item.TileInEquipment.transform.Find("Image").gameObject.SetActive(false);
            Item.TileInEquipment.transform.Find("Grade Indicator").GetComponent<Image>().color = new Color(255, 255, 255, 0);
            eqTile.Item = null;
            if(invTile != null)
            {
                invTile.InitializeOptions();
            }
        }
    }

    public void UnequipTool(InventoryTile tile, int item_slot) {
        InventoryTile invTile = tile.Item.TileInInventory;
        tile.Item = null;
        tile.AmountDisplay.text = "";
        tile.transform.Find("Default Image").gameObject.SetActive(true);
        tile.transform.Find("Image").gameObject.SetActive(false);
        if(invTile != null) {
            invTile.InitializeOptions();
            invTile.transform.Find("Equipped Indicator").gameObject.SetActive(SaveFile.Instance.EquippedItem1 == invTile.Item || SaveFile.Instance.EquippedItem2 == invTile.Item);
        }
    }

    public static InventoryTile GetEquipmentTileForItemType(Constants.ItemType type)
    {
        if (type == Constants.ItemType.Heavy || type == Constants.ItemType.Light || type == Constants.ItemType.Ranged)
        {
            return Utils.GetGameObject("Menu/Inventory Window/Equipment/Left Panel/" + type.ToString()).GetComponent<InventoryTile>();
        }
        else if (type == Constants.ItemType.Helmet || type == Constants.ItemType.Outfit || type == Constants.ItemType.Boots || type == Constants.ItemType.Gloves)
        {
            return Utils.GetGameObject("Menu/Inventory Window/Equipment/Right Panel/" + type.ToString()).GetComponent<InventoryTile>();
        }
        return null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (EquipmentTile)
        {
            GetComponent<Image>().color = Colors.EquipmentTileColorSelected;
            transform.Find("Background").GetComponent<Image>().color = Colors.EquipmentTileBackgroundSelected;
        }
        else
        {
            transform.Find("Stroke").GetComponent<Image>().color = Colors.UISelected;
        }
        MenuManager.Instance.ShowItemDetails(Item);
        MenuManager.Instance.CurrentlySelectedTile = this;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectTile();
    }

    public void DeselectTile()
    {
        if (EquipmentTile)
        {
            GetComponent<Image>().color = Colors.EquipmentTileColor;
            transform.Find("Background").GetComponent<Image>().color = Colors.EquipmentTileBackground;
        }
        else
        {
            transform.Find("Stroke").GetComponent<Image>().color = Color.white;
        }
        MenuManager.Instance.CurrentlySelectedTile = null;
        if (EventSystem.current.alreadySelecting == false)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (Item != null)
        {
            MenuManager.Instance.ShowItemDetails(Item);
            if (AvailableActions.Count > 0)
            {
                Dropdown.Show();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorHoveringOver = true;
        if (Dropdown != null && Dropdown!= null)
        {
            Dropdown.ShowDropdownOverTime();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorHoveringOver = false;
        if (Dropdown != null && Dropdown!= null)
        {
            Dropdown.HideDropdownOverTime();
        }
    }
}
