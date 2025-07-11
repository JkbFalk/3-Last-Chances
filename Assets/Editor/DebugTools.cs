using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Serialization.EventEmitters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
[InitializeOnLoad]
public class DebugTools : EditorWindow {
    public DebugTools() {
        EditorApplication.delayCall += ()=> SetPlayModeStartScene("Assets/Scenes/StartScreen.unity");
    }
    public GameObject gameObjectToGetPath;
    public string pathToGameObject = "";
    private string _pathValue = "";

    
    [MenuItem("Tools/3LC Tools")]
    static void Open()
    {
        GetWindow<DebugTools>();
    }
    public static void Init() {
        DebugTools window = (DebugTools)GetWindow(typeof(DebugTools));
        window.Show();
        window.Initialize();
    }

    public void Initialize() {
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
        if (GUILayout.Button("Check Dialogue Word Count (ENG)")) {
            Debug.Log("ENG word count: " + Label.CheckWordCount("ENG"));
        }
        if (GUILayout.Button("Check Dialogue Word Count (PL)")) {
            Debug.Log("PL word count: " + Label.CheckWordCount("PL"));
        }
        if (GUILayout.Button("Get Path to GameObject as String")) {
            _pathValue = Utils.GetGameObjectPath(gameObjectToGetPath);
        }
        gameObjectToGetPath = (GameObject) EditorGUILayout.ObjectField("Target GameObject", gameObjectToGetPath, typeof(GameObject), true);
        if(_pathValue != null) {
            pathToGameObject = (string) EditorGUILayout.TextField(_pathValue.Replace("/GameController/", ""));
        }
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

    private string GetCleanedGameObjectName(string name) {
        return name.Replace("_0", "").Replace("_1", "").Replace("_2", "").Replace("_3", "").Replace("_4", "").Replace("_5", "").Replace("_6", "").Replace("_7", "").Replace("_8", "").Replace("_9", "").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("()", "").Replace("  ", " ");
    }
}