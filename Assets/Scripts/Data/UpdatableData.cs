using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UpdatableData : ScriptableObject {
    public System.Action OnValuesUpdate;
    public bool autoUpdate;
}



[CustomEditor(typeof(UpdatableData))]
public class CustomEditorUpdatableData : Editor {
    UpdatableData data;
    private void OnEnable() {
        data = (UpdatableData)target;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update")) {
            data.OnValuesUpdate?.Invoke();
        }
    }
}