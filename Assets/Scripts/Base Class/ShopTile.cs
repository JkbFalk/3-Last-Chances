using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopTile : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerEnterHandler
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
    public string ShopName = "GenericShop";
    public bool EquipmentTile = false;
    public List<InventoryTile.InventoryActions> AvailableActions = new List<InventoryTile.InventoryActions>();
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
        if(Item == null)
        {
            return;
        }
        AvailableActions.Clear();
        AvailableActions.Add(InventoryTile.InventoryActions.Buy);

        Dropdown = transform.Find("Dropdown").GetComponent<ButtonDropdown>();
        Dropdown.Item = Item;
        Dropdown.Actions = AvailableActions;
        if(EquipmentTile == false)
        {
            switch (Item.Grade)
            {
                case Item.ItemGrade.Regular: transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(Colors.ItemGradeRegular); break;
                case Item.ItemGrade.Excellent: transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(Colors.ItemGradeExcellent); break;
                case Item.ItemGrade.Masterful: transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(Colors.ItemGradeMasterful); break;
                case Item.ItemGrade.Flawless: transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(Colors.ItemGradeFlawless); break;
                case Item.ItemGrade.Ultimate: transform.Find("Grade Indicator").GetComponent<Image>().color = Colors.GetColorFromCode(Colors.ItemGradeUltimate); break;
            }
        }
        Dropdown.SetOptions(Item);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Item != null && eventData.button == PointerEventData.InputButton.Left && Dropdown.IsExpanded)
        {
            Dropdown.Hide();
        }
        else if(Item != null && eventData.button == PointerEventData.InputButton.Left && Dropdown.IsExpanded == false)
        {
            MenuManager.Instance.ShowShopItemDetails(Item);
            if(AvailableActions.Count > 0) {
                Dropdown.Show(Item);
            }
        }
    }

    public void OptionChosen(int option, Item item = null)
    {
        InventoryTile.InventoryActions action = AvailableActions[option];
        Item existingItem = SaveFile.Instance.Inventory.FirstOrDefault(found_item => found_item.GetType() == item.GetType() && item.Grade == found_item.Grade);
        if (action == InventoryTile.InventoryActions.Buy)
        {
            if(SaveFile.Instance.Money < Item.BuyPrice) {
                CanvasElements.ShopMoneyDisplay.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FF000F");
                GameController.Instance.WaitAndRunMethodRealtime(1f, ReturnRegularMoneyDisplayColor);
                Utils.PlaySoundEffect(null, "UI/NotEnoughMoney");
            }
            else {
                SaveFile.Instance.Money -= Item.BuyPrice;
                Item i = (Item)Activator.CreateInstance(Item.GetType(), new object[] {Item.Grade});
                i.Amount = 1;
                SaveFile.Instance.AddItem(i);
                if(Item.CanOnlyBuyOnce || (Item.Type == Constants.ItemType.Tool && existingItem != null && existingItem.Amount == 9)) {
                    MonoBehaviour.Destroy(gameObject);
                    SaveFile.Instance.AddFlag(ShopName + "_" + Item.GetType() + "_" + Item.Grade + "_[Cycle]");
                }
            }
        }
        if(Settings.Instance.ControlScheme == "Gamepad" && !MenuManager.Instance.ConfirmPromptActive)
        {
            GetComponent<Button>().Select();
        }
    }

    public void ReturnRegularMoneyDisplayColor() {
        CanvasElements.ShopMoneyDisplay.GetComponent<TextMeshProUGUI>().color = Colors.GetColorFromCode("#FFDE4E");
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        MenuManager.Instance.ShowShopItemDetails(Item);
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
        MenuManager.Instance.ShowShopItemDetails(Item);
        MenuManager.Instance.CurrentlySelectedShopTile = this;
    }

    public void OnDeselect(BaseEventData eventData)
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
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if(Item != null)
        {
            MenuManager.Instance.ShowShopItemDetails(Item);
            if(AvailableActions.Count > 0) {
                Dropdown.Show();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(MenuManager.Instance.CurrentDetailedShopItemDescription != Item) {
            MenuManager.Instance.ShowShopItemDetails(Item);
        }
    }
}
