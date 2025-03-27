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

    public string GameVersion = Application.version;
    public bool CreateAuditLogs = true;
    public bool _allowSkipUnreadDialogue = true;
    public bool AllowSkipUnreadDialogue {
        get => _allowSkipUnreadDialogue;
        set {
            _allowSkipUnreadDialogue = value;
            MenuManager.Instance.transform.Find("Settings Window/Gameplay/Items/SkipText").GetComponent<Toggle>().isOn = value;
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
            MenuManager.Instance.transform.Find("Settings Window/Gameplay/Items/AutoSkip").GetComponent<Toggle>().isOn = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public Constants.Difficulty _defaultDifficulty = Constants.Difficulty.Regular;

    public Constants.Difficulty DefaultDifficulty
    {
        get => _defaultDifficulty;
        set
        {
            _defaultDifficulty = value;
            TMP_Dropdown difficulty = Utils.GetSceneRootObject("Start Screen").Find("Screen/Options/Difficulty/Dropdown").GetComponent<TMP_Dropdown>();
            difficulty.value = value == Constants.Difficulty.Story ? 0 : value == Difficulty.Regular ? 1 : value == Difficulty.Challenge ? 2 : 3;
            if(GameController.Instance.AutoSaveSettings)
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
                CanvasElements.UICanvas.StanceDisplayKeyboard.GetComponent<CanvasGroup>().alpha = 0;
                CanvasElements.UICanvas.StanceDisplayGamepad.GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (value == "Keyboard") {
                CanvasElements.UICanvas.StanceDisplayKeyboard.GetComponent<CanvasGroup>().alpha = 1;
                CanvasElements.UICanvas.StanceDisplayGamepad.GetComponent<CanvasGroup>().alpha = 0;
            }
            _controlScheme = value;
            TMP_Dropdown controls = Utils.GetSceneRootObject("Start Screen").Find("Screen/Options/Controls/Dropdown").GetComponent<TMP_Dropdown>();
            controls.value = value == "Keyboard" ? 0 : 1;
            if(Player.Instance != null && SaveFile.Instance.Stances != null)
            {
                foreach (Stance stance in SaveFile.Instance.Stances)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        stance.Abilities[i].Reload();
                    }
                    if(GameController.Instance.transform.Find("UI Canvas/Stance Display " + Settings.Instance.ControlScheme + "/Stances").childCount == 3) {
                        stance.UIStanceDisplay = GameController.Instance.transform.Find("UI Canvas/Stance Display " + Settings.Instance.ControlScheme + "/Stances/UI_" + stance.WeaponCategory + "StanceDisplay").transform;
                        stance.StanceCooldownDisplay = GameController.Instance.transform.Find("UI Canvas/Stance Display " + Settings.Instance.ControlScheme + "/Stances/UI_" + stance.WeaponCategory + "StanceDisplay/Cooldown").GetComponent<Image>();
                    }
                }
            }
            foreach(Toggle toggle in CanvasElements.MenuCanvas.CategorySelection.GetComponentsInChildren<Toggle>(true))
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
            TMP_Dropdown language = Utils.GetSceneRootObject("Start Screen").Find("Screen/Options/Language/Dropdown").GetComponent<TMP_Dropdown>();
            language.value = _currentLanguage == Language.ENG ? 0 : 1;
            if(language.transform.Find("Label").GetComponent<LabelInitializer>() != null)
            {   
                language.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel(_currentLanguage == Language.ENG ? "{LanguageEnglish}" : "{LanguagePolish}");
            }
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    /*public int _inWorldDialogueBubbleSpeed = 10;

    public int InWorldDialogueBubbleSpeed
    {
        get => _inWorldDialogueBubbleSpeed;
        set
        {
            CanvasElements.InWorldDialogueSpeedSlider.GetComponent<Slider>().value = value;
            int val = (int)(value * 10);
            CanvasElements.InWorldDialogueSpeedSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{InGameDialogueSpeed}: " + val.ToString() + "%");
            _inWorldDialogueBubbleSpeed = value == 0 ? 1 : value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }*/

    public int _dialogueTextSpeed = 10;

    public int DialogueTextSpeed
    {
        get => _dialogueTextSpeed;
        set
        {
            CanvasElements.DialogueTextSpeedSlider.GetComponent<Slider>().value = value;
            int val = (int)(value * 10);
            CanvasElements.DialogueTextSpeedSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{DialogueTextSpeed}: " + val.ToString() + "%");
            _dialogueTextSpeed = value == 0 ? 1 : value;
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
            /*AudioListener.volume = value;
            CanvasElements.MasterVolumeSlider.GetComponent<Slider>().SetValueWithoutNotify(value * 100);
            CanvasElements.MasterVolumeSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{MasterVolume}: " + value * 100);*/
            _damageNumbersSize = value;
            if (GameController.Instance.AutoSaveSettings)
            {
                Save();
            }
        }
    }

    public float _masterVolume = 0.5f;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            AudioListener.volume = value;
            CanvasElements.MasterVolumeSlider.GetComponent<Slider>().SetValueWithoutNotify(value * 100);
            CanvasElements.MasterVolumeSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{MasterVolume}: " + value * 100);
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
            CanvasElements.Music.volume = value * 0.15f;
            CanvasElements.MusicVolumeSlider.GetComponent<Slider>().SetValueWithoutNotify(value * 100);
            CanvasElements.MusicVolumeSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{MusicVolume}: " + value * 100);
            _musicVolume = value;
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
            CanvasElements.SoundVolumeSlider.GetComponent<Slider>().SetValueWithoutNotify(value * 100);
            CanvasElements.SoundVolumeSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{SoundVolume}: " + value * 100);
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
            CanvasElements.DialogueVolumeSlider.GetComponent<Slider>().SetValueWithoutNotify(value * 100);
            CanvasElements.DialogueVolumeSlider.transform.Find("Label").GetComponent<LabelInitializer>().SetLabel("{DialogueVolume}: " + value * 100);
            CanvasElements.AudioListener.volume = value * 0.35f;
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
            CanvasElements.FieldOfViewSlider.GetComponent<Slider>().value = value;
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

    string saveFile;

    public void Save()  
    {
        var TypeBlob = typeof(Settings).GetFields().ToDictionary(x => x.Name, x => x.GetValue(this));
        string jsonString = JsonUtility.ToJson(Instance);
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