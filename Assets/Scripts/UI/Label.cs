using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Label
{
    private static Dictionary<string, LabelItem> _labels;

    public static string Get(string label)
    {
        if (_labels == null)
        {
            LoadDictionary();
        }
        if (!_labels.ContainsKey(label))
        {
            Debug.LogError("Could not find label '" + label + "'.");
            return null;
        }
        return Settings.Instance.CurrentLanguage == Settings.Language.ENG ? Utils.InsertLabelsIntoText(_labels[label].ENG) : Settings.Instance.CurrentLanguage == Settings.Language.PL ? Utils.InsertLabelsIntoText(_labels[label].PL) : null;
    }

    public static bool ContainsKey(string label)
    {
        if (_labels == null)
        {
            LoadDictionary();
        }
        if (String.IsNullOrWhiteSpace(label))
        {
            return false;
        }
        return _labels.ContainsKey(label);
    }

    public static void LoadDictionary()
    {
        _labels = new Dictionary<string, LabelItem>();
        
        // Construct the path to StreamingAssets
        string filePath = Path.Combine(Application.streamingAssetsPath, "Label.csv");
        string fileContent = "";
        if (File.Exists(filePath))
        {
            // Use FileStream with FileShare.ReadWrite to ignore file locks from programs like Excel
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
            {
                fileContent = reader.ReadToEnd();
            }
        }
        else
        {
            Debug.LogError($"Could not find TSV file at path: {filePath}");
            return;
        }

        string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        int startIndex = 0;
        if (lines.Length > 0 && lines[0].StartsWith("Label\t"))
        {
            startIndex = 1;
        }

        for (int i = startIndex; i < lines.Length; i++)
        {
            // CHANGED: Use our new parser and tell it to look for a comma
            string[] columns = ParseCsvLine(lines[i], ','); 
            
            if (columns.Length >= 3)
            {
                LabelItem item = new LabelItem
                {
                    Label = columns[0].Trim(),
                    Category = columns[1].Trim(),
                    ENG = columns[2].Replace("\\n", "\n"),
                    PL = columns.Length >= 4 ? columns[3].Replace("\\n", "\n") : ""
                };

                if (!_labels.ContainsKey(item.Label))
                {
                    _labels.Add(item.Label, item);
                }
            }
        }
    }

    // A robust CSV parser that ignores delimiters hidden inside quotation marks.
    private static string[] ParseCsvLine(string line, char delimiter = ',')
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder currentField = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                // Handle escaped quotes (Excel saves double quotes as "")
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                {
                    currentField.Append('\"');
                    i++; // Skip the second quote
                }
                else
                {
                    inQuotes = !inQuotes; // Toggle whether we are safely inside quotes
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                // We hit a comma OUTSIDE of quotes. That means the field is done.
                result.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }
        
        result.Add(currentField.ToString()); // Add the final field
        return result.ToArray();
    }

    public class LabelItem
    {
        public string Label;
        public string Category;
        public string ENG;
        public string PL;
    }

    public static void SearchForUntranslatedLabels()
    {
        LoadDictionary();
        foreach (string key in _labels.Keys)
        {
            if (_labels[key].PL.Length > 20 && _labels[key].ENG.Length * 2f < _labels[key].PL.Length)
            {
                Debug.LogWarning($"Found English label ({_labels[key].ENG.Length}) smaller than Polish ({_labels[key].PL.Length}): {_labels[key].Label}");
            }
            else if (_labels[key].ENG.Length > 20 && _labels[key].PL.Length * 2f < _labels[key].ENG.Length)
            {
                Debug.LogWarning($"Found Polish label ({_labels[key].PL.Length}) smaller than English ({_labels[key].ENG.Length}): {_labels[key].Label}");
            }
            else if ((_labels[key].ENG.Contains("<b>") && !_labels[key].PL.Contains("<b>")) || (_labels[key].PL.Contains("<b>") && !_labels[key].ENG.Contains("<b>")) || (_labels[key].ENG.Contains("<i>") && !_labels[key].PL.Contains("<i>")) || (_labels[key].PL.Contains("<i>") && !_labels[key].ENG.Contains("<i>")) || (_labels[key].ENG.Count(x => x == '"') != _labels[key].PL.Count(x => x == '"')) || (_labels[key].PL.Count(x => x == '"') != _labels[key].ENG.Count(x => x == '"')))
            {
                Debug.LogWarning($"Found inconsistent special signs between English and Polish versions: {_labels[key].Label}");
            }
        }
    }

    public static int CheckWordCount(string lang)
    {
        LoadDictionary();
        int count = 0;
        foreach (string key in _labels.Keys)
        {
            if (_labels[key].Category == "Dialogue" && (lang == "ENG" ? _labels[key].ENG.Length > 0 : lang == "PL" ? _labels[key].PL.Length > 0 : false))
            {
                count += lang == "ENG" ? _labels[key].ENG.Count(w => w == ' ') : lang == "PL" ? _labels[key].PL.Count(w => w == ' ') : 0;
            }
        }
        return count;
    }

    public static void ConvertJSON()
    {

    }
}