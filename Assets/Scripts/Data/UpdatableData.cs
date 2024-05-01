using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdatableData : ScriptableObject {
    public System.Action OnUpdate;
    public bool autoUpdate;

    protected virtual void OnValidate() {
        if (autoUpdate) {
            OnUpdate?.Invoke();
        }
    }
}
