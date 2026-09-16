#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit), true)]
[CanEditMultipleObjects]
public class UnitEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
}
#endif