using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book
{
    public static Dialogue History() {
        return new Dialogue ("Book", "History", new List<DialogueLine>{
        new ("Book_History_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_20") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_40") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_50") {SpeakerPortrait="Book", SpeakerName="Name_Book_History"},
        new ("Book_History_60") {Speaker = "Player"}
    });}

    public static Dialogue LeoProprius() {
        
        return new Dialogue ("Book", "LeoProprius", new List<DialogueLine>{
        new ("Book_LeoProprius_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_20") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_40") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_50") {SpeakerPortrait="Book", SpeakerName="Name_Book_LeoProprius"},
        new ("Book_LeoProprius_60") {Speaker = "Player"}
    });}

    public static Dialogue TrueSpeech() {
        return new Dialogue ("Book", "TrueSpeech", new List<DialogueLine>{
        new ("Book_TrueSpeech_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_20") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_40") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_50") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_60") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_70") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"},
        new ("Book_TrueSpeech_80") {SpeakerPortrait="Book", SpeakerName="Name_Book_TrueSpeech"}
    });}

    public static Dialogue Energy() {
        return new Dialogue ("Book", "Energy", new List<DialogueLine>{
        new ("Book_Energy_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_20") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_40") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_50") {Speaker = "Player"},
        new ("Book_Energy_60") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_70") {Speaker = "Player"},
        new ("Book_Energy_80") {SpeakerPortrait="Book", SpeakerName="Name_Book_Energy"},
        new ("Book_Energy_90") {Speaker = "Player"},
    });}
    public static Dialogue UniquePropertiesOfEnergy() {
        return new Dialogue ("Book", "UniquePropertiesOfEnergy", new List<DialogueLine>{
        new ("Book_UniquePropertiesOfEnergy_0") {},
        new ("Book_UniquePropertiesOfEnergy_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_UniquePropertiesOfEnergy"},
        new ("Book_UniquePropertiesOfEnergy_20") {Speaker = "Player"},
    });}

    public static void OnEnd_UniquePropertiesOfEnergy() {
        if(SaveFile.Instance.HasFlag("IgnisManor_ReadImportantBook") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(10);
        }
        SaveFile.Instance.AddFlag("IgnisManor_ReadImportantBook");
    }

    public static Dialogue TheFirstKnight() {
        return new Dialogue ("Book", "TheFirstKnight", new List<DialogueLine>{
        new ("Book_TheFirstKnight_0") {},
        new ("Book_TheFirstKnight_10") {Speaker = "Player"},
        new ("Book_TheFirstKnight_20") {Speaker = "Player"}
    });}

    public static void OnEnd_TheFirstKnight() {
        if(SaveFile.Instance.HasFlag("IgnisManor_ReadNotImportantBook") == false) {
            SaveFile.Instance.ChangeIgnisEnergy(2);
        }
        SaveFile.Instance.AddFlag("IgnisManor_ReadNotImportantBook");
    }


    public static Dialogue NobleFamilies() {
        return new Dialogue ("Book", "NobleFamilies", new List<DialogueLine>{
        new ("Book_NobleFamilies_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_20") {Speaker = "Player"},
        new ("Book_NobleFamilies_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_40") {Speaker = "Player"},
        new ("Book_NobleFamilies_50") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_60") {Speaker = "Player"},
        new ("Book_NobleFamilies_70") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_80") {Speaker = "Player"},
        new ("Book_NobleFamilies_90") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_100") {Speaker = "Player"},
        new ("Book_NobleFamilies_110") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_120") {Speaker = "Player"},
        new ("Book_NobleFamilies_130") {SpeakerPortrait="Book", SpeakerName="Name_Book_NobleFamilies"},
        new ("Book_NobleFamilies_140") {Speaker = "Player"}
    });}

    public static Dialogue Awakened() {
        return new Dialogue ("Book", "Awakened", new List<DialogueLine>{
        new ("Book_Awakened_0") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_10") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_20") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_30") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_40") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_50") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_60") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_70") {Speaker = "Player"},
        new ("Book_Awakened_80") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_90") {SpeakerPortrait="Book", SpeakerName="Name_Book_Awakened"},
        new ("Book_Awakened_100") {Speaker = "Player"}
    });}

    public static Dialogue QuartermastersNotes() {
        return new Dialogue ("Book", "QuartermastersNotes", new List<DialogueLine>{
        new ("IgnisManor_QuartermastersNotes_0") {},
        new ("IgnisManor_QuartermastersNotes_10") {},
        new ("IgnisManor_QuartermastersNotes_20") {Animation="ShoulderShrug", SpeakerName = "Player", SpeakerPortrait = "Player"}
    });}

    public static Dialogue LaurasNotes() {
        return new Dialogue ("Book", "LaurasNotes", new List<DialogueLine>{
        new ("Book_LaurasNotes_0") {},
        new ("Book_LaurasNotes_10") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_20") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_30") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_40") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_50") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_60") {SpeakerName = "Laura", SpeakerPortrait = "Laura", SpeakerIsMale = false},
        new ("Book_LaurasNotes_70"),
        new ("Book_LaurasNotes_80")
    });}
}
