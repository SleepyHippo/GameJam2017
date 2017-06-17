// /********************************************************************
//   filename:  MapContainer.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/

using UnityEngine;

namespace DWM {
    [ExecuteInEditMode]
    public class MapContainer : MonoBehaviour {
        public int faceWidth;
        public int height;
        public int faceCount;

        public GameObject diryPrefab;
        public GameObject rootPrefab;
        public GameObject branchPrefab;

        public bool showGizmos = true;
        private Map map;

        void Awake() {
            map = new Map(faceWidth, height, faceCount);
        }

        [ContextMenu("DebugDrawDirt")]
        void DebugDrawDirt() {
            int totalWidth = faceWidth * faceCount;
            GameObject root = GameObject.Find("_DirtRoot");
            if (root == null) {
                root = new GameObject("_DirtRoot");
            }
            for (int x = 0; x < totalWidth; ++x) {
                for (int y = 0; y < height / 2; ++y) {
                    GameObject.Instantiate(diryPrefab, new Vector3(x, y, 0), Quaternion.identity).transform.SetParent(root.transform);
//                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                    cube.transform.position = new Vector3(x, y, 0);
                }
            }
        }

        void OnDrawGizmos() {
            if (showGizmos) {
                //Face1:
                int totalWidth = faceWidth * faceCount;
                for (int x = 0; x < totalWidth; ++x) {
                    Gizmos.DrawLine(new Vector3(x - 0.5f, -0.5f, 0), new Vector3(x - 0.5f, height + 1, 0));
                }

//                    int faceIndex = map.GetFaceIndex(x);
                Gizmos.color = Color.black;
                for (int y = 0; y < height; ++y) {
                    if (y == height / 2) {
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawLine(new Vector3(-0.5f, y - 0.5f, 0), new Vector3(totalWidth + 1, y - 0.5f, 0));
                }
            }
        }
    }
}