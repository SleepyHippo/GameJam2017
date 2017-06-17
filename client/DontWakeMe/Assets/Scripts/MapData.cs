// /********************************************************************
//   filename:  MapData.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/


using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DWM {
    public class MapData : ScriptableObject {
        public int width;
        public int height;
        public List<Cell> cells;

        public MapData(int _width, int _height, List<Cell> _cells) {
            width = _width;
            height = _height;
            cells = _cells;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/New MapData")]
        public static void CreateNewMapData()
        {
            CreateInCurrentDirectory<MapData>("MapData");
        }

        public static void CreateInCurrentDirectory<T>(string _assetName) where T : ScriptableObject
        {
            // get current selected directory
            string assetPath = "Assets";
            Object selectFolder = null;

            if (Selection.activeObject)
            {
                selectFolder = Selection.activeObject;
            }

            if (selectFolder != null)
            {
                assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Path.GetExtension(assetPath) != "")
                {
                    assetPath = Path.GetDirectoryName(assetPath);
                }
            }
            else
            {
                assetPath = SaveFileInProject("Create...", "Assets/", _assetName, "asset");
                if (string.IsNullOrEmpty(assetPath))
                    return;
            }

            //
            bool doCreate = true;
            if (doCreate)
            {
                //T newAsset = Create<T>(assetPath, _assetName);
                //Selection.activeObject = newAsset;
                CreateAsset<T>(assetPath);
            }
        }

        public static string SaveFileInProject(string _title, string _dirPath, string _fileName, string _extension)
        {
            string path = EditorUtility.SaveFilePanel(_title, _dirPath, _fileName, _extension);

            // cancelled
            if (path.Length == 0)
                return "";

            string cwd = System.IO.Directory.GetCurrentDirectory().Replace("\\", "/") + "/assets/";
            if (path.ToLower().IndexOf(cwd.ToLower()) != 0)
            {
                path = "";
                EditorUtility.DisplayDialog(_title, "Assets must be saved inside the Assets folder", "Ok");
            }
            else
            {
                path = path.Substring(cwd.Length - "/assets".Length);
            }
            return path;
        }
        
        public static void CreateAsset<T>(string _path) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            if (!string.IsNullOrEmpty(_path) && !_path.EndsWith("/"))
            {
                _path += "/";
            }
            ProjectWindowUtil.CreateAsset(asset, _path + "New " + typeof(T).Name + ".asset");
        }
#endif
    }
}