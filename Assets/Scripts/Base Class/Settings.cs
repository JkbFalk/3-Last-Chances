using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

[System.Serializable]
public class Settings {
    private static Settings _instance = null;
    public static Settings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Settings();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public Settings()
    {
        saveFile = Application.persistentDataPath + "/Settings.json";
    }

    public string GamepadType = "Xbox";
    public string GameVersion = Application.version;
    public bool CreateAuditLogs = true;
    public bool _allowSkipUnreadDialogue = true;
    public bool AllowSkipUnreadDialogue {
        get => _allowSkipUnreadDialogue;
        set {
            _allowSkipUnreadDialogue = value;
            MenuManager.Objects.OptionsAllowSkipUnreadDialogueToggle.GetComponent<Toggle>().isOn = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public bool _autoSkipReadDialogue = false;
    public bool AutoSkipReadDialogue {
        get => _autoSkipReadDialogue;
        set {
            _autoSkipReadDialogue = value;
            MenuManager.Objects.OptionsSkipReadDialogueToggle.GetComponent<Toggle>().isOn = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public string _controlScheme = "Keyboard";

    public string ControlScheme {
        get => _controlScheme;
        set {
            if (value == "Gamepad") {
                UIManager.Objects.StanceDisplayKeyboard.GetComponent<CanvasGroup>().alpha = 0;
                UIManager.Objects.StanceDisplayGamepad.GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (value == "Keyboard") {
                UIManager.Objects.StanceDisplayKeyboard.GetComponent<CanvasGroup>().alpha = 1;
                UIManager.Objects.StanceDisplayGamepad.GetComponent<CanvasGroup>().alpha = 0;
            }
            _controlScheme = value;
            MenuManager.Objects.OptionsControlsDropdown.value = value == "Keyboard" ? 0 : 1;
            if(Player.Instance != null && SaveFile.Instance.Stances != null)
            {
                foreach (Stance stance in SaveFile.Instance.Stances)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        stance.Abilities[i].Reload();
                    }
                    if(GameController.Instance.transform.Find("UI/Stance Display " + Settings.Instance.ControlScheme + "/Stances").childCount == 3) {
                        stance.UIStanceDisplay = GameController.Instance.transform.Find("UI/Stance Display " + Settings.Instance.ControlScheme + "/Stances/UI_" + stance.WeaponType + "StanceDisplay").transform;
                        stance.StanceCooldownDisplay = GameController.Instance.transform.Find("UI/Stance Display " + Settings.Instance.ControlScheme + "/Stances/UI_" + stance.WeaponType + "StanceDisplay/Cooldown").GetComponent<Image>();
                    }
                }
            }
            foreach(Toggle toggle in MenuManager.Objects.MenuSelection.GetComponentsInChildren<Toggle>(true))
            {
                toggle.interactable = _controlScheme == "Keyboard";
            }
            foreach (Scrollbar scrollbar in GameController.Instance.GetComponentsInChildren<Scrollbar>(true))
            {
                scrollbar.interactable = _controlScheme == "Keyboard" || scrollbar.gameObject.name == "Quest Details Scrollbar";
            }
            foreach(SetActiveDependingOnControlScheme item in GameController.Instance.GetComponentsInChildren<SetActiveDependingOnControlScheme>(true)) {
                if(item.SetInteractableInsteadOfActive) {
                    if(item.GetComponent<Button>() != null) {
                        item.GetComponent<Button>().interactable = (_controlScheme == "Keyboard" && item.ActiveForKeyboard) || (_controlScheme == "Gamepad" && item.ActiveForGamepad);
                    }
                    if(item.GetComponent<Toggle>() != null) {
                        item.GetComponent<Toggle>().interactable = (_controlScheme == "Keyboard" && item.ActiveForKeyboard) || (_controlScheme == "Gamepad" && item.ActiveForGamepad);
                    }
                }
                else {
                    item.gameObject.SetActive((_controlScheme == "Keyboard" && item.ActiveForKeyboard) || (_controlScheme == "Gamepad" && item.ActiveForGamepad));
                }
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public enum Language
    {
        ENG,
        PL
    };

    public Language _currentLanguage = Settings.Language.ENG;

    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            _currentLanguage = value;
            Label.LoadDictionary();
            foreach (LabelInitializer labelInit in GameController.Instance.GetComponentsInChildren<LabelInitializer>(true))
            {
                labelInit.LoadLabel();
            }
            if(SceneManager.GetActiveScene().name == "StartScreen") {
                foreach (LabelInitializer labelInit in Utils.GetSceneRootObject("Start Screen").GetComponentsInChildren<LabelInitializer>(true))
                {
                    labelInit.LoadLabel();
                }
                foreach (LabelInitializer labelInit in Utils.GetSceneRootObject("First-time Launch").GetComponentsInChildren<LabelInitializer>(true))
                {
                    labelInit.LoadLabel();
                }
            }
            MenuManager.Objects.OptionsLanguageDropdown.value = _currentLanguage == Language.ENG ? 0 : 1;
            if(MenuManager.Objects.OptionsLanguageLabel != null)
            {   
                MenuManager.Objects.OptionsLanguageLabel.SetLabel(_currentLanguage == Language.ENG ? "{LanguageEnglish}" : "{LanguagePolish}");
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _inWorldDialogueBubbleSpeed = 1f;

    public float InWorldDialogueBubbleSpeed
    {
        get => _inWorldDialogueBubbleSpeed;
        set
        {
            MenuManager.Objects.OptionsInWorldDialogueSpeedSlider.value = value * 100;
            MenuManager.Objects.OptionsInWorldDialogueSpeedLabel.string_params = new List<string> {(value * 100).ToString()};
            MenuManager.Objects.OptionsInWorldDialogueSpeedLabel.LoadLabel();
            _inWorldDialogueBubbleSpeed = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _screenShake = 1f;

    public float ScreenShake
    {
        get => _screenShake;
        set
        {
            MenuManager.Objects.OptionsScreenShakeSlider.value = value * 100;
            MenuManager.Objects.OptionsScreenShakeLabel.string_params = new List<string> {(value * 100).ToString()};
            MenuManager.Objects.OptionsScreenShakeLabel.LoadLabel();
            _screenShake = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _dialogueTextSpeed = 1f;

    public float DialogueTextSpeed
    {
        get => _dialogueTextSpeed;
        set
        {
            MenuManager.Objects.OptionsDialogueTextSpeedSlider.value = value * 100;
            MenuManager.Objects.OptionsDialogueTextSpeedLabel.string_params = new List<string> {(value * 100).ToString()};
            MenuManager.Objects.OptionsDialogueTextSpeedLabel.LoadLabel();
            _dialogueTextSpeed = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _damageNumbersSize = 1f;

    public float DamageNumbersSize
    {
        get => _damageNumbersSize;
        set
        {
            MenuManager.Objects.OptionsDamageNumbersSizeSlider.value = value * 100;
            MenuManager.Objects.OptionsDamageNumbersSizeLabel.string_params = new List<string> {(value * 100).ToString()};
            MenuManager.Objects.OptionsDamageNumbersSizeLabel.LoadLabel();
            _damageNumbersSize = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _uiSize = 1f;

    public float UISize
    {
        get => _uiSize;
        set
        {
            MenuManager.Objects.OptionsUISizeSlider.value = value * 100;
            MenuManager.Objects.OptionsUISizeLabel.string_params = new List<string> {(value * 100).ToString()};
            MenuManager.Objects.OptionsUISizeLabel.LoadLabel();
            _uiSize = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
            if((ShowExtraInfoInUI && UIManager.Instance.transform.Find("Extra Info") == null) || (!ShowExtraInfoInUI && UIManager.Instance.transform.Find("Pause Screen/Extra Info") == null)) {
                GameController.Instance.WaitAndRunMethod(0.01f, RefreshUISize);
            }
            else {
                foreach(GameObject uiElement in new List<GameObject> {UIManager.Objects.ExperienceBarSlider.gameObject, UIManager.Objects.Effects, UIManager.Objects.ResourceBars, UIManager.Objects.MissionInfo, UIManager.Objects.Notifications, UIManager.Objects.StanceDisplayKeyboard, UIManager.Objects.StanceDisplayGamepad, UIManager.Objects.ChargeBarSlider.gameObject, UIManager.Objects.CustomGaugeSlider.gameObject, UIManager.Objects.InteractIndicatorText.gameObject, UIManager.Objects.Timer, UIManager.Objects.InCombatIndicator}) {
                    uiElement.transform.localScale = new Vector3(_uiSize, _uiSize, _uiSize);
                }
            }
        }
    }

    private void RefreshUISize() {
        UISize = UISize;
    }

    public float _masterVolume = 0.5f;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            AudioListener.volume = value;
            MenuManager.Objects.OptionsMasterVolumeSlider.SetValueWithoutNotify(value * 100);
            MenuManager.Objects.OptionsMasterVolumeLabel.SetLabel("{MasterVolume}: " + value * 100);
            _masterVolume = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public bool SkipPrologue = false;
    public bool ConsoleEnabled = false;

    public float _musicVolume = 0.5f;

    public float MusicVolume {
        get => _musicVolume;
        set {
            GameController.Objects.Music.volume = value * 0.15f;
            MenuManager.Objects.OptionsMusicVolumeSlider.SetValueWithoutNotify(value * 100);
            MenuManager.Objects.OptionsMusicVolumeLabel.SetLabel("{MusicVolume}: " + value * 100);
            _musicVolume = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public bool _showExtraInfoInUI = true;
    public bool ShowExtraInfoInUI {
        get => _showExtraInfoInUI;
        set {
            _showExtraInfoInUI = value;
            MenuManager.Objects.ShowExtraInfoInUIToggle.GetComponent<Toggle>().isOn = value;
            if(_showExtraInfoInUI && UIManager.Instance.transform.Find("Extra Info") == null) {
                UIManager.Instance.transform.Find("Pause Screen/Extra Info").transform.SetParent(UIManager.Instance.transform);
            }
            else if(!_showExtraInfoInUI && UIManager.Instance.transform.Find("Pause Screen/Extra Info") == null) {
                UIManager.Instance.transform.Find("Extra Info").transform.SetParent(UIManager.Instance.transform.Find("Pause Screen"));
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _soundVolume = 0.5f;

    public float SoundVolume
    {
        get => _soundVolume;
        set
        {
            MenuManager.Objects.OptionsSoundVolumeSlider.SetValueWithoutNotify(value * 100);
            MenuManager.Objects.OptionsSoundVolumeLabel.SetLabel("{SoundVolume}: " + value * 100);
            _soundVolume = value;
            if(Area.ComponentInstance != null && Area.ComponentInstance.AudioSourceOriginalVolumes != null) {
                foreach(AudioSource audioSource in Area.ComponentInstance.AudioSourceOriginalVolumes.Keys) {
                    if(audioSource != null && audioSource.gameObject != null && audioSource.gameObject.IsDestroyed() == false) {
                        audioSource.volume = Area.ComponentInstance.AudioSourceOriginalVolumes[audioSource] * _soundVolume;
                    }
                }
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _dialogueVolume = 0.5f;

    public float DialogueVolume
    {
        get => _dialogueVolume;
        set
        {
            MenuManager.Objects.OptionsDialogueVolumeSlider.SetValueWithoutNotify(value * 100);
            MenuManager.Objects.OptionsDialogueVolumeLabel.SetLabel("{DialogueVolume}: " + value * 100);
            GameController.Objects.AudioListener.volume = value * 0.35f;
            _dialogueVolume = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _fieldOfView = 20;

    public float FieldOfView
    {
        get => _fieldOfView;
        set
        {
            MenuManager.Objects.OptionsFieldOfViewSlider.value = value;
            _fieldOfView = value;
            float scaled_value = 2 + (value <= 20 ? value / 10 : 2 + (value - 20) / 5);
            if(Player.Instance != null)
            {
                CameraController.Instance.Camera.orthographicSize = scaled_value;
                Player.Instance.transform.Find("Target Detection").localScale = new Vector3(Utils.GetValueBasedOnMinAndMax(scaled_value, 2, 8, 5.7f, 22.7f), Utils.GetValueBasedOnMinAndMax(scaled_value, 2, 8, 3.2f, 12.7f));
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public List<Keybind> Keybinds = new List<Keybind>();

    [Serializable]
    public class Keybind {
        public string ActionName;
        public string KeyboardBinding1;
        public string KeyboardBinding2;
        public string GamepadBinding1;
        public string GamepadBinding2;
    }

    string saveFile;

    public void Save()  
    {
        string jsonString = JsonUtility.ToJson(Instance, true);
        File.WriteAllText(saveFile, jsonString);
    }

    public bool Load()
    {
        if (File.Exists(saveFile))
        {
            string fileContents = File.ReadAllText(saveFile);
            Settings.Instance = JsonUtility.FromJson<Settings>(fileContents);
            foreach (PropertyInfo field in Settings.Instance.GetType().GetProperties())
            {
                try {
                    field.SetValue(Settings.Instance, field.GetValue(Settings.Instance));
                }
                catch (Exception e) {
                    Debug.LogError($"Error while loading settings field {field.Name}: {e.Message}\nInner exception:{e.InnerException}\nStack trace:{e.StackTrace}\nStack trace 2: {Utils.GetStackTrace()}");
                }
            }
            return true;
        }
        else
        {
            Utils.CreateAuditLog("Creating new settings file");
            return false;
        }
    }
}