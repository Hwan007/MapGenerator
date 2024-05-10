using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UpdatableData : ScriptableObject {
    public System.Action OnValuesUpdate;
    public bool autoUpdate;

    protected virtual void OnValidate() {
        if (autoUpdate) {
            OnValuesUpdate?.Invoke();
        }
    }
}



[CustomEditor(typeof(UpdatableData))]
public class CustomEditorUpdatableData : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        UpdatableData data = (UpdatableData)target;

        if (GUILayout.Button("Update")) {
            data.OnValuesUpdate?.Invoke();
            EditorUtility.SetDirty(target);
        }
    }
}