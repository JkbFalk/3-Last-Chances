using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using static Item;
using static MenuManager;

public class MenuManager : MonoBehaviour {
    private static MenuManager _instance = null;
    public bool ShopDetailsWindowOpen = false;
    public bool InventoryDetailsWindowOpen = false;
    public bool AbilityDetailsWindowOpen = false;
    public bool SkillTreeTileDetailsWindowOpen = false;
    public Item CurrentDetailedItemDescription;
    public Item CurrentDetailedShopItemDescription;
    public Type CurrentAbilityDescription;
    public GameObject CurrentSkillTreeTileDescription;
    public ShopTile CurrentlySelectedShopTile;
    public InventoryTile CurrentlySelectedTile;
    public int SelectedSortingIndex = 0;
    public GameObject SelectedSorting;
    public LabelInitializer Tooltip;
    public HideOrShowOverTime TooltipDisplay;
    public ChangeTransformOverTime TooltipTransformChange;
    public List<Mission> AllPossibleMissions = new List<Mission>();

    public List<AbilitySelect> AbilityLoadout = new List<AbilitySelect>();
    public List<AbilitySelect> AbilityOverview = new List<AbilitySelect>();
    public List<StanceSelect> StanceLoadout = new List<StanceSelect>();
    public List<StanceSelect> StanceOverview = new List<StanceSelect>();
    public List<AbilityUnlockTile> AbilityUnlockTiles = new List<AbilityUnlockTile>();
    public List<MasteryUnlockTile> MasteryUnlockTiles = new List<MasteryUnlockTile>();
    public List<StanceUnlockTile> StanceUnlockTiles = new List<StanceUnlockTile>();
    public List<PassivePowerUpTile> PowerUpTiles = new List<PassivePowerUpTile>();

    public InventoryTile HeavyEquipmentSlot;
    public InventoryTile LightEquipmentSlot;
    public InventoryTile RangedEquipmentSlot;
    public InventoryTile GlovesEquipmentSlot;
    public InventoryTile HelmetEquipmentSlot;
    public InventoryTile OutfitEquipmentSlot;
    public InventoryTile BootsEquipmentSlot;
    public InventoryTile Item1EquipmentSlot;
    public InventoryTile Item2EquipmentSlot;

    public ButtonDropdown CurrentOpenDropdown;

    public string AbilityBeingChanged;
    public string StanceBeingChanged;
    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameController.Instance.GetComponentInChildren<MenuManager>();
            }
            return _instance;
        }
    }

    public List<Item> EquippedItems {
        get {
            List<Item> items = new List<Item>();
            foreach(Item item in new List<Item> {SaveFile.Instance.EquippedHeavyWeapon, SaveFile.Instance.EquippedLightWeapon, SaveFile.Instance.EquippedRangedWeapon, SaveFile.Instance.EquippedGloves, SaveFile.Instance.EquippedHelmet, SaveFile.Instance.EquippedOutfit, SaveFile.Instance.EquippedBoots, SaveFile.Instance.EquippedItem1, SaveFile.Instance.EquippedItem2}) {
                if(item != null) {
                    items.Add(item);
                }
            }
            return items;
        }
    }

    public int SelectedSkillTree = 0;

    private List<string> _optionsSubMenus = new List<string> { "Gameplay", "Keybinds", "Display", "Audio", "ReportBug",};
    private int _selectedOptionsSubMenu = 0;
    public int SelectedOptionsSubMenu
    {
        get
        {
            return _selectedOptionsSubMenu;
        }
        set
        {
            if(_selectedOptionsSubMenu == value) {
                return;
            }
            int formatted_value = value < 0 ? _optionsSubMenus.Count - 1 : value > _optionsSubMenus.Count - 1 ? 0 : value;
            transform.Find("Options Window/Options/Category Selection/" + _optionsSubMenus[_selectedOptionsSubMenu] + "/Background/Checkmark (Selected)").gameObject.SetActive(false);
            transform.Find("Options Window/Options/Category Selection/" + _optionsSubMenus[_selectedOptionsSubMenu] + "/Background/Checkmark").gameObject.SetActive(true);
            transform.Find("Options Window/Options/" + _optionsSubMenus[_selectedOptionsSubMenu]).gameObject.SetActive(false);
            _selectedOptionsSubMenu = formatted_value;
            transform.Find("Options Window/Options/Category Selection/" + _optionsSubMenus[_selectedOptionsSubMenu] + "/Background/Checkmark (Selected)").gameObject.SetActive(true);
            transform.Find("Options Window/Options/Category Selection/" + _optionsSubMenus[_selectedOptionsSubMenu] + "/Background/Checkmark").gameObject.SetActive(false);
            transform.Find("Options Window/Options/" + _optionsSubMenus[_selectedOptionsSubMenu]).gameObject.SetActive(true);
            PlayerControls.GamepadSelectObjectClosestToCenter();
        }
    }

    private List<string> _subMenus = new List<string> { "Character", "Inventory", "Skill Tree", "Archive", "Tutorials", "Options", };
    private int _selectedSubMenu = 0;
    public int SelectedSubMenu
    {
        get
        {
            return _selectedSubMenu;
        }
        set
        {
            if(_selectedSubMenu == value) {
                return;
            }
            int formatted_value = value < 0 ? _subMenus.Count - 1 : value > _subMenus.Count - 1 ? 0 : value;
            transform.Find("Menu Selection/" + _subMenus[_selectedSubMenu] + "/Background/Checkmark (Selected)").gameObject.SetActive(false);
            transform.Find("Menu Selection/" + _subMenus[_selectedSubMenu] + "/Background/Checkmark").gameObject.SetActive(true);
            transform.Find(_subMenus[_selectedSubMenu] + " Window").gameObject.SetActive(false);
            _selectedSubMenu = formatted_value;
            transform.Find("Menu Selection/" + _subMenus[_selectedSubMenu] + "/Background/Checkmark (Selected)").gameObject.SetActive(true);
            transform.Find("Menu Selection/" + _subMenus[_selectedSubMenu] + "/Background/Checkmark").gameObject.SetActive(false);
            transform.Find(_subMenus[_selectedSubMenu] + " Window").gameObject.SetActive(true);
            if(_selectedSubMenu == 0)
            {
                MenuManager.Instance.HideEffects();
                int count = Player.Instance.CurrentEffects.Where(effect => effect.ShowsInMenu).ToArray().Length;
                transform.Find("Character Window/Abilities/Right-side Panel/ActiveEffects").GetComponent<TextMeshProUGUI>().text = Label.Get("UI_ActiveEffectCount") + count;
                transform.Find("Character Window/Abilities/UI_DisabledInCombat/ActiveEffects").GetComponent<TextMeshProUGUI>().text = Label.Get("UI_ActiveEffectCount") + count;
            }
            if(_selectedSubMenu == 1 && SaveFile.Instance.Inventory.Count > 0)
            {
                MenuManager.Instance.ChangeSelectedInventoryCategory(0);
            }
            if(_selectedSubMenu == 2) {
                for(int i = 0; i < 7; i++) {
                    Utils.ScrollToTopOrBottom(transform.Find("Skill Tree Window/Skill Tree").GetChild(i)); // This canvas contains the scroll rect
                    transform.Find("Skill Tree Window/Skill Tree").GetChild(i).GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
                }
            }
            if(_selectedSubMenu == 4 && Objects.MenuArchive.transform.childCount > 0) {
                Objects.MenuArchive.transform.parent.Find("Scrollbar").GetComponent<Scrollbar>().value = 0;
                for(int i = 0; i < 5; i++) {
                    GameController.Instance.WaitAndRunMethodRealtime(0.05f * i, UpdateHistoryScrollbarPosition);
                }
            }
            if(_selectedSubMenu == 5)
            {
                transform.Find("Tutorials Window/UI_Window/Viewport/Items/UI_TutorialItem").GetComponent<Button>().Select();
            }
            PlayerControls.GamepadSelectObjectClosestToCenter();
        }
    }

    public void ResetAndRefreshAllMenus() {
        Utils.DestroyAllChildren(UIManager.Objects.StanceGaugeContainer.transform);
        SaveFile.Instance.MakeSureAllCorrectTechniquesAndStancesAreUnlocked();
        Tooltip.transform.parent.gameObject.SetActive(false);
        transform.Find("Character Window/Ability Select").gameObject.SetActive(false);
        transform.Find("Character Window/Stance Select").gameObject.SetActive(false);
        transform.Find("Character Window/Energy Select").gameObject.SetActive(false);
        transform.Find("Character Window/Details").gameObject.SetActive(false);
        transform.Find("Character Window/Effects").gameObject.SetActive(false);
        transform.Find("Inventory Window/Details").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details").gameObject.SetActive(false);
        SelectedSubMenu = 0;
        ChangeDisplayedSkillTree(0);
        for(int i = 0; i < 7; i++) {
            transform.Find("Skill Tree Window/Skill Tree").GetChild(i).Find("Scrollbar").GetComponent<Scrollbar>().value = 1;
        }
        if(SaveFile.Instance != null && SaveFile.Instance.SavedItems != null && SaveFile.Instance.SavedItems.Count > 0) {
            SaveFile.Instance.ReloadItems();
        }
        SaveFile.Instance.ReloadAbilities();
    }

    public void AddHistoryEntry(string entry) {
        GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ArchivedLineText")) as GameObject;
        item.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel(entry);
        item.transform.SetParent(Objects.MenuArchive.transform);
        GameController.Instance.WaitAndRunMethodRealtime(0.01f, UpdateHistoryScrollbarPosition);
    }

    public void AddHistoryEntry(string entry, Sprite graphic) {
        GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ArchivedLineGraphic")) as GameObject;
        item.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel(entry);
        item.transform.Find("Image/Graphic").GetComponent<Image>().sprite = graphic;
        item.transform.SetParent(Objects.MenuArchive.transform);
        GameController.Instance.WaitAndRunMethodRealtime(0.01f, UpdateHistoryScrollbarPosition);
        item.transform.localScale = new Vector3(1, 1, 1);
    }

    public void AddHistoryEntry(NotificationController.InGameDialogue dialogue, bool is_choice = false, List<string> string_params = null) {
        foreach(Transform parent in new List<Transform> {GameController.Objects.DialogueArchive.transform, Objects.MenuArchive.transform}) {
            GameObject item = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ArchivedLineDialogue")) as GameObject;
            item.transform.Find("Text").GetComponent<LabelInitializer>().string_params = string_params;
            item.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel((is_choice ? "{DialogueSelectedChoice}" : "") + "{" + dialogue.Id + "}");
            item.transform.SetParent(parent);
            item.transform.localScale = new Vector3(1, 1, 1);
            if(dialogue.SpeakerPortrait == null) {
                item.transform.Find("Portrait").gameObject.SetActive(false);
            }
            else {
                item.transform.Find("Portrait/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/Face Portrait/" + dialogue.SpeakerPortrait, typeof(Sprite)) as Sprite;
                item.transform.Find("Portrait/Title/Text").GetComponent<LabelInitializer>().SetLabel(Label.ContainsKey(dialogue.SpeakerName) ? "{" + dialogue.SpeakerName + "}" : dialogue.SpeakerName);
            }
        }
        GameController.Instance.WaitAndRunMethodRealtime(0.01f, UpdateHistoryScrollbarPosition);
    }

    public void UpdateHistoryScrollbarPosition() {
        Objects.MenuArchive.transform.parent.Find("Scrollbar").GetComponent<Scrollbar>().value = 0;
        Objects.MenuArchive.transform.GetChild(Objects.MenuArchive.transform.childCount - 1).GetComponent<Button>().Select();
        Objects.MenuArchive.transform.GetChild(Objects.MenuArchive.transform.childCount - 1).GetComponent<CenterScrollRectOnItemWhenSelected>().CenterOnItem();
    }

    public void ToggleAllowSkippingUnreadDialogue(bool is_checked) {
        Settings.Instance.AllowSkipUnreadDialogue = is_checked;
    }

    public void ToggleAutoSkipReadDialogue(bool is_checked) {
        Settings.Instance.AutoSkipReadDialogue = is_checked;
    }

    public void ToggleShowExtraInfoInUI(bool is_checked) {
        Settings.Instance.ShowExtraInfoInUI = is_checked;
    }


    public void ScrollToBottomOfHistory() {
        /*GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect").GetComponent<ScrollRect>().verticalScrollbar.SetValueWithoutNotify(0);
        GameController.Instance.transform.Find("Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content").GetComponent<RectTransform>().localPosition = new Vector2(0, 999999);
        GameController.Instance.transform.Find("Archive Window/Dialogue History/Scroll Rect").GetComponent<ScrollRect>().verticalScrollbar.SetValueWithoutNotify(0);
        GameController.Instance.transform.Find("Archive Window/Dialogue History/Scroll Rect/Viewport/Content").GetComponent<RectTransform>().localPosition = new Vector2(0, 999999);*/
    }

    public void Start()
    {
        Tooltip = transform.Find("Tooltip/Text").GetComponent<LabelInitializer>();
        TooltipDisplay =  transform.Find("Tooltip").GetComponent<HideOrShowOverTime>();
        TooltipTransformChange = transform.Find("Tooltip").GetComponent<ChangeTransformOverTime>();
        transform.Find("Options Window/Options/Display/Items/FullScreenMode/Dropdown").GetComponent<TMP_Dropdown>().value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 : (Screen.fullScreenMode == FullScreenMode.Windowed ? 1 : (Screen.fullScreenMode == FullScreenMode.MaximizedWindow ? 2 : 0));
        TMP_Dropdown resolution = transform.Find("Options Window/Options/Display/Items/Resolution/Dropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown.OptionData current_res = resolution.options.FirstOrDefault(item => item.text.Split("x")[0] == Screen.width.ToString() && item.text.Split("x")[1] == Screen.height.ToString());
        if (current_res != null)
        {
            resolution.value = resolution.options.IndexOf(current_res);
        }
        Transform equipment = transform.Find("Inventory Window/Equipment");
        HeavyEquipmentSlot = equipment.Find("Left Panel/Heavy").GetComponent<InventoryTile>();
        LightEquipmentSlot = equipment.Find("Left Panel/Light").GetComponent<InventoryTile>();
        RangedEquipmentSlot = equipment.Find("Left Panel/Ranged").GetComponent<InventoryTile>();
        GlovesEquipmentSlot = equipment.Find("Right Panel/Gloves").GetComponent<InventoryTile>();
        HelmetEquipmentSlot = equipment.Find("Right Panel/Helmet").GetComponent<InventoryTile>();
        OutfitEquipmentSlot = equipment.Find("Right Panel/Outfit").GetComponent<InventoryTile>();
        BootsEquipmentSlot = equipment.Find("Right Panel/Boots").GetComponent<InventoryTile>();
        Item2EquipmentSlot = equipment.Find("Item 2").GetComponent<InventoryTile>();
        Item1EquipmentSlot = equipment.Find("Left Panel/Item 1").GetComponent<InventoryTile>();
        foreach(AbilitySelect select in MenuManager.Instance.GetComponentsInChildren<AbilitySelect>(true)) {
            if(select.IsStanceEquippedAbility) {
                AbilityLoadout.Add(select);
            }
            else {
                AbilityOverview.Add(select);
            }
        }
        foreach(StanceSelect select in MenuManager.Instance.GetComponentsInChildren<StanceSelect>(true)) {
            if(select.IsEquippedStance) {
                StanceLoadout.Add(select);
            }
            else {
                StanceOverview.Add(select);
            }
        }
        foreach(AbilityUnlockTile unlock in MenuManager.Instance.GetComponentsInChildren<AbilityUnlockTile>(true)) {
            AbilityUnlockTiles.Add(unlock);
        }        
        foreach(MasteryUnlockTile unlock in MenuManager.Instance.GetComponentsInChildren<MasteryUnlockTile>(true)) {
            MasteryUnlockTiles.Add(unlock);
        }
        foreach(StanceUnlockTile unlock in MenuManager.Instance.GetComponentsInChildren<StanceUnlockTile>(true)) {
            StanceUnlockTiles.Add(unlock);
        }
        foreach(PassivePowerUpTile unlock in MenuManager.Instance.GetComponentsInChildren<PassivePowerUpTile>(true)) {
            PowerUpTiles.Add(unlock);
        }
        foreach(string submenu in new List<string> {"Character", "Inventory", "Skill Tree", "Archive", "Tutorials", "Options"}) {
            transform.Find(submenu + " Window").gameObject.SetActive(false);
        }
    }

    public void OpenEnergySelection() {
        MenuManager.Instance.transform.Find("Character Window/Energy Select").gameObject.SetActive(true);
        HideAbilitySelection();
        HideStanceSelection();
        MenuManager.Instance.transform.Find("Character Window/Effects").gameObject.SetActive(false);
        EventManager.CancelButtonPressed.AddListener(MenuManager.Instance.HideEnergySelection);
        EventManager.ExitMenu.AddListener(MenuManager.Instance.HideStanceSelection);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            MenuManager.Instance.transform.Find("Character Window/Energy Select/Energies/None/1").GetComponent<Button>().Select();
        }
    }

    public void HideEnergySelection() {
        MenuManager.Instance.transform.Find("Character Window/Energy Select").gameObject.SetActive(false);
    }

    public void ChangeDifficulty() {
        int value = transform.Find("Options Window/Options/Gameplay/Items/Difficulty/Dropdown").GetComponent<Dropdown>().value;
        SaveFile.Instance.Difficulty = value == 0 ? Constants.Difficulty.Story : value == 1 ? Constants.Difficulty.Regular : value == 2 ? Constants.Difficulty.Challenge : Constants.Difficulty.Ultimate;
        transform.Find("Options Window/Options/Gameplay/Items/Difficulty/Dropdown/Label").GetComponent<LabelInitializer>().SetLabel("{" + SaveFile.Instance.Difficulty.ToString() +  "CombatType}");
    }

    public void AddItemToGrid(Item item)
    {
        GameObject item_tile = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_InventoryTile")) as GameObject;
        Transform itemGroup = Objects.InventoryItems.transform.Find(item.Type.ToString() + "/Items");
        item_tile.name = itemGroup.childCount.ToString();
        InventoryTile tile = item_tile.GetComponent<InventoryTile>();
        SetRegularImage(item_tile.transform.Find("Image").gameObject, item);
        tile.Item = item;
        tile.InitializeOptions();
        if(item.GetType() == typeof(Quest_UpgradeMaterials) || item.GetType() == typeof(Quest_ToolMaterials)) {
            tile.AmountDisplay.text = item.Amount.ToString();
        }
        else if(item.Type == Constants.ItemType.Tool)
        {
            tile.AmountDisplay.text = item.Amount.ToString() + "/" + SaveFile.Instance.ToolMaxAmounts[item.GetType()];
        }
        item_tile.transform.SetParent(itemGroup);
        item.TileInInventory = tile;
        item_tile.transform.localScale = new Vector3(1, 1, 1);
    }

    public void UpdateEquippedUsableItems() {
        foreach(InventoryTile tile in new List<InventoryTile> {MenuManager.Instance.Item1EquipmentSlot, MenuManager.Instance.Item2EquipmentSlot}) {
            tile.InitializeOptions();
            if(tile.Item != null && tile.Item.TileInInventory != null) {
                tile.Item.TileInInventory.InitializeOptions();
            }
        }
    }

    public void ClearInventory()
    {
        foreach(Item item in EquippedItems) {
            item.ClearFromInventory();
        }
        SaveFile.Instance.Inventory = new List<Item>();
        foreach(InventoryTile tile in Objects.InventoryEquipment.GetComponentsInChildren<InventoryTile>(true))
        {
            if(tile.Item != null && EquippedItems.Contains(tile.Item))
            {
                tile.UnequipItem();
            }
            if(tile.EquipmentTile) {
                tile.transform.Find("Image").gameObject.SetActive(false);
                tile.transform.Find("Default Image").gameObject.SetActive(true);
                tile.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        foreach (Transform child in Objects.InventoryItems.transform)
        {
            if(child.gameObject.name != "Empty Filler (Gamepad)") {
                Utils.DestroyAllChildren(child.transform.Find("Items"));
            }
        }
    }

    public void SetRegularImage(GameObject image, Item item)
    {
        image.SetActive(true);
        if(item.IconPath != null || item.Icon != null) {
            image.GetComponent<Image>().sprite = item.Icon != null ? item.Icon : Resources.Load("Sprites/" + item.IconPath, typeof(Sprite)) as Sprite;
        }
        else {
            image.GetComponent<SpriteResolver>().SetCategoryAndLabel(item.Type.ToString() + " Icons", item.GetType().ToString().Split('_')[1]);
            image.GetComponent<SpriteResolver>().ResolveSpriteToSpriteRenderer();
            image.GetComponent<Image>().sprite = image.GetComponent<SpriteRenderer>().sprite;
        }
        image.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void ChangeLanguage()
    {
        TMP_Dropdown language = transform.Find("Options Window/Options/Gameplay/Items/Language/Dropdown").GetComponent<TMP_Dropdown>();
        Settings.Instance.CurrentLanguage = language.value == 0 ? Settings.Language.ENG : Settings.Language.PL;
        if(SceneManager.GetActiveScene().name == "StartScreen") {
            Utils.GetSceneRootObject("Start Screen").Find("Sandbox Arena").GetComponent<SandboxArenaController>().Clean();
            Utils.GetSceneRootObject("Start Screen").Find("Sandbox Arena").GetComponent<SandboxArenaController>().Start();
        }
    }

    public void ChangeControls()
    {
        TMP_Dropdown controls = transform.Find("Options Window/Options/Gameplay/Items/Controls/Dropdown").GetComponent<TMP_Dropdown>();
        Settings.Instance.ControlScheme = controls.value == 0 ? "Keyboard" : "Gamepad";
    }

    public void SetInWorldDialogueBubbleSpeed()
    {
        float sliderValue = Objects.OptionsInWorldDialogueSpeedSlider.GetComponent<Slider>().value;
        Settings.Instance.InWorldDialogueBubbleSpeed = sliderValue / 100;
    }

    public void ChangeDisplayedTutorial(string tutorial_name)
    {
        if(Objects.TutorialsWindowDescriptionLabel.transform.parent.gameObject.activeSelf == false)
        {
            Objects.TutorialsWindowDescriptionLabel.transform.parent.gameObject.SetActive(true);
        }
        Objects.TutorialsWindowTitleLabel.SetLabel("{TutorialTitle" + tutorial_name + "}");
        Objects.TutorialsWindowImage.sprite = Resources.Load("Sprites/Tutorial/" + tutorial_name, typeof(Sprite)) as Sprite;
        Objects.TutorialsWindowDescriptionLabel.SetLabel("{TutorialDescription" + tutorial_name + "}");
    }

    public void EndTypingBugReport()
    {
        transform.Find("Report Bug Window/Buttons/UI_Button_1").GetComponent<Button>().Select();
    }

    public void SendBugReport()
    {
        transform.Find("Options Window/Options/ReportBug/Items/UI_Spinner").gameObject.SetActive(true);
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Timeout = 10000;
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Port = 587;
            smtpServer.EnableSsl = true;

            var log_path = CombinePaths(System.Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log");
            mail.From = new MailAddress("darven.games.bugreport@gmail.com", "3LC Bug Report");
            mail.To.Add(new MailAddress("waterfowl.games.supp@gmail.com", "Waterfowl Games Support"));
            mail.Subject = "3LC Bug";
            mail.Body = transform.Find("Options Window/Options/ReportBug/Items/Input/Text").GetComponent<TextMeshProUGUI>().text + "\n\n(" + log_path + "):\n\n" + System.Text.Encoding.UTF8.GetString(GetBytesFromFilePath(log_path));

            smtpServer.Credentials = new NetworkCredential("darven.games.bugreport@gmail.com", "dvkeuwwfuabqxipz");
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            smtpServer.Send(mail);
            transform.Find("Options Window/Options/ReportBug/Items/Description").GetComponent<LabelInitializer>().SetLabel("{DescriptionReportBugSuccess}");
        }
        catch (Exception ex)
        {
            transform.Find("Options Window/Options/ReportBug/Items/Description").GetComponent<LabelInitializer>().SetLabel("{DescriptionReportBugError}" + ex.Message + ", " + ex.StackTrace);
        }
        transform.Find("Options Window/Options/ReportBug/Items/UI_Spinner").gameObject.SetActive(false);
        transform.Find("Options Window/Options/ReportBug/Items/Input").gameObject.SetActive(false);
        transform.Find("Options Window/Options/ReportBug/Items/Buttons/Send Report Button").GetComponent<Button>().interactable = false;
        GameController.Instance.WaitAndRunMethodRealtime(300, UnlockSendReport);
    }

    private byte[] GetBytesFromFilePath(string pathToOutputLog)
    {
        using (var fileStream = File.Open(pathToOutputLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public void UnlockSendReport()
    {
        transform.Find("Options Window/Options/ReportBug/Items/Description").GetComponent<LabelInitializer>().SetLabel("{DescriptionReportBug}");
        transform.Find("Options Window/Options/ReportBug/Items/Input").gameObject.SetActive(true);
        transform.Find("Options Window/Options/ReportBug/Items/Buttons/Send Report Button").GetComponent<Button>().interactable = true;
    }

    public static string CombinePaths(string path1, params string[] paths)
    {
        if (path1 == null)
        {
            throw new ArgumentNullException("path1");
        }
        if (paths == null)
        {
            throw new ArgumentNullException("paths");
        }
        return paths.Aggregate(path1, (acc, p) => Path.Combine(acc, p));
    }

    public void CloseMenu() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
    }

    public void SetFieldOfView() {
        float sliderValue = Objects.OptionsFieldOfViewSlider.GetComponent<Slider>().value;
        Settings.Instance.FieldOfView = sliderValue;
    }

    public void SetUISize() {
        float sliderValue = Objects.OptionsUISizeSlider.GetComponent<Slider>().value;
        Settings.Instance.UISize = sliderValue / 100;
    }

    public void SetDamageNumbersSize() {
        float sliderValue = Objects.OptionsDamageNumbersSizeSlider.GetComponent<Slider>().value;
        Settings.Instance.DamageNumbersSize = sliderValue / 100;
    }

    public void SetMasterVolume() {
        float sliderValue = Objects.OptionsMasterVolumeSlider.GetComponent<Slider>().value;
        Settings.Instance.MasterVolume = sliderValue / 100;
    }

    public void SetDialogueTextSpeed() {
        float sliderValue = Objects.OptionsDialogueTextSpeedSlider.GetComponent<Slider>().value;
        Settings.Instance.DialogueTextSpeed = sliderValue / 100;
    }

    public void SetMusicVolume() {
        float sliderValue = Objects.OptionsMusicVolumeSlider.GetComponent<Slider>().value;
        Settings.Instance.MusicVolume = sliderValue / 100;
    }

    public void SetSoundVolume() {
        float sliderValue = Objects.OptionsSoundVolumeSlider.GetComponent<Slider>().value;
        Settings.Instance.SoundVolume = sliderValue / 100;
    }

    public void SetDialogueVolume() {
        float sliderValue = Objects.OptionsDialogueVolumeSlider.GetComponent<Slider>().value;
        Settings.Instance.DialogueVolume = sliderValue / 100;
    }

    public void SetFullScreenMode()
    {
        
        TMP_Dropdown fullscreen_mode = transform.Find("Options Window/Options/Display/Items/FullScreenMode/Dropdown").GetComponent<TMP_Dropdown>();
        switch (fullscreen_mode.value)
        {
            case 0: { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break; }
            case 1: { Screen.fullScreenMode = FullScreenMode.Windowed; break; }
            case 2: { Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break; }
            default: { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break; }
        }
    }

    public void SetResolution()
    {
        Vector2 resolution = Vector2.zero;
        TMP_Dropdown resolutionDropdown = transform.Find("Options Window/Options/Display/Items/Resolution/Dropdown").GetComponent<TMP_Dropdown>();
        switch (resolutionDropdown.value)
        {
            case 0: { resolution = new Vector2(1280, 720); break; }
            case 1: { resolution = new Vector2(1600, 900); break; }
            case 2: { resolution = new Vector2(1920, 1080); break; }
            case 3: { resolution = new Vector2(2560, 1440); break; }
            default: { resolution = new Vector2(1920, 1080); break; }
        }
        Screen.SetResolution((int)resolution.x, (int)resolution.y, Screen.fullScreenMode);
    }

    public void ShowItemDetails(Item item)
    {
        if(CurrentDetailedItemDescription == item || item == null)
        {
            return;
        }
        CurrentDetailedItemDescription = item;
        InventoryDetailsWindowOpen = true;
        transform.Find("Inventory Window/Details").gameObject.SetActive(true);
        transform.Find("Inventory Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(item.Type == Constants.ItemType.Quest ? " " : item.GetDescription() + GetModifierDescriptions(item));
        transform.Find("Inventory Window/Details/Flavor Text/Image/Description").GetComponent<LabelInitializer>().SetLabel(item.GetFlavorText());
        transform.Find("Inventory Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + item.GetType().ToString() + "}");
        if(item.Type == Constants.ItemType.Heavy || item.Type == Constants.ItemType.Light || item.Type == Constants.ItemType.Ranged)
        {
            transform.Find("Inventory Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{ItemGrade_" + item.Grade.ToString() + "_Colored} {ItemClass_" + item.WeaponClass.ToString() + "}");
        }
        else
        {
            transform.Find("Inventory Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{ItemGrade_" + item.Grade.ToString() + "_Colored} {ItemType_" + item.Type.ToString() + "}");
        }
        SetRegularImage(transform.Find("Inventory Window/Details/Image/Image").gameObject, item);
    }

    public void ShowShopItemDetails(Item item) {
        if(CurrentDetailedShopItemDescription == item || item == null)
        {
            return;
        }
        CurrentDetailedShopItemDescription = item;
        ShopDetailsWindowOpen = true;
        GameController.Instance.transform.Find("Shop/Details").gameObject.SetActive(true);
        GameController.Instance.transform.Find("Shop/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(item.Type == Constants.ItemType.Quest ? " " : item.GetDescription() + GetModifierDescriptions(item));
        GameController.Instance.transform.Find("Shop/Details/Flavor Text/Image/Description").GetComponent<LabelInitializer>().SetLabel(item.GetFlavorText());
        GameController.Instance.transform.Find("Shop/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + item.GetType().ToString() + "}");
        if(item.Type == Constants.ItemType.Heavy || item.Type == Constants.ItemType.Light || item.Type == Constants.ItemType.Ranged)
        {
            GameController.Instance.transform.Find("Shop/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{ItemGrade_" + item.Grade.ToString() + "_Colored} {ItemClass_" + item.WeaponClass.ToString() + "}");
        }
        else
        {
            GameController.Instance.transform.Find("Shop/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{ItemGrade_" + item.Grade.ToString() + "_Colored} {ItemType_" + item.Type.ToString() + "}");
        }
        SetRegularImage(GameController.Instance.transform.Find("Shop/Details/Image/Image").gameObject, item);
    }

    public List<string> AbilityUpgradeStringParams;

    public void ShowAbilityDetails(Type ability_type)
    {
        if(CurrentAbilityDescription == ability_type || ability_type == null)
        {
            return;
        }
        CurrentAbilityDescription = ability_type;
        AbilityDetailsWindowOpen = true;
        transform.Find("Character Window/Details").gameObject.SetActive(true);
        bool is_stance = ability_type.ToString().Contains("Stance_");
        transform.Find("Character Window/Details/Image").gameObject.SetActive(!is_stance);
        transform.Find("Character Window/Details/Stance").gameObject.SetActive(is_stance);
        transform.Find("Character Window/Details/Energy").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Stat").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Description/Image/Energy Description").gameObject.SetActive(false);
        FieldInfo family = ability_type.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        if(is_stance) {
            transform.Find("Character Window/Details/Stance/Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + ability_type.ToString(), typeof(Sprite)) as Sprite;
            transform.Find("Character Window/Details/Stance/Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
            transform.Find("Character Window/Details/Stance/Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
            transform.Find("Character Window/Details/Stance").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        }
        else {
            transform.Find("Character Window/Details/Image/Icon").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(ability_type != null ? ability_type.ToString() : null);
        }
        MethodInfo desc = ability_type.GetMethod("GetDescriptionValues", BindingFlags.Public | BindingFlags.Static);
        if (desc != null)
        {
            transform.Find("Character Window/Details/Description/Image/Description").gameObject.SetActive(true);
            transform.Find("Character Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().string_params = Utils.RoundAllNumbers((List<string>)desc.Invoke(null, null));
        }
        if(ability_type.ToString().Contains("Stance_")) {
            string stance_desc = "{" + ability_type.ToString() + "_Description}\n";
            if(SaveFile.Instance.StanceUpgrades.Contains(ability_type.ToString().Replace("Stance_", "") + "1")) {
                stance_desc += "\n{StanceUpgrade1}";
            }
            if(SaveFile.Instance.StanceUpgrades.Contains(ability_type.ToString().Replace("Stance_", "") + "2")) {
                stance_desc += "\n{StanceUpgrade2}";
            }
            if(SaveFile.Instance.StanceUpgrades.Contains(ability_type.ToString().Replace("Stance_", "") + "3")) {
                stance_desc += "\n{StanceUpgrade3}";
            }
            transform.Find("Character Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel(stance_desc);
        }
        else {
            transform.Find("Character Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel("{" + ability_type.ToString() + "_Description}");
        }
        transform.Find("Character Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + ability_type.ToString() + "}");
        FieldInfo isUltimate = ability_type.GetField("IsUltimate", BindingFlags.Public | BindingFlags.Static);
        transform.Find("Character Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{TechniqueFamily_" + family.GetValue(null) + "_Colored} " + (isUltimate != null ? "{UltimateTechnique} " : "") + (ability_type.ToString().Contains("Stance_") ? "{Stance}" : "{Technique}") + (SaveFile.Instance.AbilitiesMasteryA.Contains(ability_type) ? " ({TechniqueUpgradeA})" : SaveFile.Instance.AbilitiesMasteryB.Contains(ability_type) ? " ({TechniqueUpgradeB})" : ""));
    }

    public void ShowStatDetails(string stat) {
        transform.Find("Character Window/Details").gameObject.SetActive(true);
        transform.Find("Character Window/Details/Image").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Stance").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Energy").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Stat").gameObject.SetActive(true);
        transform.Find("Character Window/Details/Description/Image/Energy Description").gameObject.SetActive(false);
        transform.Find("Character Window/Details/Description/Image/Description").gameObject.SetActive(true);
        transform.Find("Character Window/Details/Stat").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + stat, typeof(Sprite)) as Sprite;
        transform.Find("Character Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel("{Stat_" + stat + "_Description}");
        transform.Find("Character Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{Stat_" + stat + "}");
        transform.Find("Character Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{MenuStatLabel_Stat}");
    }

    public void ShowPowerUpDetails(PassivePowerUpTile tile, bool refresh = false, bool show_detailed = false) {
        if(tile == null || tile.GetComponent<Button>().interactable == false || (CurrentSkillTreeTileDescription == tile.gameObject && refresh == false))
        {
            return;
        }
        CurrentSkillTreeTileDescription = tile.gameObject;
        SkillTreeTileDetailsWindowOpen = true;
        transform.Find("Skill Tree Window/Details").gameObject.SetActive(true);
        string desc = "";
        string[] power_up_names = tile.PowerUp.Split("+");
        List<string> string_params = new List<string>();
        List<Effect> power_ups = new List<Effect>();
        for(int i = 0; i < power_up_names.Length; i++) {
            List<Effect> effects = EffectList.GetEffect(power_up_names[i], tile.GetPowerBudgetForTile(i));
            string_params.AddRange(effects[0].DescriptionParameters);
            power_ups.AddRange(effects);
            desc += "{Effect_" + power_up_names[i] + "_Description" + ((show_detailed && Label.ContainsKey("{Effect_" + power_up_names[i] + "_DescriptionDetailed")) ? "Detailed" : "") + "}\n\n";
            if(SaveFile.Instance.UnlockedPowerUps.Contains(tile.Id) == false && effects[0].ShowCalculatedStatIncreasesBasedOnFirstStringParam != null) {
                foreach(Stat stat in effects[0].ShowCalculatedStatIncreasesBasedOnFirstStringParam) {
                    desc += 
                        "<link=\"Stat_" + stat.ToString() + "_Description\"><sprite name=\"" + stat.ToString() + "\"></link>" + stat.GetCalculatedIncrease( effects[0].FlatAmount != 0 ? 0 : float.Parse(effects[0].DescriptionParameters[0]), effects[0].FlatAmount != 0 ? float.Parse(effects[0].DescriptionParameters[0]) : 0) + "\n";
                }
                desc += "\n";
            }
        }
        transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").GetComponent<LabelInitializer>().string_params = string_params;
        transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").GetComponent<LabelInitializer>().SetLabelWithIncrementedToken(desc, string_params);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<TextMeshProUGUI>().text = "";
        transform.Find("Skill Tree Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{Effect_" + tile.PowerUp + "}");
        transform.Find("Skill Tree Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{PowerUpName}");
        transform.Find("Skill Tree Window/Details/Ability").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Stance").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Power-up").gameObject.SetActive(true);
        if(tile.transform.Find("Icon_1") != null || tile.transform.Find("Icon (1)") != null) {
            bool ver1 = tile.transform.Find("Icon_1") != null;
            transform.Find("Skill Tree Window/Details/Power-up/Icon2").gameObject.SetActive(true);
            RectTransform tile2RectTransform = tile.transform.Find(ver1 ? "Icon_1" : "Icon (1)").GetComponent<RectTransform>();
            transform.Find("Skill Tree Window/Details/Power-up/Icon2").GetComponent<Image>().sprite = tile.transform.Find(ver1 ? "Icon_1" : "Icon (1)").GetComponent<Image>().sprite;
            transform.Find("Skill Tree Window/Details/Power-up/Icon2").GetComponent<RectTransform>().sizeDelta = new Vector2(tile2RectTransform.sizeDelta.x, tile2RectTransform.sizeDelta.y);
            transform.Find("Skill Tree Window/Details/Power-up/Icon2").GetComponent<RectTransform>().localPosition = tile2RectTransform.localPosition;
        }
        else {
            transform.Find("Skill Tree Window/Details/Power-up/Icon2").gameObject.SetActive(false);
        }
        transform.Find("Skill Tree Window/Details/Power-up/Icon").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Power-up/Icon").GetComponent<Image>().sprite = tile.transform.Find("Icon").GetComponent<Image>().sprite;
        transform.Find("Skill Tree Window/Details/Power-up/Icon").GetComponent<Image>().color = tile.transform.Find("Icon").GetComponent<Image>().color;
        RectTransform tileRectTransform = tile.transform.Find("Icon").GetComponent<RectTransform>();
        transform.Find("Skill Tree Window/Details/Power-up/Icon").GetComponent<RectTransform>().sizeDelta = new Vector2(tileRectTransform.sizeDelta.x, tileRectTransform.sizeDelta.y);
        transform.Find("Skill Tree Window/Details/Power-up/Icon").GetComponent<RectTransform>().localPosition = tileRectTransform.localPosition;
    }

    public void ShowSkillTreeAbilityDetails(AbilityUnlockTile tile, bool refresh = false, bool is_ultimate = false) {
        if(tile == null || (CurrentSkillTreeTileDescription == tile?.gameObject && refresh == false) || tile.AbilityType == null || tile.AbilityType.GetMethod("GetDescriptionValues", BindingFlags.Public | BindingFlags.Static) == null)
        {
            return;
        }
        CurrentSkillTreeTileDescription = tile.gameObject;
        SkillTreeTileDetailsWindowOpen = true;
        transform.Find("Skill Tree Window/Details").gameObject.SetActive(true);
        MethodInfo desc = tile.AbilityType.GetMethod(is_ultimate ? "GetUltimateDescriptionValues" : "GetDescriptionValues", BindingFlags.Public | BindingFlags.Static);
        if (desc != null)
        {
            transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().string_params = Utils.RoundAllNumbers((List<string>)desc.Invoke(null, null));
        }
        transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + "_FlavorText}");
        transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + (is_ultimate ? "_Ultimate" : "") + "_Description}");
        transform.Find("Skill Tree Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + "}");
        FieldInfo family = tile.AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        FieldInfo isUltimate = tile.AbilityType.GetField("IsUltimate", BindingFlags.Public | BindingFlags.Static);
        transform.Find("Skill Tree Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{TechniqueFamily_" + family.GetValue(null) + "_Colored} " + (isUltimate != null ? "{UltimateTechnique} " : "") + "{Technique}" + (SaveFile.Instance.AbilitiesMasteryA.Contains(tile.AbilityType) ? " ({TechniqueUpgradeA})" : SaveFile.Instance.AbilitiesMasteryB.Contains(tile.AbilityType) ? " ({TechniqueUpgradeB})" : ""));
        transform.Find("Skill Tree Window/Details/Ability").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Stance").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Power-up").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Ability/Icon").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(tile.AbilityType != null ? tile.AbilityType.ToString() : null);
    }

    public void ShowSkillTreeStanceDetails(StanceUnlockTile tile, bool refresh = false) {
        if(tile == null || (CurrentSkillTreeTileDescription == tile.gameObject && refresh == false))
        {
            return;
        }
        FieldInfo family = tile.AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        CurrentSkillTreeTileDescription = tile.gameObject;
        SkillTreeTileDetailsWindowOpen = true;
        transform.Find("Skill Tree Window/Details").gameObject.SetActive(true);
        MethodInfo desc = tile.AbilityType.GetMethod(tile.IsUpgrade ? "GetDescriptionUpgrade" + tile.Ability[tile.Ability.Length - 1]  + "Values" : "GetDescriptionValues", BindingFlags.Public | BindingFlags.Static);
        if (desc != null)
        {
            transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().string_params = Utils.RoundAllNumbers((List<string>)desc.Invoke(null, null));
        }
        transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + (tile.IsUpgrade ? "_Upgrade" + tile.Ability[tile.Ability.Length - 1] : "") + "_Description}");
        transform.Find("Skill Tree Window/Details/Description/Image/Description").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Description/Image/Description (No Flavor)").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<TextMeshProUGUI>().text = "";
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<LabelInitializer>().OriginalValue = "";
        transform.Find("Skill Tree Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + "}");
        transform.Find("Skill Tree Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel("{TechniqueFamily_" + family.GetValue(null) + "_Colored} " + (tile.IsUpgrade ? ("{StanceUpgrade" + tile.Ability[tile.Ability.Length - 1] + "}") : "{Stance}"));
        transform.Find("Skill Tree Window/Details/Ability").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Stance").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Power-up").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Stance").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        transform.Find("Skill Tree Window/Details/Stance/Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        transform.Find("Skill Tree Window/Details/Stance/Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family.GetValue(null).ToString());
        transform.Find("Skill Tree Window/Details/Stance/Graphic").gameObject.SetActive(tile.IsUpgrade);
        if(tile.IsUpgrade) {
            transform.Find("Skill Tree Window/Details/Stance/Graphic").GetComponent<Image>().sprite = tile.transform.Find("Graphic").GetComponent<Image>().sprite;
        }
        transform.Find("Skill Tree Window/Details/Stance/Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + tile.AbilityType.ToString(), typeof(Sprite)) as Sprite;
    }

    public void OpenEffectsOverview() {
        Objects.CharacterEffects.gameObject.SetActive(true);
        Utils.DestroyAllChildren(Objects.CharacterEffectList.transform);
        foreach(Effect effect in Player.Instance.CurrentEffects.Where(effect => effect.ShowsInMenu).ToList()) {
            GameObject effectDescription = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MenuOverviewEffectDescription")) as GameObject;
            effectDescription.transform.SetParent(Objects.CharacterEffectList.transform);
            effectDescription.transform.Find("Background/Text/Grid/Image").GetComponent<Image>().sprite = effect.UIGraphic != null ? effect.UIGraphic : !String.IsNullOrWhiteSpace(effect.PathToUIGraphic) ? Resources.Load("Sprites/" + (effect.PathToUIGraphic), typeof(Sprite)) as Sprite : Resources.Load("Sprites/UI/Danger Sign", typeof(Sprite)) as Sprite;
            if(effect.BaseDuration == 0) {
                effectDescription.transform.Find("Background/Text/Grid/Duration").gameObject.SetActive(false);
            }
            else {
                effectDescription.transform.Find("Background/Text/Grid/Duration").GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedFloat(effect.RemainingDuration, 1);
            }
            effectDescription.transform.Find("Background/" + effect.Type).gameObject.SetActive(true);
            effectDescription.transform.Find("Background/Text").GetComponent<TextMeshProUGUI>().text = Label.Get(effect.GetType().ToString() + "_Description");
        }
    }

    public void ShowSkillTreeMasteryDetails(MasteryUnlockTile tile, bool refresh = false) {
        if(tile == null || (CurrentSkillTreeTileDescription == tile.gameObject && refresh == false) || tile.AbilityType == null || tile.AbilityType.GetMethod("GetMastery" + tile.Mastery + "DescriptionValues", BindingFlags.Public | BindingFlags.Static) == null)
        {
            return;
        }
        CurrentSkillTreeTileDescription = tile.gameObject;
        SkillTreeTileDetailsWindowOpen = true;
        transform.Find("Skill Tree Window/Details").gameObject.SetActive(true);
        MethodInfo desc = tile.AbilityType.GetMethod("GetMastery" + tile.Mastery + "DescriptionValues", BindingFlags.Public | BindingFlags.Static);
        if (desc != null)
        {
            transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().string_params = Utils.RoundAllNumbers((List<string>)desc.Invoke(null, null));
        }
        transform.Find("Skill Tree Window/Details/Description/Image/Description").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + "_Mastery" + tile.Mastery + "_Description}");
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<LabelInitializer>().OriginalValue = "";
        transform.Find("Skill Tree Window/Details/Description/Image/Flavor Text").GetComponent<TextMeshProUGUI>().text = "";
        transform.Find("Skill Tree Window/Details/Name/Text").GetComponent<LabelInitializer>().SetLabel("{" + tile.AbilityType.ToString() + "}");
        FieldInfo family = tile.AbilityType.GetField("Family", BindingFlags.Public | BindingFlags.Static);
        FieldInfo isUltimate = tile.AbilityType.GetField("IsUltimate", BindingFlags.Public | BindingFlags.Static);
        transform.Find("Skill Tree Window/Details/Category/Text").GetComponent<LabelInitializer>().SetLabel(tile.Mastery == "A" ? "{TechniqueUpgradeAFull}" : "{TechniqueUpgradeBFull}");
        transform.Find("Skill Tree Window/Details/Ability").gameObject.SetActive(true);
        transform.Find("Skill Tree Window/Details/Stance").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Power-up").gameObject.SetActive(false);
        transform.Find("Skill Tree Window/Details/Ability/Icon").GetComponent<Image>().sprite = Utils.GetGraphicForAbility(tile.AbilityType != null ? tile.AbilityType.ToString() : null);
    }

    public string GetModifierDescriptions(Item item, bool detailed = false)
    {
        if(item.Type == Constants.ItemType.Tool || item.Type == Constants.ItemType.Quest) {
            return "";
        }    
        string desc = "";
        foreach (Item.ItemEffect itemEffect in item.FirstItemEffects)
        {  
            List<Effect> ef = EffectList.GetEffect(itemEffect.EffectName, itemEffect.PortionOfPowerBudget * item.GetItemFirstEffectPB());
            desc += string.Format((detailed && Label.ContainsKey("Effect_" + itemEffect.EffectName + "_DescriptionDetailed")) ? Label.Get("Effect_" + itemEffect.EffectName + "_DescriptionDetailed") + "\n\n" : Label.Get("Effect_" + itemEffect.EffectName + "_Description") + (Label.ContainsKey("Effect_" + itemEffect.EffectName + "_DescriptionDetailed") ? " <link=\"FirstItemEffects\"><sprite name=\"Detailed\"></link>" : "") + "\n\n", ef[0].DescriptionParameters.ToArray());
            foreach(Stat stat in ef[0].ShowCalculatedStatIncreasesBasedOnFirstStringParam) {
                desc += "<link=\"Stat_" + stat.ToString() + "_Description\"><sprite name=\"" + stat.ToString() + "\"></link>" + stat.GetCalculatedIncrease(ef[0].FlatAmount != 0 ? 0 : float.Parse(ef[0].DescriptionParameters[0]), ef[0].FlatAmount != 0 ? float.Parse(ef[0].DescriptionParameters[0]) : 0) + "\n";
            }
            desc += "\n";
        }
        if(item.GetItemSecondEffectPB() == 0) {
            return desc;
        }
        foreach (Item.ItemEffect itemEffect in item.SecondItemEffects)
        {  
            List<Effect> ef = EffectList.GetEffect(itemEffect.EffectName, itemEffect.PortionOfPowerBudget * item.GetItemSecondEffectPB());
            desc += string.Format((detailed && Label.ContainsKey("Effect_" + itemEffect.EffectName + "_DescriptionDetailed")) ? Label.Get("Effect_" + itemEffect.EffectName + "_DescriptionDetailed") + "\n\n" : Label.Get("Effect_" + itemEffect.EffectName + "_Description") + (Label.ContainsKey("Effect_" + itemEffect.EffectName + "_DescriptionDetailed") ? " <link=\"SecondItemEffects\"><sprite name=\"Detailed\"></link>" : "") + "\n\n", ef[0].DescriptionParameters.ToArray());
            foreach(Stat stat in ef[0].ShowCalculatedStatIncreasesBasedOnFirstStringParam) {
                desc += "<link=\"Stat_" + stat.ToString() + "_Description\"><sprite name=\"" + stat.ToString() + "\"></link>" + stat.GetCalculatedIncrease(ef[0].FlatAmount != 0 ? 0 : float.Parse(ef[0].DescriptionParameters[0]), ef[0].FlatAmount != 0 ? float.Parse(ef[0].DescriptionParameters[0]) : 0) + "\n";
            }
        }
        return desc;
    }

    public void HideItemDetailsWindow()
    {
        CurrentDetailedItemDescription = null;
        InventoryDetailsWindowOpen = false;
        transform.Find("Inventory Window/Details").gameObject.SetActive(false);
    }

    public void ChangeSelectedInventoryCategory(int sort)
    {
        if(SelectedSorting != null)
        {
            SelectedSorting.transform.Find("Background/Checkmark (Selected)").gameObject.SetActive(false);
            SelectedSorting.transform.Find("Background/Checkmark").gameObject.SetActive(true);
        }
        Transform items = transform.Find("Inventory Window/Inventory/Viewport/Items");
        for (int i = 0; i < items.childCount; i++)
        {
            if(items.GetChild(i).gameObject.name.Contains("Set_")) {
                items.GetChild(i).gameObject.SetActive(false);
            }
            else {
                items.GetChild(i).gameObject.SetActive(sort == 0);
            }
        }
        foreach(InventoryTile item in items.GetComponentsInChildren<InventoryTile>(true)) {
            if(sort != 1 || item.Item.Type == Constants.ItemType.Tool || item.Item.Type == Constants.ItemType.Quest) {
                item.transform.SetParent(items.Find(item.Item.Type.ToString()+"/Items"));
            }
            else {
                item.transform.SetParent(items.Find("Set_" + item.Item.Set.ToString()+"/Items"));
            }
        }
        if(sort != 0 && sort != 1)
        {
            transform.Find("Inventory Window/Inventory/Viewport/Items").GetChild(sort - 2).gameObject.SetActive(true);
        }
        else if(sort == 1) {
            foreach(Transform child in items) {
                if(child.gameObject.name.Contains("Set_") && child.Find("Items").childCount > 0) {
                    child.gameObject.SetActive(true);
                }
            }
        }
        SelectedSorting = transform.Find("Inventory Window/Inventory/Category Selection/UI_InventorySortTile_" + sort.ToString()).gameObject;
        SelectedSorting.transform.Find("Background/Checkmark (Selected)").gameObject.SetActive(true);
        SelectedSorting.transform.Find("Background/Checkmark").gameObject.SetActive(false);
        SelectedSortingIndex = sort;
        if(Settings.Instance.ControlScheme == "Gamepad" && items.GetChild(sort == 0 ? 0 : sort - 2).Find("Items").childCount > 0)
        {
            InventoryTile tile = items.GetChild(sort == 0 ? 0 : sort - 2).Find("Items").GetChild(0).GetComponent<InventoryTile>();
            if (CurrentlySelectedTile != null && CurrentlySelectedTile != tile)
            {
                CurrentlySelectedTile.OnDeselect(null);
            }
            if(CurrentlySelectedTile != tile)
            {
                CurrentlySelectedTile = tile;
                CurrentlySelectedTile.GetComponent<Button>().Select();
            }
        }
        items.position = new Vector2(items.position.x, 0);
        foreach(Transform child in items) {
            if(child.gameObject.activeSelf) {
                List<Item> consideredItems = new();
                List<Item> weaponItems = new();
                List<Item> nonWeaponItems = new();
                foreach(InventoryTile tile in child.GetComponentsInChildren<InventoryTile>()) {
                    consideredItems.Add(tile.Item);
                    if(sort == 1 && (tile.Item.Type == Constants.ItemType.Heavy || tile.Item.Type == Constants.ItemType.Light || tile.Item.Type == Constants.ItemType.Ranged)) {
                        weaponItems.Add(tile.Item);
                    }
                    else if(sort == 1){
                        nonWeaponItems.Add(tile.Item);
                    }
                }
                List<Item> sortedItems;
                if(sort == 1) {
                    weaponItems = weaponItems.OrderBy(item => item.Type).ThenBy(item => item.GetType().ToString()).ThenByDescending(item => item.Grade).ToList();
                    nonWeaponItems = nonWeaponItems.OrderBy(item => item.Type).ThenBy(item => item.GetType().ToString()).ThenByDescending(item => item.Grade).ToList();
                    sortedItems = nonWeaponItems.Concat(weaponItems).ToList();
                }
                else {
                    sortedItems = consideredItems.OrderBy(item => item.Type).ThenBy(item => item.Set).ThenByDescending(item => item.Grade).ToList();
                } 
                for(int i = 0; i < sortedItems.Count; i++) {
                    sortedItems[i].TileInInventory.transform.SetSiblingIndex(i);
                }
            }
        }
    }

    public void ChangeDisplayedSkillTree(int sort) {
        SelectedSkillTree = sort;
        for(int i = 0; i < 7; i++) {
            transform.Find("Skill Tree Window/Skill Tree").GetChild(i).gameObject.SetActive(i == sort);
            transform.Find("Skill Tree Window/Category Selection").GetChild(i + 2).transform.Find("Background/Checkmark (Selected)").gameObject.SetActive(sort == i);
        }
    }

    public class ModifierDescription
    {
        public List<string> Params;
        public string Label;

        public ModifierDescription(string label, List<string> string_params)
        {
            Label = label;
            Params = string_params;
        }
    }

    public void EquipAbility(Type ability) {
        string[] stanceAndSlot = AbilityBeingChanged.Split("-");
        Stance stance = SaveFile.Instance.Stances[stanceAndSlot[0] == "Heavy" ? 0 : stanceAndSlot[0] == "Light" ? 1 : 2];
        HandleAbilityEquipInvokes(stance, ability, stanceAndSlot);
        AbilityBeingChanged = null;
        transform.Find("Character Window/Abilities/Stances/" + stanceAndSlot[0] + "/" + int.Parse(stanceAndSlot[1])).GetComponent<AbilitySelect>().AbilityType = ability;
        transform.Find("Character Window/Abilities/Stances/" + stanceAndSlot[0] + "/" + int.Parse(stanceAndSlot[1])).GetComponent<AbilitySelect>().Ability = ability.ToString();
        foreach(AbilitySelect abilitySelect in MenuManager.Instance.AbilityLoadout) {
            abilitySelect.UpdateUnlockedStatus();
        }
        Player.Instance.UpdateTechniqueStacksAmount(ability, 1);
        HideAbilitySelection();
        UpdateLoadoutUpgradePoints();
    }

    private void HandleAbilityEquipInvokes(Stance stance, Type ability, string[] stanceAndSlot) {
        int unequipedAbilityCount = 0;
        foreach(Stance.EquippedAbility equippedAbility in SaveFile.Instance.Stances[0].Abilities.Concat(SaveFile.Instance.Stances[1].Abilities).Concat(SaveFile.Instance.Stances[2].Abilities)) {
            if(equippedAbility.Type == stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type) {
                unequipedAbilityCount++;
            }
        }
        if(unequipedAbilityCount == 1 && stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type != null) {
            MethodInfo methodInfo = stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type.GetMethod("OnUnequip", BindingFlags.Public | BindingFlags.Static);
            if(methodInfo != null) {
                methodInfo.Invoke(null, null);
            }
        }
        stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type = ability;
        int equipedAbilityCount = 0;
        foreach(Stance.EquippedAbility equippedAbility in SaveFile.Instance.Stances[0].Abilities.Concat(SaveFile.Instance.Stances[1].Abilities).Concat(SaveFile.Instance.Stances[2].Abilities)) {
            if(equippedAbility.Type == ability) {
                equipedAbilityCount++;
            }
        }
        if(equipedAbilityCount == 1 && stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type != null) {
            MethodInfo methodInfo2 = stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type.GetMethod("OnEquip", BindingFlags.Public | BindingFlags.Static);
            if(methodInfo2 != null) {
                methodInfo2.Invoke(null, null);
            }
        }
    }

    public void EquipStance(Type ability) {
        string family = ability.GetField("Family",  BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();
        SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect.OnStanceUnequipped();
        if(SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)] == Player.Instance.CurrentStance) {
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect.OnStanceDeactivated();
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect = (Effect_Stance)Activator.CreateInstance(ability, new object[] { null });
            if(Player.Instance.CurrentStance == SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)]) {
                Player.Instance.ReplaceStanceDisplay();
            }
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect.OnStanceActivated();
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect.OnStanceEquipped();
        }
        else {
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect = (Effect_Stance)Activator.CreateInstance(ability, new object[] { null });
            SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].StanceEffect.OnStanceEquipped();
        }
        foreach(Image img in new List<Image> {transform.Find("Character Window/Abilities/Stances").GetChild(int.Parse(StanceBeingChanged)).Find("UI_StanceTile").GetComponent<Image>(), SaveFile.Instance.Stances[int.Parse(StanceBeingChanged)].UIStanceDisplay.GetComponent<Image>()}) {
            if(img.GetComponent<Button>() != null) {
                img.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + ability.ToString(), typeof(Sprite)) as Sprite;
                img.GetComponent<Image>().color = ability == typeof(Stance_None) ? Color.grey : Colors.GetFamilyColor(family);
            }
            else {
                img.transform.Find("Stance Border").GetComponent<Image>().sprite = Resources.Load("Sprites/Stance/" + ability.ToString(), typeof(Sprite)) as Sprite;
                img.color = Colors.GetFamilyColor(family);
            }
            img.transform.Find("Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family);
            img.transform.Find("Border Upper").GetComponent<Image>().color = Colors.GetFamilyColor(family);
            img.transform.Find("Border Lower").GetComponent<Image>().color = Colors.GetFamilyColor(family);
        }
        transform.Find("Character Window/Abilities/Stances").GetChild(int.Parse(StanceBeingChanged)).Find("UI_StanceTile").GetComponent<StanceSelect>().StanceType = ability;
        transform.Find("Character Window/Abilities/Stances").GetChild(int.Parse(StanceBeingChanged)).Find("UI_StanceTile").GetComponent<StanceSelect>().Stance = ability.ToString();
        HideStanceSelection();
        foreach(StanceSelect stanceSelect in MenuManager.Instance.StanceLoadout) {
            stanceSelect.UpdateUnlockedStatus();
        }
        UpdateLoadoutUpgradePoints();
    }
    public List<string> ResolvedUpgrades;

    public void UpdateLoadoutUpgradePoints() {
        ResolvedUpgrades = new();
        int spentPoints = 0;
        SaveFile.Instance.ActiveUpgrades.Clear();
        foreach(StanceSelect stanceSelect in StanceLoadout) {
            for(int i = 1; i <= 3; i++) {
                if(ResolvedUpgrades.Contains(stanceSelect.Stance + i)) {
                    stanceSelect.transform.Find("UI_UpgradesLoadout/" + i).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.ActiveUpgrades.Contains(stanceSelect.Stance + i) ? "UpgradeUnlockedButDuplicate" : "UpgradeNotActive"), typeof(Sprite)) as Sprite;
                }
                else if(SaveFile.Instance.StanceUpgrades.Contains(stanceSelect.Stance + i)){
                    spentPoints++;
                    if(spentPoints <= SaveFile.Instance.MaxUpgradePoints) {
                        SaveFile.Instance.ActiveUpgrades.Add(stanceSelect.Stance + i);
                    }
                    else {
                        stanceSelect.transform.Find("UI_UpgradesLoadout/" + i).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/UpgradeNotActive", typeof(Sprite)) as Sprite;
                    }
                    ResolvedUpgrades.Add(stanceSelect.Stance + i);
                }
            }
        }
        for(int i = 1; i <= 4; i++) {
            for(int j = 0; j < 3; j++) {
                AbilitySelect abilitySelect = MenuManager.Instance.transform.Find("Character Window/Abilities/Stances/" + (j == 0 ? "Heavy" : j == 1 ? "Light" : "Ranged") + "/" + i).GetComponent<AbilitySelect>();
                if(abilitySelect.AbilityType == null) {
                    continue;
                }
                if(ResolvedUpgrades.Contains(abilitySelect.AbilityType.ToString() + "_UpgradeA")) {
                    abilitySelect.transform.Find("UI_UpgradesLoadout/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.ActiveUpgrades.Contains(abilitySelect.AbilityType.ToString() + "_UpgradeA") ? "UpgradeUnlockedButDuplicate" : "UpgradeNotActive"), typeof(Sprite)) as Sprite;
                }
                else if(SaveFile.Instance.AbilitiesMasteryA.Contains(abilitySelect.AbilityType)){
                    spentPoints++;
                    if(spentPoints <= SaveFile.Instance.MaxUpgradePoints) {
                        SaveFile.Instance.ActiveUpgrades.Add(abilitySelect.AbilityType.ToString() + "_UpgradeA");
                    }
                    else {
                        abilitySelect.transform.Find("UI_UpgradesLoadout/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/UpgradeNotActive", typeof(Sprite)) as Sprite;
                    }
                    ResolvedUpgrades.Add(abilitySelect.AbilityType.ToString() + "_UpgradeA");
                }
                if(ResolvedUpgrades.Contains(abilitySelect.AbilityType.ToString() + "_UpgradeB")) {
                    abilitySelect.transform.Find("UI_UpgradesLoadout/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + (SaveFile.Instance.ActiveUpgrades.Contains(abilitySelect.AbilityType.ToString() + "_UpgradeB") ? "UpgradeUnlockedButDuplicate" : "UpgradeNotActive"), typeof(Sprite)) as Sprite;
                }
                else if(SaveFile.Instance.AbilitiesMasteryB.Contains(abilitySelect.AbilityType)){
                    spentPoints++;
                    if(spentPoints <= SaveFile.Instance.MaxUpgradePoints) {
                        SaveFile.Instance.ActiveUpgrades.Add(abilitySelect.AbilityType.ToString() + "_UpgradeB");
                    }
                    else {
                        abilitySelect.transform.Find("UI_UpgradesLoadout/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/UpgradeNotActive", typeof(Sprite)) as Sprite;
                    }
                    ResolvedUpgrades.Add(abilitySelect.AbilityType.ToString() + "_UpgradeB");
                }
            }
        }
        int counter = 1;
        int usedPoints = spentPoints > SaveFile.Instance.MaxUpgradePoints ? SaveFile.Instance.MaxUpgradePoints : spentPoints;
        int unusedPoints = SaveFile.Instance.MaxUpgradePoints - spentPoints < 0 ? 0 : SaveFile.Instance.MaxUpgradePoints - spentPoints;
        int invalidPoints = spentPoints - SaveFile.Instance.MaxUpgradePoints < 0 ? 0 : spentPoints - SaveFile.Instance.MaxUpgradePoints;
        for(int i = 0; i < usedPoints; i++, counter++) {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + "UpgradeUnlocked", typeof(Sprite)) as Sprite;
        }
        for(int i = 0; i < unusedPoints; i++, counter++) {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + "UpgradeNotUnlocked", typeof(Sprite)) as Sprite;
        }
        for(int i = 0; i < invalidPoints; i++, counter++) {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).gameObject.SetActive(true);
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + "UpgradeNotActive", typeof(Sprite)) as Sprite;
        }
        for(; counter < 34; counter++) {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Right-side Panel/AvailableUpgradePoints/" + counter).gameObject.SetActive(false);
        }
    }

    public void HideAbilitySelection() {
        transform.Find("Character Window/Ability Select").gameObject.SetActive(false);
        EventManager.CancelButtonPressed.RemoveListener(HideAbilitySelection);
        EventManager.ExitMenu.RemoveListener(MenuManager.Instance.HideAbilitySelection);
        if(!String.IsNullOrWhiteSpace(AbilityBeingChanged)) {
            string[] stanceAndSlot = AbilityBeingChanged.Split("-");
            Stance stance = SaveFile.Instance.Stances[stanceAndSlot[0] == "Heavy" ? 0 : stanceAndSlot[0] == "Light" ? 1 : 2];
            int equippedAbilityCount = 0;
            foreach(Stance.EquippedAbility equippedAbility in SaveFile.Instance.Stances[0].Abilities.Concat(SaveFile.Instance.Stances[1].Abilities).Concat(SaveFile.Instance.Stances[2].Abilities)) {
                if(equippedAbility.Type == stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type) {
                    equippedAbilityCount++;
                }
            }
            if(equippedAbilityCount == 1 && stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type != null) {
                MethodInfo methodInfo2 = stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type.GetMethod("OnUnequip", BindingFlags.Public | BindingFlags.Static);
                if(methodInfo2 != null) {
                    methodInfo2.Invoke(null, null);
                }
            }
            stance.Abilities[int.Parse(stanceAndSlot[1]) - 1].Type = null;
            MenuManager.Instance.transform.Find("Character Window/Abilities/Stances/" + stanceAndSlot[0] + "/" + stanceAndSlot[1] + "/UI_UpgradesLoadout/1").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + "UpgradeNotUnlocked", typeof(Sprite)) as Sprite;
            MenuManager.Instance.transform.Find("Character Window/Abilities/Stances/" + stanceAndSlot[0] + "/" + stanceAndSlot[1] + "/UI_UpgradesLoadout/2").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/" + "UpgradeNotUnlocked", typeof(Sprite)) as Sprite;
            UpdateLoadoutUpgradePoints();
        }
        if(Settings.Instance.ControlScheme == "Gamepad") {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Stances/Heavy/UI_StanceTile").GetComponent<Button>().Select();
        }
    }

    public void HideStanceSelection() {
        transform.Find("Character Window/Stance Select").gameObject.SetActive(false);
        EventManager.CancelButtonPressed.RemoveListener(HideStanceSelection);
        EventManager.ExitMenu.RemoveListener(HideStanceSelection);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            MenuManager.Instance.transform.Find("Character Window/Abilities/Stances/Heavy/UI_StanceTile").GetComponent<Button>().Select();
        }
    }

    public void ShowEffects() {
        MenuManager.Instance.HideEnergySelection();
        MenuManager.Instance.HideAbilitySelection();
        MenuManager.Instance.HideStanceSelection();
        MenuManager.Instance.transform.Find("Character Window/Effects").gameObject.SetActive(true);
    }

    public void HideEffects() {
        Objects.CharacterEffects.gameObject.SetActive(false);
    }

    public int MoneyReward = 0;
    public int ExperienceReward = 0;
    public List<Mission.MissionReward> Rewards = new();

    public void ShowTransitionIntoMissionSelect(bool instant = false) {
        if(instant == false && GameController.Instance.GameplayMode != Constants.GameplayMode.MissionSelect) {
            if(GameController.Objects.TransitionBlackScreen.alpha != 1) {
                UIManager.Instance.ShowBlackScreen(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION);
            }
            if(GameController.Instance.GameplayMode == Constants.GameplayMode.OnStartScreen) {
                GameController.Instance.GameplayMode = Constants.GameplayMode.MissionSelect;
                EventManager.FinishedLoadingArea.AddListener(FinishTransitionIntoMissionSelect);
            }
            else {
                GameController.Instance.WaitAndRunMethodRealtime(Constants.DEFAULT_BLACK_SCREEN_TRANSITION_DURATION, FinishTransitionIntoMissionSelect);
            }
        }
        else {
            GameController.Instance.GameplayMode = Constants.GameplayMode.MissionSelect;
            if(SceneManager.GetActiveScene().name == "MissionSelect") {
                FinishTransitionIntoMissionSelect();
            }
            else {
                EventManager.FinishedLoadingArea.AddListener(FinishTransitionIntoMissionSelect);
            }
        }
    }

    public void FinishTransitionIntoMissionSelect() {
        if(GameController.Instance.GameplayMode != Constants.GameplayMode.MissionSelect) {
            GameController.Instance.GameplayMode = Constants.GameplayMode.MissionSelect;
            EventManager.FinishedLoadingArea.AddListener(FinishTransitionIntoMissionSelect);
            return;
        }
        SaveFile.Instance.SetCorrectCycle();
        HideMissionDetails();
        GameController.Instance.ToggleSavePanel(false);
        if(SceneManager.GetActiveScene().name == "MissionSelect") {
            Utils.GetSceneRootObject("Mission Select").transform.Find("Survival Type Selection").gameObject.SetActive(false);
        }
        Utils.PlaySoundEffect(null, "UI/MissionSelect");
        UIManager.Instance.HideBlackScreen(0.5f);
        foreach(Mission.MissionReward reward in Rewards) {
            if(reward.Name == "MissionReward_Gold" && MoneyReward > 0) {
                SaveFile.Instance.Money += Utils.GetCalculatedGain(MoneyReward);
            }
            else if(reward.Name == "MissionReward_Experience" && ExperienceReward > 0) {
                SaveFile.Instance.ExperiencePoints += Utils.GetCalculatedGain(ExperienceReward);
            }
            else if(reward.Item != null) {
                Item item = (Item)Activator.CreateInstance(reward.Item , new object[] { reward.Rarity });
                item.Amount = reward.Amount;
                int sellPrice = SaveFile.Instance.CheckSellPriceIfItemIsDuplicate(item);
                NotificationController.ShowItemDropNotification(item, sellPrice);
            }
            else if(reward.ClassAndMethodToExecute != null){
                Type type = System.Type.GetType(reward.ClassAndMethodToExecute.Split(".")[0]);
                if(type == null) {
                    Debug.LogError("Could not find class: " + type);
                }
                else {
                    MethodInfo method = type.GetMethod(reward.ClassAndMethodToExecute.Split(".")[1], BindingFlags.Public | BindingFlags.Static);
                    if(method == null) {
                        Debug.LogError("Could not find method: " + method);
                    }
                    else {
                        method.Invoke(null, null);
                    }
                }
            }
        }
        Rewards.Clear();
        MoneyReward = 0;
        ExperienceReward = 0;
        SaveFile.Instance.CheckIfShouldPerformTimeSensitiveEvent();
        Utils.GetSceneRootObject("Mission Select").Find("Camera").GetComponent<Camera>().enabled = true;
        Player.Instance.transform.position = new Vector2(-100, -100);
    }

    public void PlayMissionSelectMusic() {
        if(SaveFile.Instance.Week < 31) {
            Utils.SetDefaultMusic("Mission Screen 1");
        }
        else  if(SaveFile.Instance.Week < 51) {
            Utils.SetDefaultMusic("Mission Screen 2");
        }
        else {
            Utils.SetDefaultMusic("Foreboding_80");
        }
    }

    public void UpdateMissionList() {
        if(SaveFile.Instance.SaveFileType != SaveFile.SaveFileTypeEnum.Story || GameController.Instance.GameplayMode != Constants.GameplayMode.MissionSelect || SceneManager.GetActiveScene().name != "MissionSelect") {
            return;
        }
        if(SaveFile.Instance.CompletedStoryMissions == null) {
            SaveFile.Instance.CompletedStoryMissions = new();
        }
        HideMissionDetails();
        Dictionary<String, int> MissionTypeCounts = new Dictionary<string, int>() {
            {"Activity", 0}, {"MainQuest", 0}, {"SideQuest", 0}, {"Random", 0}, {"Exploration", 0}
        };
        Transform items = Utils.GetSceneRootObject("Mission Select").Find("Mission List/Viewport/Items");
        foreach(String type in new List<String> {"MainQuest", "SideQuest", "Random", "Activity", "Exploration"}) {
            Utils.DestroyAllChildren(items.Find(type));
        }
        List<Mission> missionsMadeAvailable = new List<Mission>();
        
        foreach (Mission mission in AllPossibleMissions)
        {
            if(SaveFile.Instance.Missions.FirstOrDefault(m => m.GetType() == mission.GetType()) == null && 
            mission.MissionShouldBeAvailable() && 
            ((mission.Type != Mission.MissionType.MainQuest && mission.Type != Mission.MissionType.SideQuest) 
            || SaveFile.Instance.CompletedStoryMissions.Contains(mission.GetType()) == false)) {
                SaveFile.Instance.Missions.Add(mission);
                missionsMadeAvailable.Add(mission);
            }
        }
        foreach (Mission mission in missionsMadeAvailable)
        {
            mission.OnMissionMadeAvailable();
            if(mission.CanExpire) {
                mission.WeeksUntilExpiryRemaining = mission.WeeksUntilExpiryMax;
            }
        }
        bool removeAllMissions = false;
        foreach (Mission mission in AllPossibleMissions)
        {
            if(mission.RemoveOtherMissions && mission.MissionShouldBeAvailable()) {
                removeAllMissions = true;
            }
        }
        foreach(Mission mission in SaveFile.Instance.Missions.OrderByDescending(m => m.EnemyLevel).ThenByDescending(m => m.WeeksUntilExpiryRemaining).ThenBy(m => m.GetType().ToString()).ToList()) {
            if(mission.MissionShouldBeAvailable() == false || (mission.WeeksUntilExpiryRemaining <= 0 && mission.CanExpire) || (removeAllMissions && mission.RemoveOtherMissions == false && (SaveFile.Instance.Week == 51 || mission.AlwaysShow == false))) {
                SaveFile.Instance.Missions.Remove(mission);
            }
            else {
                MissionTypeCounts[mission.Type.ToString()]++;
                GameObject missionSelect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MissionSelect")) as GameObject;
                mission.MissionSelectGameObject = missionSelect;
                missionSelect.transform.Find("Text").GetComponent<LabelInitializer>().string_params = mission.GetTitleParameters();
                missionSelect.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel("{" + mission.GetType() + "}");
                missionSelect.transform.Find("Text/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/" + mission.Icon, typeof(Sprite)) as Sprite;
                missionSelect.transform.SetParent(items.Find(mission.Type.ToString()));
                missionSelect.transform.Find("Time Remaining").gameObject.SetActive(mission.CanExpire);
                missionSelect.transform.Find("New").gameObject.SetActive(mission.IsNew);
                if(mission.CanExpire) {
                    mission.MissionSelectGameObject.transform.Find("Time Remaining").gameObject.SetActive(mission.CanExpire);
                    mission.MissionSelectGameObject.transform.Find("Time Remaining/Clock").GetComponent<Image>().color = mission.GetClockColor();
                    mission.MissionSelectGameObject.transform.Find("Time Remaining/Weeks").GetComponent<TextMeshProUGUI>().text = mission.WeeksUntilExpiryRemaining.ToString();
                }
                mission.MissionSelectGameObject.transform.Find("Level").gameObject.SetActive(mission.EnemyLevel > 0);
                if(mission.EnemyLevel > 0) {
                    mission.MissionSelectGameObject.transform.Find("Level/Text").GetComponent<TextMeshProUGUI>().text = mission.EnemyLevel.ToString();
                }
                missionSelect.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        foreach(String type in MissionTypeCounts.Keys) {
            items.Find(type + " Title").gameObject.SetActive(MissionTypeCounts[type] > 0);
            items.Find(type).gameObject.SetActive(MissionTypeCounts[type] > 0);
        }
    }

    public void InitializeMissionList() {
        SaveFile.Instance.Missions = new();
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Mission))))
        {
            Mission m = (Mission)Activator.CreateInstance(type);
            if(m.Type != Mission.MissionType.Random) {
                AllPossibleMissions.Add(m);
            }
        }
    }

    public Mission CurrentlySelectedMission;

    public void ShowMissionDetails(GameObject mission_select) {
        Mission mission = SaveFile.Instance.Missions.FirstOrDefault(m => m.MissionSelectGameObject == mission_select);
        if(mission == null) {
            UnityEngine.Debug.LogError("Could not find mission for gameObject: " + Utils.GetGameObjectPath(mission_select));
            return;
        }
       Transform markers = Utils.GetSceneRootObject("Mission Select").Find("Markers");
        foreach(Transform marker in markers) {
            marker.gameObject.SetActive(marker.gameObject.name == mission.MapMarker);
            if(marker.gameObject.name == mission.MapMarker) {
                marker.GetComponent<Image>().sprite = Resources.Load("Sprites/UI/MarkerActive", typeof(Sprite)) as Sprite;
            }
        }
        CurrentlySelectedMission = mission;
        Utils.GetSceneRootObject("Mission Select").Find("Mission List").gameObject.SetActive(false);
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details").gameObject.SetActive(true);
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Top Info/Time Passed").GetComponent<LabelInitializer>().SetLabel(mission.NumberOfWeeksConsumed == 0 ? "{MissionNoTimeConsumed}" : mission.NumberOfWeeksConsumed == 1 ? "{Mission1WeekConsumed}" : String.Format(Label.Get("MissionWeeksConsumed"), new string[] {mission.NumberOfWeeksConsumed.ToString()}));
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Top Info/Time Passed/Image").GetComponent<Image>().color = mission.GetClockColor();
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Top Info/Level").GetComponent<LabelInitializer>().SetLabel(String.Format(Label.Get("MissionDetailsLevel"), new string[] {mission.EnemyLevel == 0 ? "-" : mission.EnemyLevel.ToString()}));
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Title/Text").GetComponent<LabelInitializer>().string_params = mission.GetTitleParameters();
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Title/Text").GetComponent<LabelInitializer>().SetLabel("{" + mission.GetType() + "}");
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Title/Image").GetComponent<Image>().sprite = Resources.Load("Sprites/" + mission.Icon, typeof(Sprite)) as Sprite;
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Description").GetComponent<LabelInitializer>().string_params = mission.GetDescriptionParameters();
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Description").GetComponent<LabelInitializer>().SetLabel("{" + mission.GetDescriptionLabel() + "}");
        Transform rewards_list = Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Viewport/Content/Bottom Info/RewardsList");
        Utils.DestroyAllChildren(rewards_list);
        bool at_least_one_reward = false;
        foreach(Mission.MissionReward reward in mission.GetRewards()) {
            at_least_one_reward = true;
            GameObject missionReward = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_MissionReward")) as GameObject;
            missionReward.transform.SetParent(rewards_list);
            if(reward.Item != null) {
                Item item = (Item)Activator.CreateInstance(reward.Item, new object[] { ItemGrade.Regular });
                string amount_string = reward.Amount > 1 ? " x" + reward.Amount.ToString() : "";
                if (item.Type == Constants.ItemType.Heavy || item.Type == Constants.ItemType.Light || item.Type == Constants.ItemType.Ranged)
                {
                    missionReward.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel("{" + item.GetType().ToString() + "} ({ItemGrade_" + reward.Rarity.ToString() + "_Colored} {ItemClass_" + item.WeaponClass.ToString() + "})" + amount_string);
                }
                else
                {
                    missionReward.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel("{" + item.GetType().ToString() + "} ({ItemGrade_" + reward.Rarity.ToString() + "_Colored} {ItemType_" + item.Type.ToString() + "})" + amount_string);
                }
                missionReward.transform.Find("Image").gameObject.SetActive(item.Type != Constants.ItemType.Heavy && item.Type != Constants.ItemType.Light  && item.Type != Constants.ItemType.Ranged );
                MenuManager.Instance.SetRegularImage(missionReward.transform.Find("Image").gameObject, item);
            }
            else {
                missionReward.transform.Find("Text").GetComponent<LabelInitializer>().string_params = new List<string> {Utils.GetFormattedInteger(reward.Amount).ToString()};
                missionReward.transform.Find("Text").GetComponent<LabelInitializer>().SetLabel("{" + reward.Name + "}");
                missionReward.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load("Sprites/" + reward.Graphic, typeof(Sprite)) as Sprite;
            }
        }
        rewards_list.transform.parent.gameObject.SetActive(at_least_one_reward);
        if(Settings.Instance.ControlScheme == "Gamepad") {
            Utils.GetSceneRootObject("Mission Select").Find("Mission Details/Confirm").GetComponent<Button>().Select();
        }
    }

    public void HideMissionDetails() {
        if(GameController.Instance.GameplayMode != Constants.GameplayMode.MissionSelect || SceneManager.GetActiveScene().name != "MissionSelect") {
            return;
        }
        Utils.GetSceneRootObject("Mission Select").Find("Mission List").gameObject.SetActive(true);
        Utils.GetSceneRootObject("Mission Select").Find("Mission Details").gameObject.SetActive(false);
        SelectTopmostMission();
    }

    public void RemoveMission(Mission mission) {
        if(SaveFile.Instance.Missions.Contains(mission)) {
            SaveFile.Instance.Missions.Remove(mission);
        }
        UpdateMissionList();
    }

    public void StartMission() {
        UIManager.Instance.ShowBlackScreen(0.5f);
        GameController.Instance.WaitAndRunMethodRealtime(1f, ContinueStartMission);
    }

    public void ContinueStartMission() {
        UIManager.Instance.ToggleLoadingScreen();
        GameController.Instance.WaitAndRunMethodRealtime(0.01f, FinishStartingMission);
    }  

    public void FinishStartingMission() {
        if(CurrentlySelectedMission.Type != Mission.MissionType.Activity && CurrentlySelectedMission.Type != Mission.MissionType.Exploration && CurrentlySelectedMission.CanAbandonMission) {
            SaveFile.Instance.CurrentMissionAttemptId = Guid.NewGuid().ToString();
            SaveFile.Instance.Save("PreMissionAutoSave-" + SaveFile.Instance.CurrentMissionAttemptId + ".es3");
        }
        SaveFile.Instance.CurrentMission = CurrentlySelectedMission;
        CurrentlySelectedMission.OnStart();
        EventManager.FinishedLoadingArea.AddListener(CurrentlySelectedMission.OnFinishedLoadingArea);
        UIManager.Instance.ToggleLoadingScreen(false);
    }

    public void MoveToNextCycle() {
        SaveFile.Instance.Cycle++;
        SaveFile.Instance.Week = 1;
        SaveFile.Instance.Money = 0;
        SaveFile.Instance.PowerUpsRemovedOnNextCycle.Clear();
        MenuManager.Instance.ClearInventory();
        SaveFile.Instance.AddDefaultItems();
        SaveFile.Instance.HealthGainedFromTraining = 0;
        SaveFile.Instance.StaggerBarGainedFromTraining = 0;
        SaveFile.Instance.GoldFromInvestmentsMinimum += SaveFile.Instance.ExtraGoldFromInvestmentsStartingNextCycle / 2;
        SaveFile.Instance.GoldFromInvestmentsMaximum += SaveFile.Instance.ExtraGoldFromInvestmentsStartingNextCycle;
        SaveFile.Instance.ExtraGoldFromInvestmentsStartingNextCycle = 0;
        SaveFile.Instance.WeeksAndInvestments.Clear();
        MenuManager.Instance.InitializeMissionList();
        Utils.GetSceneRootObject("Mission Select").Find("Cycle").GetComponent<Image>().sprite = Resources.Load("Sprites/UI/Cycle" + SaveFile.Instance.Cycle, typeof(Sprite)) as Sprite;
        UIManager.Objects.CycleDisplayImage.sprite = Resources.Load("Sprites/UI/Cycle" + SaveFile.Instance.Cycle, typeof(Sprite)) as Sprite;
        SaveFile.Instance.EquippedHelmet = null;
        SaveFile.Instance.EquippedOutfit = null;
        SaveFile.Instance.EquippedGloves = null;
        SaveFile.Instance.EquippedBoots = null;
        SaveFile.Instance.EquippedItem1 = null;
        SaveFile.Instance.EquippedItem2 = null;
        SaveFile.Instance.GetQuest("3LastChances").AdvanceObjective(SaveFile.Instance.Cycle * 100);
    }

    public Mission GetMission(Type mission_type) {
        Mission m = AllPossibleMissions.FirstOrDefault(m => m.GetType() == mission_type);
        if(m != null) {
            return m;
        }
        Mission m2 = (Mission)Activator.CreateInstance(mission_type);
        return m2.Type == Mission.MissionType.Random ? m2 : null;
    }

    public void AddMission(Type mission_type) {
        if(SaveFile.Instance.SaveFileType != SaveFile.SaveFileTypeEnum.Story) {
            return;
        }
        Mission m = MenuManager.Instance.GetMission(mission_type);
        m.IgnoreRemoveAllMissions = true;
        m.WeeksUntilExpiryRemaining = m.WeeksUntilExpiryMax;
        SaveFile.Instance.Missions.Add(m);
    }

    public void SelectTopmostMission() {
        if(Settings.Instance.ControlScheme == "Gamepad") {
            Transform items2 = Utils.GetSceneRootObject("Mission Select").Find("Mission List/Viewport/Items");
            foreach(Transform folder in new List<Transform>{items2.Find("MainQuest"), items2.Find("SideQuest"), items2.Find("Random"), items2.Find("Activity")}) {
                if(folder.childCount > 0) {
                    folder.GetChild(0).GetComponent<Button>().Select();
                    break;
                }
            }
        }
    }

    public void OpenShop(List<Item> items, string shop_name = "GenericShop") {
        GameController.Instance.GameplayMode = Constants.GameplayMode.Shopping;
        Transform itemContainer = GameController.Objects.ShopItems.transform;
        foreach(Item item in items) {
            if(item.BuyPrice == 0) {
                item.BuyPrice = item.SellPrice * 2;
            }
        }
        items = items.OrderByDescending(item => item.BuyPrice).ToList();
        Utils.DestroyAllChildren(itemContainer);
        foreach(Item item in items) {
            if(SaveFile.Instance.Inventory.FirstOrDefault(i => (i.Type != Constants.ItemType.Tool || i.Amount == i.MaxAmount) && i is not Quest_UpgradeMaterials && i.GetType() == item.GetType() && i.Grade == item.Grade) == null 
            && (item.CanOnlyBuyOnce == false || !SaveFile.Instance.HasFlag(shop_name + "_" + item.GetType() + "_" + item.Grade + "_[Cycle]"))) {
                GameObject shop_tile = MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/UI_ShopTile")) as GameObject;
                shop_tile.name = itemContainer.childCount.ToString();
                ShopTile tile = shop_tile.GetComponent<ShopTile>();
                SetRegularImage(shop_tile.transform.Find("Image").gameObject, item);
                tile.Item = item;
                tile.ShopName = shop_name;
                tile.transform.Find("Price/Text").GetComponent<TextMeshProUGUI>().text = Utils.GetFormattedInteger(item.BuyPrice);
                tile.InitializeOptions();
                shop_tile.transform.SetParent(itemContainer);
                shop_tile.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void CloseShop() {
        GameController.Instance.GameplayMode = Constants.GameplayMode.Regular;
        Utils.DestroyAllChildren(GameController.Objects.ShopItems.transform);
    }

    public void CloseStartScreenOptionsMenu() {
        transform.Find("Options Window/StartScreenOptions").gameObject.SetActive(false);
        transform.Find("Options Window/Options/Gameplay/Items/Difficulty").gameObject.SetActive(true);
        Utils.SetActiveOnCanvasGroup(Utils.CanvasType.StartScreen, true);
        GameController.Instance.GameplayMode = Constants.GameplayMode.OnStartScreen;
    }

    public static class Objects {
        public static GameObject MenuSelection => Utils.GetGameObject("Menu/Menu Selection");
        public static GameObject Notifications => Utils.GetGameObject("Menu/Notifications/List");
        public static LabelInitializer InventoryItemDescriptionLabel => (LabelInitializer)Utils.GetComponent("Menu/Inventory Window/Details/Description/Image/Description", typeof(LabelInitializer));
        public static GameObject SkillTreeDescription => Utils.GetGameObject("Menu/Skill Tree Window/Details/Description/Image/Description");
        public static LabelInitializer SkillTreeDescriptionLabel => (LabelInitializer)Utils.GetComponent("Menu/Skill Tree Window/Details/Description/Image/Description", typeof(LabelInitializer));
        public static GameObject SkillTreeFlavorText => Utils.GetGameObject("Menu/Skill Tree Window/Details/Description/Image/Flavor Text");
        public static GameObject SkillTreeLongDescription => Utils.GetGameObject("Menu/Skill Tree Window/Details/Description/Image/Description (No Flavor)");
        public static LabelInitializer SkillTreeLongDescriptionLabel => (LabelInitializer)Utils.GetComponent("Menu/Skill Tree Window/Details/Description/Image/Description (No Flavor)", typeof(LabelInitializer));
        public static LabelInitializer TutorialsWindowTitleLabel => (LabelInitializer)Utils.GetComponent("Menu/Tutorials Window/Description Window/Title/Text", typeof(LabelInitializer));
        public static Image TutorialsWindowImage => (Image)Utils.GetComponent("Menu/Tutorials Window/Description Window/Image/Image", typeof(Image));
        public static LabelInitializer TutorialsWindowDescriptionLabel => (LabelInitializer)Utils.GetComponent("Menu/Tutorials Window/Description Window/Description", typeof(LabelInitializer));
        public static GameObject InventoryEquipment => Utils.GetGameObject("Menu/Inventory Window/Equipment");
        public static GameObject InventoryItems => Utils.GetGameObject("Menu/Inventory Window/Inventory/Viewport/Items");
        public static GameObject InventoryItemsTools => Utils.GetGameObject("Menu/Inventory Window/Inventory/Viewport/Items/Tool/Items");
        public static GameObject CharacterSheet => Utils.GetGameObject("Menu/Character Window");
        public static GameObject CharacterStatList => Utils.GetGameObject("Menu/Character Window/Stats/Stats");
        public static GameObject MenuArchive => Utils.GetGameObject("Menu/Archive Window/Dialogue History/Scroll Rect/Viewport/Content");
        public static GameObject CharacterEffects => Utils.GetGameObject("Menu/Character Window/Effects");
        public static GameObject CharacterEffectList => Utils.GetGameObject("Menu/Character Window/Effects/Viewport/Items");
        public static Slider OptionsInWorldDialogueSpeedSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/InGame Dialogue Speed/Slider", typeof(Slider));
        public static LabelInitializer OptionsInWorldDialogueSpeedLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/InGame Dialogue Speed/Label", typeof(LabelInitializer));
        public static Slider OptionsDialogueTextSpeedSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Dialogue Text Speed/Slider", typeof(Slider));
        public static LabelInitializer OptionsDialogueTextSpeedLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Dialogue Text Speed/Label", typeof(LabelInitializer));
        public static Slider OptionsMasterVolumeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Master Volume/Slider", typeof(Slider));
        public static LabelInitializer OptionsMasterVolumeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Master Volume/Label", typeof(LabelInitializer));
        public static Slider OptionsMusicVolumeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Music Volume/Slider", typeof(Slider));
        public static LabelInitializer OptionsMusicVolumeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Music Volume/Label", typeof(LabelInitializer));
        public static Slider OptionsSoundVolumeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Sound Volume/Slider", typeof(Slider));
        public static LabelInitializer OptionsSoundVolumeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Sound Volume/Label", typeof(LabelInitializer));
        public static Slider OptionsDialogueVolumeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Dialogue Volume/Slider", typeof(Slider));
        public static LabelInitializer OptionsDialogueVolumeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Audio/Items/Dialogue Volume/Label", typeof(LabelInitializer));
        public static Slider OptionsFieldOfViewSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Display/Items/Field of View/Slider", typeof(Slider));
        public static LabelInitializer OptionsFieldOfViewLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Display/Items/Field of View/Label", typeof(LabelInitializer));
        public static Slider OptionsUISizeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Display/Items/UI Size/Slider", typeof(Slider));
        public static LabelInitializer OptionsUISizeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Display/Items/UI Size/Label", typeof(LabelInitializer));
        public static Slider OptionsDamageNumbersSizeSlider => (Slider)Utils.GetComponent("Menu/Options Window/Options/Display/Items/Damage Numbers Size/Slider", typeof(Slider));
        public static LabelInitializer OptionsDamageNumbersSizeLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Display/Items/Damage Numbers Size/Label", typeof(LabelInitializer));
        public static Toggle OptionsSkipReadDialogueToggle => (Toggle)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/AutoSkip/Background", typeof(Toggle));
        public static Toggle OptionsAllowSkipUnreadDialogueToggle => (Toggle)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/SkipText/Background", typeof(Toggle));
        public static Toggle ShowExtraInfoInUIToggle => (Toggle)Utils.GetComponent("Menu/Options Window/Options/Display/Items/ShowExtraInfoInUI/Background", typeof(Toggle));
        public static TMP_Dropdown OptionsLanguageDropdown => (TMP_Dropdown)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Language/Dropdown", typeof(TMP_Dropdown));
        public static LabelInitializer OptionsLanguageLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Language/Dropdown/Label", typeof(LabelInitializer));
        public static TMP_Dropdown OptionsDifficultyDropdown => (TMP_Dropdown)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Difficulty/Dropdown", typeof(TMP_Dropdown));
        public static LabelInitializer OptionsDifficultyLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Difficulty/Dropdown/Label", typeof(LabelInitializer));
        public static TMP_Dropdown OptionsControlsDropdown => (TMP_Dropdown)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Controls/Dropdown", typeof(TMP_Dropdown));
        public static LabelInitializer OptionsControlsLabel => (LabelInitializer)Utils.GetComponent("Menu/Options Window/Options/Gameplay/Items/Controls/Dropdown/Label", typeof(LabelInitializer));
        public static TextMeshProUGUI InventoryMoneyText => (TextMeshProUGUI)Utils.GetComponent("Menu/Inventory Window/Equipment/Money/Amount", typeof(TextMeshProUGUI));
    }
}