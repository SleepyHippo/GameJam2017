// Reverse engineered UnityEditor.TransformInspector

using UnityEngine;
using Assets.GorGame.Editor;
using UnityEditor;

public class ExTransformInspector : Editor
{
    private const float POSITION_MAX = 100000.0f;

    private static GUIContent positionGUIContent = new GUIContent(LocalString("Position")
        , LocalString("The local position of this Game Object relative to the parent."));
    private static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation")
        , LocalString("The local rotation of this Game Object relative to the parent."));
    private static GUIContent scaleGUIContent = new GUIContent(LocalString("Scale")
        , LocalString("The local scaling of this Game Object relative to the parent."));

    private static string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

    private SerializedProperty positionProperty;
    private SerializedProperty rotationProperty;
    private SerializedProperty scaleProperty;

    private static string LocalString(string text)
    {
        return LocalizationDatabase.GetLocalizedString(text);
    }

    public void OnEnable()
    {
        this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
        this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
        this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
    }

    public override void OnInspectorGUI()
    {
        Transform seclected = (Transform)target;
        this.serializedObject.Update();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
        if (GUILayout.Button("↓", GUILayout.Width(16)))
        {
            Undo.RecordObject(seclected, "Record Transform");
            EditorUtils_Duke.ResetParentGameObjectToLocalBottomCenter();
        }
        if (GUILayout.Button("P", GUILayout.Width(20)))
        {
            Undo.RecordObject(seclected, "Record Transform");
            seclected.localPosition = Vector3.zero;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
        if (GUILayout.Button("R", GUILayout.Width(40)))
        {
            Undo.RecordObject(seclected, "Record Transform");
            seclected.localEulerAngles = Vector3.zero;
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);
        if (GUILayout.Button("S", GUILayout.Width(40)))
        {
            Undo.RecordObject(seclected, "Record Transform");
            seclected.localScale = Vector3.one;
        }
        EditorGUILayout.EndHorizontal();


        if (!ValidatePosition(((Transform)this.target).position))
        {
            EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    private bool ValidatePosition(Vector3 position)
    {
        if (Mathf.Abs(position.x) > POSITION_MAX) return false;
        if (Mathf.Abs(position.y) > POSITION_MAX) return false;
        if (Mathf.Abs(position.z) > POSITION_MAX) return false;
        return true;
    }

    private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
    {
        Transform transform = (Transform)this.targets[0];
        Quaternion localRotation = transform.localRotation;
        foreach (UnityEngine.Object t in (UnityEngine.Object[])this.targets)
        {
            if (!SameRotation(localRotation, ((Transform)t).localRotation))
            {
                EditorGUI.showMixedValue = true;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();

        Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(this.targets, "Rotation Changed");
            foreach (UnityEngine.Object obj in this.targets)
            {
                Transform t = (Transform)obj;
                t.localEulerAngles = eulerAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }

        EditorGUI.showMixedValue = false;
    }

    private bool SameRotation(Quaternion rot1, Quaternion rot2)
    {
        if (rot1.x != rot2.x) return false;
        if (rot1.y != rot2.y) return false;
        if (rot1.z != rot2.z) return false;
        if (rot1.w != rot2.w) return false;
        return true;
    }
}

[CanEditMultipleObjects()]
[CustomEditor(typeof(Transform), true)]
public class CustomTransfrom : ExTransformInspector {
    private static double lastToggleTime;
    [MenuItem("CONTEXT/Transform/Create EmptyParent", false, 150)]
    public static void CreateEmptyParent() {
        if (EditorApplication.timeSinceStartup - lastToggleTime > 0.5f) {
            lastToggleTime = EditorApplication.timeSinceStartup;
            Transform[] targets = Selection.transforms;
            if (targets.Length == 1)
            {
                Transform target = targets[0];
                string name = targets[0].gameObject.name;
                GameObject emptyParent = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(emptyParent, "Create empty parent");

                Undo.SetTransformParent(emptyParent.transform, target, "Move parent");
                Undo.RecordObject(emptyParent.transform, "Reset transform");
                GlobalTools.ResetTransform(emptyParent.transform, false, true);
                Undo.SetTransformParent(emptyParent.transform, target.parent, "Move parent");

                Undo.SetTransformParent(target, emptyParent.transform, "Move to new parent");
                Selection.activeGameObject = emptyParent;
            }
            else if (targets.Length > 1)
            {
                string name = targets[0].gameObject.name;
                GameObject emptyParent = new GameObject(name + "Root");
                Undo.RegisterCreatedObjectUndo(emptyParent, "Create empty parent");

                Undo.SetTransformParent(emptyParent.transform, targets[0], "Move parent");
                Undo.RecordObject(emptyParent.transform, "Reset transform");
                GlobalTools.ResetTransform(emptyParent.transform, false, true);
                Undo.SetTransformParent(emptyParent.transform, targets[0].parent, "Move parent");

                foreach (var target in targets)
                {
                    Undo.SetTransformParent(target, emptyParent.transform, "Move to new parent");
                }
                Selection.activeGameObject = emptyParent;
            }
        }
    }
}