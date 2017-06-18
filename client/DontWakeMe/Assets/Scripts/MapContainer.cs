// /********************************************************************
//   filename:  MapContainer.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace DWM {
    public class MapContainer : MonoBehaviour {
//        public int faceWidth;
//        public int height;
//        public int faceCount;

        public MapData data;

        [Range(0, 100)]
        public int waterValue = 10;
        [Range(-100, 0)]
        public int digValue = -10;

//        public GameObject dirtPrefab;
        public GameObject[] rootPrefab;
        public GameObject[] branchPrefab;
//        public GameObject ladderPrefab;
        public GameObject itemPrefab;

        public bool showGizmos = true;

        private Map map;

        public Map Map
        {
            get
            {
                if (map == null)
                {
                    map = new Map(null);
                }
                return map;
            }
        }

        private List<GameObject> cellObjects = new List<GameObject>();
        private Dictionary<int, GameObject> cellObjectDic = new Dictionary<int, GameObject>();

        void Awake() {
            DrawMap();
            Map.InitTree();
        }

        /// <summary>
        /// Refresh using data
        /// </summary>
        public void Refresh() {
            Map.Deserilize(data);
        }

        public void DrawMap(bool _loadData = true) {
            if (data == null) {
                return;
            }
            if (_loadData) {
                Refresh();
            }
            ClearMap();
            for (int i = 0; i < map.cells.Count; i++) {
                Cell cell = map.cells[i];
                DrawCellObject(cell);
            }
        }

        public void ClearMap() {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                if (Application.isPlaying)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                else
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
            cellObjects.Clear();
            cellObjectDic.Clear();
        }

        public void RefreshCell(int _x, int _y) {
            Cell cell = map.GetCell(_x, _y);
            RemoveCellObject(_x, _y);
            DrawCellObject(cell);
        }

        public void Water(Vector3 _position) {
            int x = Mathf.RoundToInt(_position.x);
            int y = Mathf.RoundToInt(_position.y);
            Cell waterCell = Map.GetCell(x, y);
            if (waterCell == null || waterCell.hp == 100) {
                return;
            }
            Group group = Map.AddHp(x, y, waterValue);
            if (group != null) {
                for (int i = 0; i < group.cells.Count; ++i) {
                    Cell cell = group.cells[i];
                    //特效
                    RefreshCell(cell.x, cell.y);
                }
            }
            int mirrorY = map.height - y;
            group = Map.AddHp(x, mirrorY, waterValue);
            if (group != null) {
                for (int i = 0; i < group.cells.Count; ++i) {
                    Cell cell = group.cells[i];
                    //特效
                    RefreshCell(cell.x, cell.y);
                }
            }
        }

        public void Dig(Vector3 _position) {
            int x = Mathf.RoundToInt(_position.x);
            int y = Mathf.RoundToInt(_position.y);
            Cell digCell = Map.GetCell(x, y);
            if (digCell == null || digCell.hp == 0) {
                return;
            }
            Group group = Map.AddHp(x, y, digValue);
            if (group.hp == 0) {
                Branch nowBranch = Map.botTree.branchs.Find(b => b.branchId == digCell.branchId);
                for (int groupIndex = group.groupId + 1; groupIndex < nowBranch.groups.Count; ++groupIndex) {
                    Group nextGroup = nowBranch.groups[groupIndex];
                    if (nextGroup.groupId > group.groupId)
                    {
                        nextGroup.AddHp(-100);
                        for (int i = 0; i < nextGroup.cells.Count; ++i) {
                            Cell cell = nextGroup.cells[i];
                            //特效
                            RefreshCell(cell.x, cell.y);
                        }

                        Group mirrorGroup = Map.upTree.branchGroupMap[nextGroup.branchGroupId];
                        mirrorGroup.AddHp(-100);
                        for (int i = 0; i < mirrorGroup.cells.Count; ++i) {
                            Cell cell = mirrorGroup.cells[i];
                            //特效
                            RefreshCell(cell.x, cell.y);
                        }
                    }
                }
                return;
            }
            if (group != null) {
                for (int i = 0; i < group.cells.Count; ++i) {
                    Cell cell = group.cells[i];
                    //特效
                    RefreshCell(cell.x, cell.y);
                }
            }
            int mirrorY = map.height - y;
            group = Map.AddHp(x, mirrorY, digValue);
            if (group != null) {
                for (int i = 0; i < group.cells.Count; ++i) {
                    Cell cell = group.cells[i];
                    //特效
                    RefreshCell(cell.x, cell.y);
                }
            }
        }

        void DrawCellObject(Cell _cell) {
            if (_cell == null) {
                return;
            }
            GameObject cellObject = null;
            switch (_cell.type) {
                case CellType.Root:
                    cellObject = Object.Instantiate(rootPrefab[_cell.style]);
                    if (Application.isPlaying) {
                        Material m = cellObject.GetComponent<MeshRenderer>().material;
                        m.SetFloat("_Intensity", Mathf.Lerp(1, 0, _cell.hp / 100f));
                        m.SetFloat("_Alpha", Mathf.Lerp(0, 1, _cell.hp / 100f));
                    }
                    break;
                case CellType.Branch:
                    cellObject = Object.Instantiate(branchPrefab[_cell.style]);
                    if (Application.isPlaying) {
                        cellObject.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", Mathf.Lerp(1, 0, _cell.hp / 100f));
                    }
                    break;
                //                case CellType.Ladder:
                //                    cellObject = Object.Instantiate(ladderPrefab);
                //                    break;
                case CellType.Item:
                    cellObject = Object.Instantiate(itemPrefab);
                    break;
            }
            if (cellObject != null) {
                cellObject.transform.position = new Vector3(_cell.x, _cell.y, 0);
                cellObject.transform.parent = transform;
                cellObjects.Add(cellObject);
                cellObjectDic.Add(map.GetCellIndex(_cell.x, _cell.y), cellObject);
            }
        }

        void RemoveCellObject(int _x, int _y) {
            int index = map.GetCellIndex(_x, _y);
            if (!cellObjectDic.ContainsKey(index)) {
                return;
            }
            GameObject cellObject = cellObjectDic[index];
            cellObjects.Remove(cellObject);
            cellObjectDic.Remove(index);
            if (Application.isPlaying) {
                Destroy(cellObject);
            }
            else {
                DestroyImmediate(cellObject);
            }
        }

//        [ContextMenu("DebugDrawDirt")]
//        void DebugDrawDirt() {
//            int totalWidth = data.width;
//            GameObject root = GameObject.Find("_DirtRoot");
//            if (root == null) {
//                root = new GameObject("_DirtRoot");
//            }
//            for (int x = 0; x < totalWidth; ++x) {
//                for (int y = 0; y < data.height / 2; ++y) {
//                    GameObject.Instantiate(dirtPrefab, new Vector3(x, y, 0), Quaternion.identity).transform.SetParent(root.transform);
////                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
////                    cube.transform.position = new Vector3(x, y, 0);
//                }
//            }
//        }

        void OnDrawGizmos() {
            if (data != null && showGizmos) {
                //Face1:
                int totalWidth = data.width;
                Gizmos.color = Color.white;
                for (int x = 0; x < totalWidth + 1; ++x) {
//                    if (x == totalWidth * 0.25f) {
//                        Gizmos.color = Color.red;
//                    }
//                    else if (x == totalWidth * 0.5f) {
//                        Gizmos.color = Color.green;
//                    }
//                    else if (x == totalWidth * 0.75f) {
//                        Gizmos.color = Color.blue;
//                    }
                    Gizmos.DrawLine(new Vector3(x - 0.5f, -0.5f, 0), new Vector3(x - 0.5f, data.height - 0.5f, 0));
                }

//                    int faceIndex = map.GetFaceIndex(x);
                Gizmos.color = Color.black;
                for (int y = 0; y < data.height + 1; ++y) {
                    if (y == data.height / 2) {
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawLine(new Vector3(-0.5f, y - 0.5f, 0), new Vector3(totalWidth - 0.5f, y - 0.5f, 0));
                }
            }
        }
    }
}