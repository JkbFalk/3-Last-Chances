using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class DebugController : MonoBehaviour
{
    public static DebugController Instance {
        get {
            return UIManager.Objects.DebugConsole.GetComponent<DebugController>();
        }
    }
    public string NotificationToBeSent;
    public static List<string> ConsoleHistory = new List<string>();
    public static bool ConsoleOpen = false;
    public static string ConsoleText = "";
    public static List<DebugCommand> Commands;
    public static float TimeScaleBeforeDebug = 1;

    public void Start()
    {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        Commands = new List<DebugCommand>()
        {
            new DebugCommand("info", "ShowDebugConsoleInfo", "Display all available commands in the debug console", typeof(int)),
            new DebugCommand("win", "DefeatAllEnemies", "Defeat all enemies in current area.", typeof(int)),
            new DebugCommand("lose", "LowerTo1H", "Lower Player's current Health to 1.", typeof(int)),
            new DebugCommand("finish", "FinishCurrentMission", "Finish current mission (only gives end-of-mission rewards).", typeof(int)),
            new DebugCommand("cheat", "AllowToToggleCheatMode", "Allows to enter cheat mode by holding shift on the keyboard. 'cheat 0' = can't toggle, 'cheat 1' = allow toggle", typeof(int), "Cheat mode ", true),
            new DebugCommand("speed", "ToggleSpeedMode", "Set all units attack speed to max (300%). 'speed 0' = off, 'speed 1' = on", typeof(int), "Maximum speed ", true),
            new DebugCommand("spam", "ToggleSpamMode", "Give infinite cdr and energy. 'spam 0' = off, 'spam 1' = on", typeof(int), "Ability spam ", true),
            new DebugCommand("unstuck", "Unstuck", "Reset the player character and move forward (posibly clipping through terrain)", typeof(int)),
            new DebugCommand("effects", "CheckPlayerEffects", "Display player's current effects and durations", typeof(int)),
            new DebugCommand("find", "FindScripts", "Find scripts in Unity Scene (for dev purposes)", typeof(string)),
            new DebugCommand("cursor", "ToggleCursor", "Turn cursor on or off.", typeof(int)),
            new DebugCommand("save", "Save", "Make mid-mission save. May not work properly.", typeof(int)),
            new DebugCommand("load", "Load", "Load previously made mid-mission save. May not work properly", typeof(int)),
            new DebugCommand("fps", "ToggleFPSCounter", "Turn fps display on or off.", typeof(int)),
            new DebugCommand("lvl_up", "IncreaseLevel", "Increase player level by specified amount ('lvl_up 10' gives 10 levels).", typeof(int)),
            new DebugCommand("money", "ChangeMoney", "Change current money amount. 'money 1000' = set money to 1000", typeof(int)),
            new DebugCommand("ammo", "ChangeAmmo", "Change current ammo amount. 'ammo 99' = set ammo to 99", typeof(int)),
            new DebugCommand("give_all", "GiveAllItems", "Acquire all items available in the game in all 5 rarity versions", typeof(int)),
            new DebugCommand("debug", "ToggleWorldspaceDebug", "Show debug info on each unit. 'debug 0' = off, 'debug 1' = on", typeof(int), "Visible debug ", true)
        };
    }

    public void InputDebugConsoleCommand(string custom_command = null)
    {
        string command = custom_command != null ? custom_command : UIManager.Instance.transform.Find("Debug Console/Text Area/Text").GetComponent<TextMeshProUGUI>().text;
        bool command_was_executed = false;
        bool info_command_executed = false;
        foreach (DebugCommand item in Commands)
        {
            if(command.Contains(item.Name))
            {
                if(item.Name == "info" || item.Name == "effects")
                {
                    info_command_executed = true;
                }
                MethodInfo methodToExecute = GetType().GetMethod(item.FunctionToExecute);
                if (methodToExecute != null)
                {
                    NotificationToBeSent = item.Notification;
                    string param_string = new string(command.Where(c => char.IsDigit(c)).ToArray());
                    if(item.ParameterType == typeof(int))
                    {
                        int parameter = String.IsNullOrEmpty(param_string) ? 0 : Int32.Parse(param_string);
                        methodToExecute.Invoke(this, new object[] { parameter });
                    }
                    else
                    {
                        string formatted = new string(command.Split(' ')[1].Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
                        methodToExecute.Invoke(this, new object[] { formatted });
                    }
                    AddDebugConsoleHistoryEntry(command);
                    command_was_executed = true;
                    if(string.IsNullOrEmpty(NotificationToBeSent) == false)
                    {
                        if(item.ParameterType == typeof(int) && item.AddEnabledOrDisabledToNotificationDependingOnParam)
                        {
                            int parameter = String.IsNullOrEmpty(param_string) ? 0 : Int32.Parse(param_string);
                            NotificationToBeSent += parameter != 0 ? "enabled" : "disabled";
                        }
                        NotificationController.ShowTextNotification(NotificationToBeSent);
                    }
                }
            }
        }
        if(command_was_executed == false)
        {
            AddDebugConsoleHistoryEntry("Command does not exist: " + command);
            UIManager.Objects.DebugConsole.GetComponent<TMP_InputField>().ActivateInputField();
        }
        else if(info_command_executed == false)
        {
            CloseDebugConsole();
        }
        else
        {
            UIManager.Objects.DebugConsole.GetComponent<TMP_InputField>().ActivateInputField();
        }
        UIManager.Objects.DebugConsole.GetComponent<TMP_InputField>().text = "";
        ConsoleText = "";
    }

    public static void CloseDebugConsole()
    {
        Time.timeScale = DebugController.TimeScaleBeforeDebug;
        GameController.Instance.PlayerInput.SwitchCurrentActionMap("Regular");
        UIManager.Objects.DebugConsole.gameObject.SetActive(false);
        UIManager.Objects.DebugConsole.GetComponent<TMP_InputField>().text = "";
        DebugController.ConsoleText = "";
    }
    
    public void AddDebugConsoleHistoryEntry(string entry)
    {
        ConsoleHistory.Add(entry);
        string console_history = "";
        foreach (string history in ConsoleHistory)
        {
            console_history += history + "\n";
        }
        UIManager.Instance.transform.Find("Debug Console/Text Area/History/History Text").GetComponent<TextMeshProUGUI>().text = console_history;
    }

    public class DebugCommand
    {
        public string Name;
        public string FunctionToExecute;
        public string Description;
        public string Notification;
        public Type ParameterType;
        public bool AddEnabledOrDisabledToNotificationDependingOnParam;

        public DebugCommand(string name, string function_to_execute, string description, Type param_type, string notification_to_show = "", bool add_enabled_or_disabled_to_notification_depending_on_param = false) {
            this.Name = name;
            FunctionToExecute = function_to_execute;
            ParameterType = param_type;
            this.Description = description;
            Notification = notification_to_show;
            AddEnabledOrDisabledToNotificationDependingOnParam = add_enabled_or_disabled_to_notification_depending_on_param;
        }

    }

    public void ShowDebugConsoleInfo(int param)
    {
        string debug_info = "";
        foreach (DebugCommand command in Commands)
        {
            debug_info += command.Name + ": " + command.Description + "\n";
        }
        AddDebugConsoleHistoryEntry(debug_info);
    }

    public void Unstuck(int param)
    {
        Player.Instance.transform.position += Player.Instance.Actions.IsFlipped ? Vector3.left * 3 : Vector3.right * 3;
        Player.Instance.Actions.CurrentActionBeingPerformed = Constants.ActionType.Idle;
        Player.Instance.Actions.CurrentAbilityBeingPerformed = null;
        Player.Instance.EndEffect(typeof(Effect_Invincible));
        Player.Instance.EndEffect(typeof(Effect_RootedInPlace));
        Player.Instance.EndEffect(typeof(Effect_Block));
    }

    public bool CheatModeCanBeToggled = true;

    private bool _cheatModeActive = false;
    private Effect_ChangeCompositeStat superInjury;
    private Effect_ChangeCompositeStat superAS;
    private Effect_ChangeStat superDef;
    private Effect_ChangeStat superRes;
    private Effect_ChangeStat superSpeed;

    public void AllowToToggleCheatMode(int param) {
        CheatModeCanBeToggled = param != 0;
    }

    public void ToggleCheatMode(int turned_on)
    {
        _cheatModeActive = CheatModeCanBeToggled ? turned_on != 0 : false;
        RefreshCheatMode();
        EventManager.PlayerObjectReinitialized.AddListener(RefreshCheatMode);
    }

    public void RefreshCheatMode() {
        if(_cheatModeActive) {
            foreach(Effect e in new List<Effect> {superInjury, superAS, superDef, superRes, superSpeed}) {
                if(e != null && !e.EffectEnded) {
                    e.EndThisEffect();
                }
            }
            superInjury = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, new("Cheat")) {
                IsRemovable = false, 
                PercentageModifier = 100000
            };
            superAS = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new("Cheat")) {
                IsRemovable = false,
                PercentageModifier = 100
            };
            superDef = new Effect_ChangeStat(Player.Instance.Armor, new("Cheat")) {
                IsRemovable = false, 
                PercentageAmount = 5000
            };
            superRes = new Effect_ChangeStat(Player.Instance.Tenacity, new("Cheat")) {
                IsRemovable = false, 
                PercentageAmount = 1000
            };
            superSpeed = new Effect_ChangeStat(Player.Instance.MovementSpeed, new("Cheat")) {
                IsRemovable = false, 
                PercentageAmount = 250
            };
            foreach(Effect e in new List<Effect> {superInjury, superAS, superDef, superRes, superSpeed}) {
                Player.Instance.AddEffect(e);
            }
        }
        else if(superInjury != null){
            superInjury.EndThisEffect();
            superAS.EndThisEffect();
            superDef.EndThisEffect();
            superRes.EndThisEffect();
            superSpeed.EndThisEffect();
        }
    }

    private static bool _worldSpaceDebugEnabled = false;

    public static bool WorldspaceDebugEnabled
    {
        get
        {
            return _worldSpaceDebugEnabled;
        }
        set
        {
            _worldSpaceDebugEnabled = value;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Debug"))
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    if (Player.Instance.gameObject == go.transform.root && go.transform.GetChild(i).name == "AI")
                    {
                        go.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        go.transform.GetChild(i).gameObject.SetActive(value);
                    }
                }
            }
        }
    }

    public void ToggleWorldspaceDebug(int turned_on)
    {
        WorldspaceDebugEnabled = turned_on != 0;
    }

    public static bool MaxAttackSpeed = false;

    public static List<Effect_ChangeCompositeStat> SpeedBuffs = new();
    public void ToggleSpeedMode(int turned_on)
    {
        MaxAttackSpeed = turned_on != 0;
        RefreshSpeedMode();
        EventManager.PlayerObjectReinitialized.AddListener(RefreshSpeedMode);
    }

    public void RefreshSpeedMode() {
        if(MaxAttackSpeed) {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Enemy")) {
                Effect_ChangeCompositeStat buff = new(go.GetComponent<Unit>(), Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new("Cheat")) {PercentageModifier = 1000};
                SpeedBuffs.Add(buff);
                go.GetComponent<Unit>().AddEffect(buff);
            }
            Effect_ChangeCompositeStat buff2 = new(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, new("Cheat")) {PercentageModifier = 1000};
            SpeedBuffs.Add(buff2);
            Player.Instance.AddEffect(buff2);
        }
        else {
            foreach(Effect_ChangeCompositeStat e in SpeedBuffs) {
                e.EndThisEffect();
            }
            SpeedBuffs.Clear();
        }
    }

    public static bool SpamModeEnabled = false;
    private Effect_ChangeStat superCDR;
    private Effect_ChangeStat superEG;

    public void ToggleSpamMode(int turned_on)
    {
        SpamModeEnabled = turned_on != 0;
        RefreshSpamMode();
        EventManager.PlayerObjectReinitialized.AddListener(RefreshSpamMode);
    }

    public void RefreshSpamMode() {
        if (SpamModeEnabled)
        {
            superCDR = new Effect_ChangeStat(Player.Instance.CooldownReduction, new("Cheat"))
            {
                IsRemovable = false,
                FlatAmount = 1000
            };
            superEG = new Effect_ChangeStat(Player.Instance.Energy, new("Cheat"))
            {
                IsRemovable = false,
                RegenerationFlatAmount = 1000
            };
            Debug.Log("QQ1: " + Player.Instance.Energy.Current + " , " + Player.Instance.CooldownReduction.Current);
            Player.Instance.AddEffect(superCDR);
            Player.Instance.AddEffect(superEG);
            Player.Instance.RemoveAllCooldowns();
            Debug.Log("QQ2: " + Player.Instance.Energy.Current + " , " + Player.Instance.CooldownReduction.Current);
        }
        else
        {
            superCDR.EndThisEffect();
            superEG.EndThisEffect();
        }
    }

    public void ChangeMoney(int money_amount)
    {
        SaveFile.Instance.Money = money_amount;
    }

    public void ChangeAmmo(int ammo_amount)
    {
        Player.Instance.Ammo = ammo_amount;
    }

    public void FindScripts(string script_name)
    {
        Type type = Type.GetType(script_name);
        MonoBehaviour[] list = FindObjectsOfType(type) as MonoBehaviour[];
        foreach (var t in list)
        {
            Debug.Log(Utils.GetGameObjectPath(t.gameObject));
        }
    }

    public void CheckPlayerEffects(int param)
    {
        string stat_list = "Current player effects:" + "\n";
        foreach (Effect e in Player.Instance.CurrentEffects.ToList())
        {
            stat_list += e.GetType().ToString() + ": " + e.ToString() + (e.BaseDuration == 0 ? " (Inf)" :  " (" + e.RemainingDuration + "/" + e.BaseDuration + " sec)") + "\n";
        }
        AddDebugConsoleHistoryEntry(stat_list);
    }

    public void ToggleCursor(int param)
    {
        Cursor.visible = param == 1;
    }

    public void DefeatAllEnemies(int param)
    {
        foreach(Unit unit in Utils.GetAllUnits(true, true))
        {
            for(int i = 0; i < unit.HealthBars.Count; i++) {
                Damage damage = new Damage(unit, new Ability_SourcelessDamage(Player.Instance), null).SetDamageSource(10 * unit.Health.Maximum / unit.GetComponent<Unit>().Armor.Current, 0);
                damage.CalculateAndApplyDamage();
            }
        }
    }

    public void LowerTo1H(int param)
    {
        Player.Instance.Health.Current = 1;
    }

    public void ToggleFPSCounter(int param)
    {
        UIManager.Objects.FPSCounter.SetActive(param == 1);
    }

    public void GiveAllItems(int param)
    {
        List<Item.ItemGrade> grades = new List<Item.ItemGrade>() { Item.ItemGrade.Regular, Item.ItemGrade.Excellent, Item.ItemGrade.Masterful, Item.ItemGrade.Flawless, Item.ItemGrade.Ultimate };
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item))))
        {
            foreach (Item.ItemGrade grade in grades)
            {
                Item found_item = SaveFile.Instance.Inventory.FirstOrDefault(item => item.GetType() == type && item.Grade == grade);
                if(found_item != null && (type == typeof(Quest_UpgradeMaterials)))
                {
                    found_item.Amount = found_item.MaxAmount;
                }
                else if (found_item == null)
                {
                    Item added_item = (Item)Activator.CreateInstance(type, new object[] { grade });
                    if(type == typeof(Quest_UpgradeMaterials) || type == typeof(Quest_ToolMaterials))
                    {
                        added_item.Amount = added_item.MaxAmount;
                    }
                    if(added_item.Type == Constants.ItemType.Tool) {
                        SaveFile.Instance.UnlockTool(type);
                        break;
                    }
                    else if(!(added_item is Quest_UpgradeMaterials) && added_item.Type == Constants.ItemType.Quest) {
                        added_item.Grade = Item.ItemGrade.None;
                        SaveFile.Instance.AddItem(added_item, false);
                        break;
                    }
                    else {
                        SaveFile.Instance.AddItem(added_item, false);
                    }
                }
            }
        }
    }

    public void FinishCurrentMission(int param) {
        if(SaveFile.Instance.CurrentMission != null) {
            SaveFile.Instance.CurrentMission.Finish();
        }
    }

    public void IncreaseLevel(int param) {
        for(int i = 0; i < param; i++) {
            SaveFile.Instance.ExperiencePoints += SaveFile.Instance.GetExperiencePointsNeededToLevelUp();
        }
    }
}
