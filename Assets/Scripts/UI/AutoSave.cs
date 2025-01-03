#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
    private static DateTime _nextSaveTime;

    static AutoSave()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                return;
            }

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        };

        _nextSaveTime = DateTime.Now.AddMinutes(30);
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (_nextSaveTime > DateTime.Now)
        {
            return;
        }

        _nextSaveTime = _nextSaveTime.AddMinutes(30);
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }
}

#endif