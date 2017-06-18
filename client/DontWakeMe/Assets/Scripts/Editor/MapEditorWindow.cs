using System;
using System.Collections.Generic;
using Assets.GorGame.Editor;
using DWM;
using GoR.Framework;
//using Assets.GorGame.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

public class MapEditorWindow : EditorWindow {
    RaycastHit _hitInfo;

    private CellType currentCellType;
    private int branchId;
    private int groupId;
    private int value;
    private int hp;
    private int style;
    private bool showGizmos = false;

    private GameObject MapBGQuad {
        get {
            GameObject mapBGQuad = GameObject.Find("_MapBGPlane");
            if (mapBGQuad == null) {
                mapBGQuad = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mapBGQuad.GetComponent<MeshRenderer>().enabled = false;
                mapBGQuad.name = "_MapBGPlane";
                Map map = MapContainer.Map;
                mapBGQuad.transform.position = new Vector3(map.width / 2 - 0.5f, map.height / 2 - 0.5f, -5);
                mapBGQuad.transform.localScale = new Vector3(map.width, map.height, 2);
            }
            return mapBGQuad;
        }
    }

    private MapContainer MapContainer {
        get {
            MapContainer mapContainer = GameObject.FindObjectOfType<MapContainer>();
            if (mapContainer == null) {
                mapContainer = new GameObject("_MapContainer").AddComponent<MapContainer>();
            }
            return mapContainer;
        }
    }

    [MenuItem("Window/MapEditor #`")]
    static void AddWindow() {
        var window = GetWindow<MapEditorWindow>();
        window.Init();
    }

    void Init() {
        //        Debug.Log("MapEditor Init");
        SceneView.onSceneGUIDelegate -= OnSceneFunc;
        SceneView.onSceneGUIDelegate += OnSceneFunc;

        Debug.Log(MapBGQuad.transform.position);
        MapContainer.Refresh();
        Repaint();
    }

    void OnEnable() {
        //        Debug.Log("MapEditor Enable");
        Init();
    }

    void OnDisable() {
        //        Debug.Log("MapEditor Destroy");
        SceneView.onSceneGUIDelegate -= OnSceneFunc;
    }

    void OnGUI() {
        if (EditorApplication.isCompiling) {
            ShowNotification(new GUIContent("正在编译代码\n...请等待..."));
            return;
        }

        MapContainer container;
        bool isSelectMapEditor = IsSelectMapContainer(out container);

//        if (GUILayout.Button("Import data")) {
//            ImportMapData();
//        }
        if ((isSelectMapEditor || (Selection.activeGameObject != null && Selection.activeGameObject.name.Equals("_MapContainer")))
            && GUILayout.Button("Export data")) {
            ExportAllMapData();
        }
        if (GUILayout.Button("Reset(重置)")) {
            RemoveAllCellObject();
            Refresh();
            EditorSceneManager.MarkAllScenesDirty();
        }
        //        GUILayout.Label("N: Create Spawner");
        if (!isSelectMapEditor) {
            return;
        }
//        GUILayout.Label("G: Move Spawner");
//        GUILayout.Label("A: Add mob");
        GUILayout.Label("Current:\n" + currentCellType);
//        if (GUILayout.Button("树根(Root)")) {
//            currentCellType = CellType.Root;
//        }
        if (GUILayout.Button("树枝(Branch)")) {
            currentCellType = CellType.Branch;
        }
//        if (GUILayout.Button("楼梯(Ladder)")) {
//            currentCellType = CellType.Ladder;
//        }
        if (GUILayout.Button("道具(Item)")) {
            currentCellType = CellType.Item;
        }
        if (currentCellType == CellType.Root || currentCellType == CellType.Branch) {
            branchId = DrawIntField("BranchID", branchId, 1, 4);
            groupId = DrawIntField("GroupID", groupId, 1, 40);
            value = DrawIntField("Value", value, 1, 99);
            hp = DrawIntField("Hp", hp, 0, 100);
            style = DrawIntField("Style", style, 0, 2);
        }
        else {
            branchId = 0;
            groupId = 0;
            value = 0;
            hp = 0;
            style = 0;
        }

        if (GUILayout.Button("Swap Root/Branch")) {
            for (int i = 0; i < MapContainer.Map.cells.Count; i++) {
                Cell cell = MapContainer.Map.cells[i];
                if (cell.type == CellType.Root) {
                    cell.type = CellType.Branch;
                }
                else if (cell.type == CellType.Branch) {
                    cell.type = CellType.Root;
                }
                MapContainer.RefreshCell(cell.x, cell.y);
            }
        }
        if (GUILayout.Button("复制上面到下面")) {
            //清空下面的
            for (int i = 0; i < MapContainer.Map.cells.Count; i++) {
                Cell cell = MapContainer.Map.cells[i];
                if (cell.y < MapContainer.Map.landHeight) {
                    MapContainer.Map.RemoveCell(cell.x, cell.y);
                }
            }
            for (int i = 0; i < MapContainer.Map.cells.Count; i++) {
                Cell cell = MapContainer.Map.cells[i];
                int mirrorY = MapContainer.Map.height - 1 - cell.y;
                CellType mirrorType = CellType.Branch;
                if (cell.type == CellType.Root) {
                    mirrorType = CellType.Branch;
                }
                else if (cell.type == CellType.Branch) {
                    mirrorType = CellType.Root;
                }
                MapContainer.Map.SetCell(cell.x, mirrorY, mirrorType, cell.branchId, cell.groupId, cell.hp, cell.value, cell.style);
            }
            MapContainer.DrawMap(false);
            MapContainer.Map.InitTree();
        }
        showGizmos = EditorGUILayout.Toggle("ShowGizmos", showGizmos);
        //        scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition, false, true);
        //        for (int i = 0; i < MetaManager.Instance.Monster.Count; ++i)
        //        {
        //            MonsterDefine monsterDefine = MetaManager.Instance.Monster[i];
        //            if (GUILayout.Button(monsterDefine.Name))
        //            {
        //                currentMobId = monsterDefine.ID;
        //                currentMobName = monsterDefine.Name;
        //                SelectSceneView();
        //            }
        //        }
        //        GUILayout.EndScrollView();
    }

    void OnInspectorGUI() {
        Debug.Log("OnInspectorGUI");
    }

    public void OnSceneFunc(SceneView sceneView) {
        CustomSceneGUI(sceneView);
    }

    void OnSelectionChange() {
        Repaint();
    }

    private bool isPainting = false;

    void CustomSceneGUI(SceneView sceneView) {
        Vector3 mouseSelectPosition;
        if (Assets.GorGame.Editor.EditorUtils_Duke.GetMouseHitInfoInScene(sceneView, out _hitInfo, true, false)) {
            mouseSelectPosition = _hitInfo.point;
//            Debug.Log(mouseSelectPosition);
            sceneView.Focus();
//            //create new spawner at mouse
            Event e = Event.current;
//            if (e.type == EventType.keyUp && e.keyCode == KeyCode.N) {
//                int count = Object.FindObjectsOfType<MapEditor>().Length + 1;
//                GameObject newSpawner = new GameObject("Spawner" + count);
//                Undo.RegisterCreatedObjectUndo(newSpawner, "new spawner");
//                newSpawner.AddComponent<MapEditor>();
//                newSpawner.AddComponent<BoxCollider>().isTrigger = true;
//                newSpawner.transform.SetParent(MapContainer.transform);
//                newSpawner.transform.position = mouseSelectPosition;
//                Selection.activeGameObject = newSpawner;
//                EditorGUIUtility.PingObject(newSpawner);
//            }

            Assets.GorGame.Editor.EditorUtils_Duke.DrawMarkAtScene(sceneView, mouseSelectPosition);

//            MapContainer MapEditor;
//            if (!IsSelectMapContainer(out MapEditor)) {
//                SceneView.RepaintAll();
//                return;
//            }


            //place current spawner at mouse
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.A) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                MapContainer.Map.SetCell(x, y, currentCellType, branchId, groupId, hp, value, style);
                MapContainer.RefreshCell(x, y);
                e.Use();
                EditorSceneManager.MarkAllScenesDirty();
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.S) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    branchId = cell.branchId;
                    groupId = cell.groupId;
                    value = cell.value;
                    hp = cell.hp;
                    style = cell.style;
                    Repaint();
                }
                else {
                    Debug.LogError("cell is null: " + x + " " + y);
                }
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.D) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                MapContainer.Map.RemoveCell(x, y);
                MapContainer.RefreshCell(x, y);
                e.Use();
                EditorSceneManager.MarkAllScenesDirty();
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.U) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    cell.style = 0;
                    Repaint();
                    MapContainer.RefreshCell(x, y);
                    e.Use();
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.I) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    cell.style = 1;
                    Repaint();
                    MapContainer.RefreshCell(x, y);
                    e.Use();
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.O) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    cell.style = 2;
                    Repaint();

                    MapContainer.RefreshCell(x, y);
                    e.Use();
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.J) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    Group group;
                    if (MapContainer.Map.upTree.branchGroupMap.TryGetValue(cell.nodeId, out group)) {
                        for (int i = 0; i < group.cells.Count; ++i) {
                            hp = group.cells[i].hp = 0;
                        }
                    }
                    Repaint();
                    e.Use();
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            if (e.type == EventType.keyDown && e.keyCode == KeyCode.K) {
                int x = Mathf.RoundToInt(mouseSelectPosition.x);
                int y = Mathf.RoundToInt(mouseSelectPosition.y);
                Cell cell = MapContainer.Map.GetCell(x, y);
                if (cell != null) {
                    Group group;
                    if (MapContainer.Map.upTree.branchGroupMap.TryGetValue(cell.nodeId, out group)) {
                        for (int i = 0; i < group.cells.Count; ++i) {
                            hp = group.cells[i].hp = 100;
                        }
                    }
                    Repaint();
                    e.Use();
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }
            if (showGizmos) {
                List<Cell> cells = MapContainer.Map.cells;
                for (int i = 0; i < cells.Count; ++i) {
                    Cell cell = cells[i];
                    Handles.Label(new Vector3(cell.x, cell.y, -3f), cell.hp.ToString());
                }
            }
            SceneView.RepaintAll();
        }
    }

    private bool IsSelectMapContainer(out MapContainer _mapContainer) {
        _mapContainer = null;
        if (Selection.activeGameObject == null) {
            return false;
        }
        _mapContainer = Selection.activeGameObject.GetComponent<MapContainer>();
        if (_mapContainer == null) {
            return false;
        }
        return true;
    }

    void SelectSceneView() {
        if (SceneView.sceneViews.Count > 0) {
            SceneView sceneView = (SceneView) SceneView.sceneViews[0];
            sceneView.Focus();
        }
    }

    void RemoveAllCellObject() {
        int childCount = MapContainer.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            DestroyImmediate(MapContainer.transform.GetChild(i).gameObject);
        }
        MapContainer.ClearMap();
    }

    void Refresh() {
        MapContainer.Refresh();
        Map map = MapContainer.Map;
        MapBGQuad.transform.position = new Vector3(map.width / 2 - 0.5f, map.height / 2 - 0.5f, -5);
        MapBGQuad.transform.localScale = new Vector3(map.width, map.height, 2);

        MapContainer.DrawMap();
        MapContainer.Map.InitTree();
    }

//    void ImportMapData() {
//        var path = EditorUtility.OpenFilePanel(
//            "Open MapEditorData",
//            "Assets/StaticData/Resources",
//            "asset");
//        if (string.IsNullOrEmpty(path)) {
//            return;
//        }
//        MapData mapData = AssetDatabase.LoadAssetAtPath<MapData>(Utilities.GetAssetPath(path));
//        GameObject mapContainer = GameObject.Find("MapContainer");
//        if (mapContainer != null) {
//            Undo.DestroyObjectImmediate(mapContainer);
//        }
//        mapContainer = new GameObject("MapContainer");
////        for (int i = 0; i < mapData.cells.Count; ++i)
////        {
////            Cell data = mapData.cells[i];
////            MapEditor MapEditor = new GameObject(data.spwnerName).AddComponent<MapEditor>();
////            MapEditor.transform.SetParent(mapContainer.transform);
////            MapEditor.Deserialize(data);
////        }
//        Undo.RegisterCreatedObjectUndo(mapContainer, "create root");
//        Selection.activeGameObject = mapContainer;
//        EditorGUIUtility.PingObject(mapContainer);
//    }

    void ExportAllMapData() {
//        var path = EditorUtility.SaveFilePanel(
//            "Create MapMapEditorData",
//            "Assets/StaticData/Resources",
//            "data.asset",
//            "asset");
//        Debug.Log(path);
//        if (string.IsNullOrEmpty(path)) {
//            return;
//        }
        string path = "Assets/StaticData/Resources/data.asset";
        MapData mapData = ScriptableObject.CreateInstance<MapData>();
        mapData.width = MapContainer.Map.width;
        mapData.height = MapContainer.Map.height;
        mapData.cells = new List<Cell>(MapContainer.Map.cells.Count);
        for (int i = 0; i < MapContainer.Map.cells.Count; ++i) {
            Cell cell = MapContainer.Map.cells[i];
            mapData.cells.Add(new Cell(cell));
        }
//        mapData.cells = MapContainer.Map.cells;
        AssetDatabase.CreateAsset(mapData, path);
//        Selection.activeObject = mapData;
        AssetDatabase.SaveAssets();
        MapContainer.data = AssetDatabase.LoadAssetAtPath<MapData>(path);
    }

    int DrawIntField(string name, int value, int min, int max) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(60));
        int result = EditorGUILayout.IntSlider(value, min, max);
        GUILayout.EndHorizontal();
        return result;
    }
}