using System;
using System.IO;
using UnityEngine;

public class ConsoleToGUI : MonoBehaviour
{
    string myLog = "*begin log";
    public static bool ShowOnScreenLogs = false;
    int kChars = 2200;
    void OnEnable() { Application.logMessageReceived += Log;  }
    void OnDisable() { Application.logMessageReceived -= Log; }
    public void Log(string logString, string stackTrace, LogType type)
    {
        myLog = myLog + "\n" + logString;
        if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }
    }

    void OnGUI()
    {
        if (!ShowOnScreenLogs) { return; }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
           new Vector3(1.5f, 1.5f, 1.5f));
        GUI.TextArea(new Rect(5, 5, 500, 700), myLog);
    }
}