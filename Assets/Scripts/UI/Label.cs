using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Label {

    private static Dictionary<string, LabelItem> _labels;

    public static string Get(string label) {
        if (_labels == null) {
            LoadDictionary();
        }
        if (!_labels.ContainsKey(label)) {
            Debug.LogError("Could not find label '" + label + "'.");
            return null;
        }
        return Settings.Instance.CurrentLanguage == Settings.Language.ENG ? Utils.InsertLabelsIntoText(_labels[label].ENG) : Settings.Instance.CurrentLanguage == Settings.Language.PL ? Utils.InsertLabelsIntoText(_labels[label].PL) : null;
    }

    public static bool ContainsKey(string label) {
        if (_labels == null) {
            LoadDictionary();
        }
        if(String.IsNullOrWhiteSpace(label)) {
            return false;
        }
        return _labels.ContainsKey(label);
    }

    public static void LoadDictionary() {
        _labels = new Dictionary<string, LabelItem>();
        TextAsset dictionary = Resources.Load("Label", typeof(TextAsset)) as TextAsset;
        LabelItem[] labels = JsonConvert.DeserializeObject<LabelItem[]>(dictionary.text);
        foreach (LabelItem label in labels) {
            _labels.Add(label.Label, label);
        }
    }

    public class LabelItem
    {
        public string Label;
        public string Category;
        public string ENG;
        public string PL;
    }

    public static void SearchForUntranslatedLabels() {
        LoadDictionary();
        foreach (string key in _labels.Keys) {
            if(_labels[key].PL.Length > 20 && _labels[key].ENG.Length * 2f < _labels[key].PL.Length) {
                Debug.LogWarning($"Found English label ({_labels[key].ENG.Length}) smaller than Polish ({_labels[key].PL.Length}): {_labels[key].Label}");
            }
            else if(_labels[key].ENG.Length > 20 && _labels[key].PL.Length * 2f < _labels[key].ENG.Length) {
                Debug.LogWarning($"Found Polish label ({_labels[key].PL.Length}) smaller than English ({_labels[key].ENG.Length}): {_labels[key].Label}");
            }
            else if((_labels[key].ENG.Contains("<b>") && !_labels[key].PL.Contains("<b>")) || (_labels[key].PL.Contains("<b>") && !_labels[key].ENG.Contains("<b>")) || (_labels[key].ENG.Contains("<i>") && !_labels[key].PL.Contains("<i>")) || (_labels[key].PL.Contains("<i>") && !_labels[key].ENG.Contains("<i>")) || (_labels[key].ENG.Count(x => x == '"') != _labels[key].PL.Count(x => x == '"')) || (_labels[key].PL.Count(x => x == '"') != _labels[key].ENG.Count(x => x == '"'))) {
                Debug.LogWarning($"Found inconsistent special signs between English and Polish versions: {_labels[key].Label}");
            }
        }
    }

    public static int CheckWordCount(string lang) {
        LoadDictionary();
        int count = 0;
        foreach (string key in _labels.Keys) {
            if(_labels[key].Category == "Dialogue" && (lang == "ENG" ? _labels[key].ENG.Length > 0 : lang == "PL" ? _labels[key].PL.Length > 0 : false)) {
                count += lang == "ENG" ? _labels[key].ENG.Count(w => w == ' ') : lang == "PL" ? _labels[key].PL.Count(w => w == ' ') : 0;
            }
        }
        return count;
    }
}