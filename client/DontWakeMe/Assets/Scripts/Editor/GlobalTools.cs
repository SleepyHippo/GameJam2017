using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

static public class GlobalTools {
    static public T UndoAddMissingComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
#if UNITY_EDITOR
            comp = Undo.AddComponent<T>(go);
#else
            comp = go.AddComponent<T>();
#endif
        }
        return comp;
    }

    static public T AddMissingComponent<T> (this GameObject go) where T : Component {
        T comp = go.GetComponent<T>();
        if (comp == null) {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    static public T AddMissingComponent<T> (this Component _comp) where T : Component {
        T result = null;
        if (_comp != null) {
            result = _comp.gameObject.AddMissingComponent<T>();
        }
        return result;
    }

    public static void ResetTransform(UnityEngine.Transform transform, bool selfRotation = false, bool selfScale = false)
    {
        transform.localPosition = UnityEngine.Vector3.zero;
        if (!selfRotation)
            transform.localEulerAngles = UnityEngine.Vector3.zero;

        if (!selfScale)
            transform.localScale = UnityEngine.Vector3.one;
    }
}
