/********************************************************************
  filename:  EditorUtils_Duke.cs
  author:    Duke
  date:      2017/05/25

  purpose:   
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
//using OrbCreationExtensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.GorGame.Editor {
    public class EditorUtils_Duke {
        [MenuItem("Tools/Duke/ShowHideObject %G", false)]
        private static void ShowHideObject() {
            GameObject[] gos = Selection.gameObjects;
            Undo.RecordObjects(gos, "Show hide objects");
            foreach (GameObject go in gos) {
                go.SetActive(!go.activeSelf);
            }
        }

        [MenuItem("Tools/Duke/Add Bound BoxCollider #&C", false)]
        private static void AddBoundBoxCollider() {
            GameObject parent = Selection.activeGameObject;
            Bounds localBounds = GetLocalBounds(parent);
            BoxCollider collider = parent.UndoAddMissingComponent<BoxCollider>();
            Undo.RecordObject(collider, "Resize collider");
            collider.center = localBounds.center;
            collider.size = localBounds.size;
        }

        public static Bounds GetLocalBounds(GameObject go) {
            //先设置旋转为0
            Vector3 originEuler = go.transform.eulerAngles;
            go.transform.eulerAngles = Vector3.zero;

            Bounds parentBounds = GetWorldBounds(go);
            Vector3 position = go.transform.position;
            Vector3 scale = go.transform.localScale;

            Bounds localBounds = new Bounds();
            localBounds.size = new Vector3(parentBounds.size.x / scale.x, parentBounds.size.y / scale.y,
                parentBounds.size.z / scale.z);
            localBounds.center = new Vector3((parentBounds.center.x - position.x) / scale.x,
                (parentBounds.center.y - position.y) / scale.y, (parentBounds.center.z - position.z) / scale.z);

            //重置旋转
            go.transform.eulerAngles = originEuler;
            return localBounds;
        }

        public static Bounds GetWorldBounds(GameObject go, bool _includeInactive = true, bool _includeRenderer = true, bool _includeCollider = true, bool _includeTransform = false) {
            if (go.transform == null) return new Bounds();
            Bounds goBounds = new Bounds(go.transform.position, Vector3.zero);
            bool first = true;
            if (_includeRenderer) {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(_includeInactive);
                foreach (Renderer r in renderers) {
                    Bounds bounds = r.bounds;
                    if (first) {
                        goBounds.center = bounds.center;
                        goBounds.size = bounds.size;
                        first = false;
                    }
                    else {
                        goBounds.Encapsulate(bounds);
                    }
                }
            }
            if (_includeCollider) {
                Collider[] colliders = go.GetComponentsInChildren<Collider>(_includeInactive);
                foreach (Collider c in colliders)
                {
                    Bounds bounds = c.bounds;
                    if (first)
                    {
                        goBounds.center = bounds.center;
                        goBounds.size = bounds.size;
                        first = false;
                    }
                    else {
                        goBounds.Encapsulate(bounds);
                    }
                }
            }
            if (_includeTransform)
            {
                Transform[] transforms = go.GetComponentsInChildren<Transform>(_includeInactive);
                foreach (Transform t in transforms)
                {
                    if (first)
                    {
                        goBounds.center = t.position;
                        first = false;
                    }
                    else {
                        goBounds.Encapsulate(t.position);
                    }
                }
            }
            return goBounds;
        }

        [MenuItem("Tools/Duke/Set parent GameObject to bottom center(Local)", false)]
        public static void ResetParentGameObjectToLocalBottomCenter() {
            GameObject parent = Selection.activeGameObject;
            Bounds parentBounds = GetLocalBounds(parent);
            Vector3 bottomCenterPosition =
                parent.transform.TransformPoint(parentBounds.center + Vector3.down * parentBounds.size.y / 2);

            MoveParentTo(parent.transform, bottomCenterPosition, Space.World);
        }

        [MenuItem("Tools/Duke/Set parent GameObject to bottom center(World)", false)]
        public static void ResetParentGameObjectToWorldBottomCenter() {
            GameObject parent = Selection.activeGameObject;
            Bounds parentBounds = GetWorldBounds(parent);
            Vector3 bottomCenterPosition = new Vector3(parentBounds.center.x,
                parentBounds.center.y - parentBounds.size.y / 2, parentBounds.center.z);

            MoveParentTo(parent.transform, bottomCenterPosition, Space.World);
        }

        public static void MoveParentTo(Transform _parent, Vector3 _position, Space _relativeTo = Space.World) {
            int childCount = _parent.childCount;
            if (childCount > 0) {
                List<Transform> childTsfs = new List<Transform>();
                for (int i = 0; i < childCount; ++i) //先移动到同一层
                {
                    Transform tsf = _parent.GetChild(0);
                    childTsfs.Add(tsf);
                    Undo.RecordObject(tsf, "Move Parent Position");
                    tsf.parent = _parent.parent;
                }
                Undo.RecordObject(_parent, "Move Parent Position");
                if (_relativeTo == Space.World) {
                    _parent.transform.position = _position; //设置position
                }
                else {
                    _parent.transform.localPosition = _position; //设置position
                }
                for (int i = 0; i < childCount; ++i) //移回widget
                {
                    Transform tsf = childTsfs[i];
                    Undo.RecordObject(tsf, "Move Parent Position");
                    tsf.parent = _parent;
                }
            }
        }

        public static void ClearLog() {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        private static RaycastHit[] hits = new RaycastHit[20];
        public static bool GetMouseHitInfoInScene(SceneView _sceneView, out RaycastHit _hitInfo, bool _ignoreTrigger = true, bool _ignoreInvisible = true)
        {
            Camera cameara = _sceneView.camera;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            QueryTriggerInteraction qti = _ignoreInvisible || _ignoreTrigger
                ? QueryTriggerInteraction.Ignore
                : QueryTriggerInteraction.Collide;
            if (Physics.Raycast(ray, out _hitInfo, 10000, -1, qti))
            {
                if (_ignoreInvisible) {
                    Renderer renderer = _hitInfo.transform.GetComponent<Renderer>();
                    if (renderer == null || renderer.enabled == false) {
                        int count = Physics.RaycastNonAlloc(ray, hits, 10000, -1, qti);
                        for (int i = 0; i < count; i++) {
                            RaycastHit hit = hits[i];
                            Renderer nextRenderer = hit.transform.GetComponent<Renderer>();
                            if (nextRenderer == null || nextRenderer.enabled == false) {
                                continue;
                            }
                            _hitInfo = hit;
                            return true;
                        }
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static void DrawMarkAtScene(SceneView _sceneView, Vector3 _position)
        {
            Vector3 upPosition = _position;
            upPosition.z -= 5;

            Handles.color = Color.yellow;
            Handles.DrawLine(_position, upPosition);
            float arrowSize = 1;
            Vector3 pos = _position;
            Quaternion quat;
            Handles.color = Color.green;
            quat = Quaternion.LookRotation(Vector3.up, Vector3.up);
            Handles.ArrowCap(0, pos, quat, arrowSize);
            Handles.color = Color.red;
            quat = Quaternion.LookRotation(Vector3.right, Vector3.up);
            Handles.ArrowCap(0, pos, quat, arrowSize);
            Handles.color = Color.blue;
            quat = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            Handles.ArrowCap(0, pos, quat, arrowSize);
            //Handles.DrawLine(pos + new Vector3(0, 3, 0), pos); 
        }

        public static bool GetMouseHitVertexInScene(SceneView _sceneView, out RaycastHit _hitInfo, out Vector3 _vertex, float _threshold = 0.35f, bool _ignoreTrigger = true, bool _ignoreInvisible = true) {
            if (GetMouseHitInfoInScene(_sceneView, out _hitInfo, _ignoreTrigger, _ignoreInvisible)) {
                MeshFilter meshFilter = _hitInfo.transform.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null) {
                    Vector3 localHitPos = meshFilter.transform.InverseTransformPoint(_hitInfo.point);
                    Vector3[] vertices = meshFilter.sharedMesh.vertices;
                    for (int i = 0; i < vertices.Length; ++i) {
                        Vector3 localVPos = vertices[i];
                        //check should snap
                        if (-_threshold < localVPos.x - localHitPos.x && localVPos.x - localHitPos.x < _threshold &&
                            -_threshold < localVPos.y - localHitPos.y && localVPos.y - localHitPos.y < _threshold &&
                            -_threshold < localVPos.z - localHitPos.z && localVPos.z - localHitPos.z < _threshold) {
                            _vertex = meshFilter.transform.TransformPoint(localVPos);
                            return true;
                        }
                    }
                }

                _vertex = _hitInfo.point;
                return true;
            }
            _vertex = Vector3.zero;
            return false;
        }

        public static void SelectAssetPath(string _assetPath)
        {
            Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            if (projectBrowserType != null)
            {
                FieldInfo lastProjectBrowser = projectBrowserType.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public);
                if (lastProjectBrowser != null)
                {
                    object lastProjectBrowserInstance = lastProjectBrowser.GetValue(null);
                    FieldInfo projectBrowserViewMode = projectBrowserType.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (projectBrowserViewMode != null)
                    {
                        // 0 - one column, 1 - two column
                        int viewMode = (int)projectBrowserViewMode.GetValue(lastProjectBrowserInstance);
                        if (viewMode == 1)
                        {
                            MethodInfo showFolderContents = projectBrowserType.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (showFolderContents != null)
                            {
                                Object sceneFolder = AssetDatabase.LoadAssetAtPath(_assetPath, typeof(Object));
                                if (sceneFolder != null) {
                                    showFolderContents.Invoke(lastProjectBrowserInstance, new object[] {sceneFolder.GetInstanceID(), true});
                                }
                                else {
                                    Debug.LogError("Path is wrong: " + _assetPath);
                                }
                            }
                            else
                            {
                                Debug.LogError("Can't find ShowFolderContents method!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Can't find m_ViewMode field!");
                    }
                }
                else
                {
                    Debug.LogError("Can't find s_LastInteractedProjectBrowser field!");
                }
            }
            else
            {
                Debug.LogError("Can't find UnityEditor.ProjectBrowser type!");
            }

//            selection.Add(targetGameObjectInScene);
//            Selection.objects = selection.ToArray();
        }
    }
}