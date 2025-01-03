using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
[InitializeOnLoad]
public class DebugTools : EditorWindow {
    public DebugTools() {
        EditorApplication.delayCall += ()=> SetPlayModeStartScene("Assets/Scenes/StartScreen.unity");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Force Initialize")) {
            SetPlayModeStartScene("Assets/Scenes/StartScreen.unity");
        }
        if (GUILayout.Button("Rename Duplicate GameObjects")) {
            RenameDuplicateGameObjects();
        }
        if (GUILayout.Button("Search For Untranslated Labels")) {
            Label.SearchForUntranslatedLabels();
        }
        /*if (GUILayout.Button("Equip Item On Player")) {
            Utils.CopyItemAppearanceForPlayer()
        }*/
    }

    void SetPlayModeStartScene(string scenePath)
    {
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (myWantedStartScene != null)
            EditorSceneManager.playModeStartScene = myWantedStartScene;
        else
            Debug.Log("Could not find Scene " + scenePath);
    }

    public Dictionary<GameObject, string> GameObjectsToRename;
    void RenameDuplicateGameObjects()
    {
        GameObjectsToRename = new();
        GatherGameObjects(Area.Instance.transform);
    }

    void GatherGameObjects(Transform target) {
        foreach(Transform child in target) {
            if(child.GetComponent<Unit>() == null) {
                GameObjectsToRename.Add(child.gameObject, Utils.GetGameObjectPath(child.gameObject));
            }
        }
        Dictionary<string, int> GameObjectNameToCount = new();
        foreach(GameObject go in GameObjectsToRename.Keys) {
            go.name = GetCleanedGameObjectName(go.name);
            string pathToGameObject = Utils.GetGameObjectPath(go.gameObject);
            if(go.name[go.name.Length - 1] == ' ') {
                go.name = go.name.Substring(0, go.name.Length - 1);
            }
            if(GameObjectNameToCount.ContainsKey(pathToGameObject) == false) {
                GameObjectNameToCount.Add(pathToGameObject, 0);
            }
            GameObjectNameToCount[pathToGameObject]++;
            go.name = go.name + (go.GetComponent<Unit>() == null && GameObjectNameToCount[pathToGameObject] == 1 ? "" : "_" + GameObjectNameToCount[pathToGameObject]);
        }
        foreach(Transform child in target) {
            if(child.GetComponent<Unit>() == null) {
                GatherGameObjects(child);
            }
        }
    }

    [MenuItem("Custom/Debug Tools")]
    static void Open()
    {
        GetWindow<DebugTools>();
    }

    private string GetCleanedGameObjectName(string name) {
        return name.Replace("_0", "").Replace("_1", "").Replace("_2", "").Replace("_3", "").Replace("_4", "").Replace("_5", "").Replace("_6", "").Replace("_7", "").Replace("_8", "").Replace("_9", "").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("()", "").Replace("  ", " ");
    }
}