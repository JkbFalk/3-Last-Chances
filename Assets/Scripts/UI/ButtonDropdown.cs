using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static InventoryTile;

public class ButtonDropdown : MonoBehaviour
{
    public string PathToLabel = "Template/Item/Item Label";
    public string PathToContent = "Dropdown List/";
    public List<UnityEngine.UI.Button> Buttons;
    public List<InventoryTile.InventoryActions> Actions;
    public Item Item;
    public bool IsExpanded = false;
    public void Start()
    {
        transform.Find(PathToLabel).gameObject.AddComponent<LabelInitializer>();
    }


    public void Show(Item item = null)
    {
        SetOptions(item);
        if(MenuManager.Instance.CurrentOpenDropdown != null && MenuManager.Instance.CurrentOpenDropdown != this)
        {
            MenuManager.Instance.CurrentOpenDropdown.Hide();
        }
        MenuManager.Instance.CurrentOpenDropdown = this;
        IsExpanded = true;
        gameObject.SetActive(true);
        EventManager.CancelButtonPressed.AddListener(Hide);
        EventManager.ExitMenu.AddListener(Hide);
    }

    public void SetOptions(Item item = null)
    {
        Transform template = transform.Find("Template");
        for (int i = template.childCount - 1; i > 0; i--)
        {
            if (template.GetChild(i).gameObject.name != "Image" && template.GetChild(i).gameObject.activeSelf)
            {
                template.GetChild(i).gameObject.SetActive(false);
                MonoBehaviour.Destroy(template.GetChild(i).gameObject);
            }
        }
        int option = 0;
        foreach (InventoryTile.InventoryActions action in Actions)
        {
            GameObject gameObject = MonoBehaviour.Instantiate(transform.Find("Template/Item").gameObject);
            gameObject.SetActive(true);
            gameObject.transform.SetParent(transform.Find("Template"));
            gameObject.name = option.ToString();
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            if(action == InventoryActions.Buy)
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().string_params = new() {Utils.GetFormattedInteger(item.BuyPrice)};
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "}";
            }
            else if(action == InventoryActions.Upgrade)
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "} (" + Item.GetUpgradePrice() + ")"; 
            }
            else if(action == InventoryActions.UpgradeGradeTool)
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "} (" + Item.GetToolGradeUpgradePrice() + ")"; 
            }
            else if(action == InventoryActions.UpgradeUsesTool)
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().string_params = new() {Utils.GetFormattedInteger(Item.MaxAmount), Utils.GetFormattedInteger(Item.MaxAmount + 1)};
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "} (" + Item.GetToolAmountUpgradePrice() + ")"; 
            }
            else if (action == InventoryActions.Sell)
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "} (" + (Item.Type == Constants.ItemType.Tool ? "[Money]" + Utils.GetFormattedInteger(Item.Amount * Item.SellPrice) : "[Money]" + Utils.GetFormattedInteger(Item.SellPrice)) + ")";
            }
            else
            {
                gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().OriginalValue = "{Inventory" + action.ToString() + "}";
            }
            gameObject.transform.Find("Item Label").GetComponent<LabelInitializer>().Start();

            option++;
        }
        Transform item_parent = transform.Find("Template");
        Buttons = new List<UnityEngine.UI.Button>();
        foreach(Transform child in item_parent)
        {
            UnityEngine.UI.Button b = child.GetComponent<UnityEngine.UI.Button>();
            if (b != null && b.gameObject.name != "Item")
            {
                Buttons.Add(b);
            }
        }
        for (int i = 0; i < Buttons.Count; i++)
        {
            Navigation nav = Buttons[i].navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = i == 0 ? Buttons[Buttons.Count - 1] : Buttons[i - 1];
            nav.selectOnDown = i == Buttons.Count - 1 ? Buttons[0] : Buttons[i + 1];
            Buttons[i].navigation = nav;
        }
    }

    public void Hide() 
    {
        IsExpanded = false;
        gameObject.SetActive(false);
        if(Settings.Instance.ControlScheme == "Gamepad" && !MenuManager.Instance.ConfirmPromptActive)
        {
            EventSystem.current.SetSelectedGameObject(transform.parent.gameObject);
        }
        EventManager.CancelButtonPressed.RemoveListener(Hide);
        EventManager.ExitMenu.RemoveListener(Hide);
        MenuManager.Instance.CurrentOpenDropdown = null;
    }

    public void Clicked(UnityEngine.UI.Button button)
    {
        if(transform.parent.GetComponent<InventoryTile>() != null) {
            transform.parent.GetComponent<InventoryTile>().OptionChosen(Int32.Parse(button.gameObject.name));
        }
        else {
            transform.parent.GetComponent<ShopTile>().OptionChosen(Int32.Parse(button.gameObject.name), Item);
        }
        IsExpanded = false;
        gameObject.SetActive(false);
    }
}
