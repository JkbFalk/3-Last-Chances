using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitAI), true)]
[CanEditMultipleObjects]
public class UnitAIEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
}
#endif
