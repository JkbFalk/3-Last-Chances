using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CanvasElements {
    private static GameObject _gameController;

    public static GameObject GameController
    {
        get {
            if (_gameController == null)
            {
                _gameController = GameObject.FindGameObjectWithTag("Game Controller");
            }
            return _gameController; 
        }
    }
    private static GameObject _shopCanvasObject;
    public static GameObject ShopCanvasObject => GetCanvasElement(ref _shopCanvasObject, "Shop");
    private static GameObject _shopItems;
    public static GameObject ShopItems => GetCanvasElement(ref _shopItems, "Shop/Items/Viewport/Items");
    private static GameObject _dialogueMoneyDisplay;
    public static GameObject DialogueMoneyDisplay => GetCanvasElement(ref _dialogueMoneyDisplay, "Dialogue Window/Money");
    private static GameObject _dialogueExperienceBar;
    public static GameObject DialogueExperienceBar => GetCanvasElement(ref _dialogueExperienceBar, "Dialogue Window/Level");

    private static GameObject _shopMoneyDisplay;
    public static GameObject ShopMoneyDisplay => GetCanvasElement(ref _shopMoneyDisplay, "Shop/Money");

    private static GameObject _dialogueNotifications;
    public static GameObject DialogueNotifications => GetCanvasElement(ref _dialogueNotifications, "Dialogue Window/Window/List");

    private static GameObject _dialogueArchive;
    public static GameObject DialogueArchive => GetCanvasElement(ref _dialogueArchive, "Dialogue Window/Dialogue History/Scroll Rect/Viewport/Content");

    private static GameObject _missionSelectObject; 
    public static GameObject MissionSelectObject => GetCanvasElement(ref _missionSelectObject, "Mission Select");

    private static GameObject _unlockInformation;
    public static GameObject UnlockInformation => GetCanvasElement(ref _unlockInformation, "Unlock Information"); 

    private static GameObject _levelUpSelection;
    public static GameObject LevelUpSelection => GetCanvasElement(ref _levelUpSelection, "Level Up Selection");

    private static GameObject _levelUpPassives;
    public static GameObject LevelUpPassives => GetCanvasElement(ref _levelUpPassives, "Level Up Selection/Choices/Passives");

    private static GameObject _menuCanvasObject;
    public static GameObject MenuCanvasObject => GetCanvasElement(ref _menuCanvasObject, "Menu Canvas");

    public static class MenuCanvas
    {
        private static GameObject _notifications;
        public static GameObject Notifications => GetCanvasElement(ref _notifications, "Menu Canvas/Notifications/List");
        private static GameObject _gamepadIndicator;
        public static GameObject GamepadIndicator => GetCanvasElement(ref _gamepadIndicator, "Menu Canvas/Gamepad Indicator");

        private static GameObject _tutorialWindowTitle;
        public static GameObject TutorialWindowTitle => GetCanvasElement(ref _tutorialWindowTitle, "Menu Canvas/Guide Window/Description Window/Title/Text");

        private static GameObject _tutorialWindowImage;
        public static GameObject TutorialWindowImage => GetCanvasElement(ref _tutorialWindowImage, "Menu Canvas/Guide Window/Description Window/Image/Image");

        private static GameObject _tutorialWindowDescription; 
        public static GameObject TutorialWindowDescription => GetCanvasElement(ref _tutorialWindowDescription, "Menu Canvas/Guide Window/Description Window/Description");

        private static GameObject _categorySelection;
        public static GameObject CategorySelection => GetCanvasElement(ref _categorySelection, "Menu Canvas/Inventory Window/Inventory/Category Selection");

        private static GameObject _energySelection;
        public static GameObject EnergySelection => GetCanvasElement(ref _energySelection, "Menu Canvas/Overview Window/Energy Select/Energies");

        private static GameObject _inventoryItems;
        public static GameObject InventoryItems => GetCanvasElement(ref _inventoryItems, "Menu Canvas/Inventory Window/Inventory/Viewport");

        private static GameObject _statList;
        public static GameObject StatList => GetCanvasElement(ref _statList, "Menu Canvas/Overview Window/Stats/Stats");

        private static GameObject _menuArchive;
        public static GameObject MenuArchive => GetCanvasElement(ref _menuArchive, "Menu Canvas/History Window/Dialogue History/Scroll Rect/Viewport/Content");
        private static GameObject _energy;
        public static GameObject Energy => GetCanvasElement(ref _energy, "Menu Canvas/Overview Window/Abilities/Right-side Panel/Energy");

         private static GameObject _effectsOverview;
        public static GameObject EffectsOverview => GetCanvasElement(ref _effectsOverview, "Menu Canvas/Overview Window/Effects");
    
    }

    private static GameObject _worldSpaceCanvasObject;
    public static GameObject WorldSpaceCanvasObject => GetCanvasElement(ref _worldSpaceCanvasObject, "World Space Canvas");

    private static GameObject _transitionScreenObject;
    public static GameObject TransitionScreenObject => GetCanvasElement(ref _transitionScreenObject, "Transition Screen");

    private static AudioSource _music;

    public static AudioSource Music {
        get {
            if (_music == null) {
                _music = GameController.transform.Find("Music").GetComponent<AudioSource>();
            }
            return _music;
        }
    }

    private static AudioSource _audioListener;

    public static AudioSource AudioListener {
        get {
            if (_audioListener == null) {
                _audioListener = GameController.transform.Find("Audio Listener").GetComponent<AudioSource>();
            }
            return _audioListener;
        }
    }

    /*private static GameObject _inWorldDialogueSpeedSlider;
    public static GameObject InWorldDialogueSpeedSlider => GetCanvasElement(ref _inWorldDialogueSpeedSlider, "Menu Canvas/Settings Window/Gameplay/Items/InGame Dialogue Speed");*/
    private static GameObject _dialogueTextSpeedSlider;
    public static GameObject DialogueTextSpeedSlider => GetCanvasElement(ref _dialogueTextSpeedSlider, "Menu Canvas/Settings Window/Gameplay/Items/Dialogue Text Speed");

    private static GameObject _masterVolumeSlider;
    public static GameObject MasterVolumeSlider => GetCanvasElement(ref _masterVolumeSlider, "Menu Canvas/Settings Window/Audio/Items/Master Volume");

    private static GameObject _musicVolumeSlider;
    public static GameObject MusicVolumeSlider => GetCanvasElement(ref _musicVolumeSlider, "Menu Canvas/Settings Window/Audio/Items/Music Volume");

    private static GameObject _soundVolumeSlider;
    public static GameObject SoundVolumeSlider => GetCanvasElement(ref _soundVolumeSlider, "Menu Canvas/Settings Window/Audio/Items/Sound Volume");
    private static GameObject _dialogueVolumeSlider;
    public static GameObject DialogueVolumeSlider => GetCanvasElement(ref _dialogueVolumeSlider, "Menu Canvas/Settings Window/Audio/Items/Dialogue Volume");

    private static GameObject _fieldOfViewSlider;
    public static GameObject FieldOfViewSlider => GetCanvasElement(ref _fieldOfViewSlider, "Menu Canvas/Settings Window/Graphics/Items/Field of View");

    public static class TransitionScreen {
        private static GameObject _blackScreen;
        public static GameObject BlackScreen => GetCanvasElement(ref _blackScreen, "Transition Screen/Black Screen");
        private static GameObject _loadingScreen;
        public static GameObject LoadingScreen => GetCanvasElement(ref _loadingScreen, "Transition Screen/Loading Screen");
        private static GameObject _loadProgress;
        public static GameObject LoadProgress => GetCanvasElement(ref _loadProgress, "Transition Screen/Loading Screen/Progress");

        private static GameObject _upperText;
        public static GameObject UpperText => GetCanvasElement(ref _upperText, "Transition Screen/Upper Text");

        private static GameObject _lowerText;
        public static GameObject LowerText => GetCanvasElement(ref _lowerText, "Transition Screen/Lower Text");

        private static GameObject _areaName;
        public static GameObject AreaName => GetCanvasElement(ref _areaName, "Transition Screen/Area Name");
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Anagram")]
    private static GameObject _UICanvasObject;

    public static GameObject UICanvasObject => GetCanvasElement(ref _UICanvasObject, "UI Canvas");

    public static class UICanvas {
        private static GameObject _notifications;
        public static GameObject Notifications => GetCanvasElement(ref _notifications, "UI Canvas/Notifications/List");
        private static GameObject _moneyDisplay;
        public static GameObject MoneyDisplay => GetCanvasElement(ref _moneyDisplay, "UI Canvas/Money");
        private static GameObject _experienceBar;
        public static GameObject ExperienceBar => GetCanvasElement(ref _experienceBar, "UI Canvas/Level");
        private static GameObject _inGameDialogue;
        public static GameObject InGameDialogue => GetCanvasElement(ref _inGameDialogue, "UI Canvas/Notifications/In-Game Dialogue");
        private static GameObject _effects;
        public static GameObject Effects => GetCanvasElement(ref _effects, "UI Canvas/Effects");
        private static GameObject _resourceBars;
        public static GameObject ResourceBars => GetCanvasElement(ref _resourceBars, "UI Canvas/Resource Bars");
        private static GameObject _ultimateUses;
        public static GameObject UltimateUses => GetCanvasElement(ref _ultimateUses, "UI Canvas/Resource Bars/Ultimate Uses");
        private static GameObject _timer;
        public static GameObject Timer => GetCanvasElement(ref _timer, "UI Canvas/Timer");

        private static GameObject _chargeBar;
        public static GameObject ChargeBar => GetCanvasElement(ref _chargeBar, "UI Canvas/Charge Bar");

        private static GameObject _inCombatMask;
        public static GameObject InCombatMask => GetCanvasElement(ref _inCombatMask, "UI Canvas/Resource Bars/InCombat Indicator/Mask");

        private static GameObject _inCombatFill;
        public static GameObject InCombatFill => GetCanvasElement(ref _inCombatFill, "UI Canvas/Resource Bars/InCombat Indicator/Mask/Red Fill");
        public static GameObject AmmoDisplay => Settings.Instance.ControlScheme == "Keyboard" ? StanceDisplayKeyboard.transform.Find("Ammo Display").gameObject : StanceDisplayGamepad.transform.Find("Ammo Display").gameObject;

        public static GameObject Stances => Settings.Instance.ControlScheme == "Keyboard" ? StanceDisplayKeyboard.transform.Find("Stances").gameObject : StanceDisplayGamepad.transform.Find("Stances").gameObject;

        private static GameObject _itemsGamepad;
        public static GameObject ItemsGamepad => GetCanvasElement(ref _itemsGamepad, "UI Canvas/Stance Display Gamepad/Items");

        public static GameObject Items => Settings.Instance.ControlScheme == "Keyboard" ? StanceDisplayKeyboard.transform.Find("Items").gameObject : StanceDisplayGamepad.transform.Find("Items").gameObject;
        
        private static GameObject _stanceGaugeContainerKeyboard;
        public static GameObject StanceGaugeContainerKeyboard => GetCanvasElement(ref _stanceGaugeContainerKeyboard, "UI Canvas/Stance Gauge");

        private static GameObject _stanceGaugeContainerGamepad;
        public static GameObject StanceGaugeContainerGamepad => GetCanvasElement(ref _stanceGaugeContainerGamepad, "UI Canvas/Stance Gauge");

        private static GameObject _stanceGaugeContainer;
        public static GameObject StanceGaugeContainer => Settings.Instance.ControlScheme == "Keyboard" ? StanceGaugeContainerKeyboard : StanceGaugeContainerGamepad;

        private static GameObject _stanceDisplayKeyboard;
        public static GameObject StanceDisplayKeyboard => GetCanvasElement(ref _stanceDisplayKeyboard, "UI Canvas/Stance Display Keyboard");

        private static GameObject _stanceDisplayGamepad;
        public static GameObject StanceDisplayGamepad => GetCanvasElement(ref _stanceDisplayGamepad, "UI Canvas/Stance Display Gamepad");

        private static GameObject _abilitiesKeyboard;
        public static GameObject AbilitiesKeyboard => GetCanvasElement(ref _abilitiesKeyboard, "UI Canvas/Stance Display Keyboard/Abilities");

        private static GameObject _abilitiesGamepad;
        public static GameObject AbilitiesGamepad => GetCanvasElement(ref _abilitiesGamepad, "UI Canvas/Stance Display Gamepad/Abilities");

        public static GameObject Abilities => Settings.Instance.ControlScheme == "Keyboard" ? AbilitiesKeyboard : AbilitiesGamepad;

        private static GameObject _eliteEnemyDisplays;
        public static GameObject EliteEnemyDisplays => GetCanvasElement(ref _eliteEnemyDisplays, "UI Canvas/Elite Enemy Displays");

        private static GameObject _objectivesDisplay;
        public static GameObject ObjectivesDisplay => GetCanvasElement(ref _objectivesDisplay, "UI Canvas/Objectives");

        private static GameObject _helpTextOpen;
        public static GameObject HelpTextOpen => GetCanvasElement(ref _helpTextOpen, "UI Canvas/Help Text Open");

        private static GameObject _helpTextOpenMessage;
        public static GameObject HelpTextOpenMessage => GetCanvasElement(ref _helpTextOpenMessage, "UI Canvas/Help Text Open/Text");

        private static GameObject _helpTextOpenLabel;
        public static GameObject HelpTextOpenLabel => GetCanvasElement(ref _helpTextOpenLabel, "UI Canvas/Help Text Open/Label");

        private static GameObject _helpTextClosedLabel;
        public static GameObject HelpTextClosedLabel => GetCanvasElement(ref _helpTextClosedLabel, "UI Canvas/Help Text Closed/Label");

        private static GameObject _helpTextClosed;
        public static GameObject HelpTextClosed => GetCanvasElement(ref _helpTextClosed, "UI Canvas/Help Text Closed");


        private static GameObject _fpsCounter;
        public static GameObject FPSCounter => GetCanvasElement(ref _fpsCounter, "UI Canvas/FPS Counter");

        private static Image _gamepadAbilitiesIndicator;

        public static Image GamepadAbilitiesIndicator
        {
            get
            {
                if(_gamepadAbilitiesIndicator == null)
                {
                    _gamepadAbilitiesIndicator = CanvasElements.UICanvas.AbilitiesGamepad.transform.parent.Find("Binding Abilities").GetComponent<Image>();
                }
                return _gamepadAbilitiesIndicator;
            }
        }

        private static Image _gamepadItemsIndicator;

        public static Image GamepadItemsIndicator
        {
            get
            {
                if (_gamepadItemsIndicator == null)
                {
                    _gamepadItemsIndicator = CanvasElements.UICanvas.AbilitiesGamepad.transform.parent.Find("Binding Items").GetComponent<Image>();
                }
                return _gamepadItemsIndicator;
            }
        }
    }

    public static GameObject GetCanvasElement(ref GameObject game_object, string path)
    {
        if (GameController != null && game_object == null)
        {
            game_object = GameController.transform.Find(path)?.gameObject;
        }
        return game_object;
    }

    public static void SetActiveOnCanvasGroup(GameObject element_to_change, bool is_active)
    {
        CanvasGroup item = element_to_change.GetComponent<CanvasGroup>();
        item.alpha = is_active ? 1 : 0;
        item.interactable = is_active;
        item.blocksRaycasts = is_active;
    }
}