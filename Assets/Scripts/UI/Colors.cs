using System.Globalization;
using System;
using UnityEngine;
using System.Collections.Generic;

public static class Colors {

    public static List<string> NoneEnergy = new() {"#2721BC", "#0B0690", "#604CFF", "#8F88FF"};
    public static List<string> AnimaEnergy = new() {"#80D980", "#1C3F11", "#73AB49", "#B1D9AB"};
    public static List<string> IgnisEnergy = new() {"#D61123", "#4F0613", "#FF3E3B", "#951C19"};
    public static List<string> GlaciesEnergy = new() {"#30CDD9", "#05626A", "#44AFC0", "#169F90"};
    public static List<string> MolisEnergy = new() {"#FF970B", "#6A460D", "#E09D3E", "#A66100"};
    public static List<string> SalutisEnergy = new() {"#C0C0C0", "#222121", "#E9E9E9", "#E9E9E9"};
    public static List<string> TonitruiEnergy = new() {"#FADE19", "#785A00", "#B7911E", "#FFEC96"};
    public static List<string> PropriusEnergy = new() {"#7B00EE", "#18003C", "#690DFF", "#B04FFA"};
    public static string Proprius = "#2F197B";
    public static string Ignis = "#B40020";
    public static string Tonitrui = "#ffcc00";
    public static string Aevitas = "#000000";
    public static string Rego = "#FF6400";
    public static string Emendavi = "#005500";
    public static string AlacritasSepire = "#ac9d93";

    public static Color UnlockedTile = new Color(179/255f, 152/255f, 79/255f);

    public static Color SelectedColor = new Color(118/255f, 139/255f, 216/255f);
    public static string Health = "#ff0000";
    public static Color HealthColor = new Color(0.77f, 0.05f, 0);
    public static string Barrier = "#92A3A6";
    public static string Energy = "#0073FF";
    public static string StaggerBar = "8000E6";
    public static Color UISelected = new Color(0.46f, 0.54f, 0.84f);
    public static Color StaggerColor = new Color(0.6590239f, 0.2311321f, 1f);
    public static string Staggered = "#6D2E8C";
    public static Color StaggeredColor = new Color(1f, 0.475f, 0);
    public static string Analysis = "#6D2E8C";
    public static string Broken = "#FF6400";
    public static string Influence = "#FF6400";
    public static Color InfluenceColor = new Color(1, 0.392f, 0);
    public static string Protection = "#92A3A6";
    public static string Enhancement = "#008205";
    public static string Emission = "#FFF600";
    public static string WeaponDamage = "#A13F42";
    public static string AttackSpeed = "#FFFDCE";
    public static string CastSpeed = "#B5FF94";
    public static string MovementSpeed = "#B5FF94";
    public static string Tenacity = "#B5FF94";
    public static Color OutOfCombat = new Color(0.66f, 1, 0.55f);
    public static string ItemGradeRegular = "#676767";
    public static string ItemGradeExcellent = "#0009CF";
    public static string ItemGradeMasterful = "#F30000";
    public static string ItemGradeFlawless = "#2E124B";
    public static string ItemGradeUltimate = "#FFF89C";
    public static string ItemGradeRegularDarker = "#878787";
    public static string ItemGradeExcellentDarker = "#323F8C";
    public static string ItemGradeMasterfulDarker = "#721414";
    public static string ItemGradeFlawlessDarker = "#231838";
    public static string ItemGradeUltimateDarker = "#989762";

    public static Color EquipmentTileBackground = new Color(0.114142f, 0.1246679f, 0.1415094f);
    public static Color EquipmentTileColor = new Color(0.1762193f, 0.1962976f, 0.2264151f);
    public static Color EquipmentTileBackgroundSelected = new Color(0.25178f, 0.3298582f, 0.4339623f);
    public static Color EquipmentTileColorSelected = new Color(0.499911f, 0.6051702f, 0.7735849f);

    public static Color AbilityEnoughEnergyColor = new Color(1, 1, 1);
    public static Color AbilityEnoughEnergyBorderColor = new Color(1, 1, 1);
    public static Color AbilityNotEnoughEnergyColor = new Color(0.792f, 0.317f, 0.327f);
    public static Color AbilityNotEnoughEnergyBorderColor = new Color(0.509f, 0.122f, 0.137f);

    public static string GradeD = "#00A357";
    public static string GradeC = "#FFFFFF";
    public static string GradeB = "#875500";
    public static string GradeA = "#ADADAD";
    public static string GradeS = "#FFED36";

    public static string Red = "#B7141F";
    public static string Blue = "#1447B7";
    public static Color Gold = GetColorFromCode("#B3984F");
    public static string Silver = "#737373";
    public static string Bronze = "#AD7300";
    public static string Black = "#000000";
    public static string White = "#ffffff";

    public static string Longsword = "#00FCFF";
    public static string Greatsword = "#B40020";
    public static string Blade = "#FFFDCE";
    public static string Spear = "#FF6400";
    public static string Dagger = "#B5FF94";
    public static string Rapier = "#008205";
    public static string Katana = "#FF66D5";
    public static string BareHanded = "#92A3A6";
    public static string Shield = "#932D00";
    public static string Bow = "#FFF600";
    public static string Gun = "#0064DD";
    public static string ESPConductor = "#6D2E8C";

    public static string IgnisFamily = "#96191E";
    public static string AnimaFamily = "#2A7E12";
    public static string GlaciesFamily = "#4982D2";
    public static string MolisFamily = "#D48341";
    public static string SalutisFamily = "#B9B9B9";
    public static string TonitruiFamily = "#CFCD6D";
    public static string PropriusFamily = "#2F197B";

    /// <summary>
    /// Takes in a 7-character code starting with '#' and followed by 3 pairs of 16-bit numbers.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static Color GetColorFromCode(string code)
    {
        return new Color(Convert.ToInt32(code.Substring(1, 2), 16) / 255f, Convert.ToInt32(code.Substring(3, 2), 16) / 255f, Convert.ToInt32(code.Substring(5, 2), 16) / 255f);
    }

    public static Color GetFamilyColor(string family_name) {
        switch(family_name) {
            case "Ignis": return GetColorFromCode(IgnisFamily);
            case "Anima": return GetColorFromCode(AnimaFamily);
            case "Glacies": return GetColorFromCode(GlaciesFamily);
            case "Molis": return GetColorFromCode(MolisFamily);
            case "Salutis": return GetColorFromCode(SalutisFamily);
            case "Tonitrui": return GetColorFromCode(TonitruiFamily);
            case "Proprius": return GetColorFromCode(PropriusFamily);
            case "None": return Color.gray;
            default: return Color.black;
        }
    }
}