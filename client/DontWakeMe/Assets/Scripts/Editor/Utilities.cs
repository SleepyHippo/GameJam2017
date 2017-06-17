/********************************************************************
	filename: 	Utilities.cs
	author:		Cloud Zhang
	
	purpose:	工具类
*********************************************************************/
using System;
using System.Reflection;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GoR.Framework {
    /// <summary>
    /// 工具类
    /// </summary>
    public class Utilities {
        // 测试有无写权限
        public static bool HasWriteAccessToFolder(string folderPath) {
            try {
                string tmpFilePath = Path.Combine(folderPath, Path.GetRandomFileName());
                using (
                    FileStream fs = new FileStream(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite,
                        FileShare.ReadWrite)) {
                    StreamWriter writer = new StreamWriter(fs);
                    writer.Write("1");
                }
                File.Delete(tmpFilePath);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Destroy a game object's children
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyGameObjectChildren(GameObject go, bool undo = false) {
            var tran = go.transform;

            while (tran.childCount > 0) {
                var child = tran.GetChild(0);

                if (Application.isEditor && !Application.isPlaying) {
                    child.parent = null; // 清空父, 因为.Destroy非同步的
                    if (undo)
                    {
#if UNITY_EDITOR
                        Undo.DestroyObjectImmediate(child.gameObject);
#else 
                        GameObject.DestroyImmediate(child.gameObject);
#endif
                    }
                    else {
                        GameObject.DestroyImmediate(child.gameObject);
                    }
                }
                else {
                    GameObject.Destroy(child.gameObject);
                    // 预防触发对象的OnEnable，先Destroy
                    child.parent = null; // 清空父, 因为.Destroy非同步的
                }
            }
        }

        /// <summary>
        /// Find Type from every assembly
        /// </summary>
        /// <param name="type"></param>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        public static Type FindType(string qualifiedTypeName) {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null) {
                return t;
            }
            else {
                Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < Assemblies.Length; n++) {
                    Assembly asm = Assemblies[n];
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

        public static void SetChild(GameObject child, GameObject parent, bool selfRotation = false, bool selfScale = false) {
            SetChild(child.transform, parent.transform, selfRotation, selfScale);
        }

        public static void SetChild(Transform child, Transform parent, bool selfRotation = false, bool selfScale = false) {
            child.parent = parent;
            ResetTransform(child, selfRotation, selfScale);
        }

        public static void ResetTransform(UnityEngine.Transform transform, bool selfRotation = false, bool selfScale = false) {
            transform.localPosition = UnityEngine.Vector3.zero;
            if (!selfRotation)
                transform.localEulerAngles = UnityEngine.Vector3.zero;

            if (!selfScale)
                transform.localScale = UnityEngine.Vector3.one;
        }

        /// <summary>
        /// 2个向量行列式的值
        /// </summary>
        /// <param name="_v1"></param>
        /// <param name="_v2"></param>
        /// <returns></returns>
        static float Cross(Vector2 _v1, Vector2 _v2) {
            return _v1.x * _v2.y - _v2.x * _v1.y;
        }
        /// <summary>
        /// 判断点是否在多边形内（2D) 
        /// <returns>返回true表示内部，false表示外部</returns>
        public static bool PtInPolygon2D(Vector2 _point, Vector2[] _polygon) {
            //bool flag = false;
            var count = 0;
            int n = _polygon.Length;
            for (int i = 0; i < _polygon.Length; i++) {
                Vector2 p1 = _polygon[i];
                Vector2 p2 = _polygon[(i + 1) % n];

                //如果点是多边形的顶点之一，则认为在多边形内
                if (FloatUtil.Equal(_point.x, p1.x) && FloatUtil.Equal(_point.y, p1.y)) {
                    return true;
                }
                if (FloatUtil.Equal(p1.y, p2.y)) {
                    continue;
                }
                var min = Mathf.Min(p1.y, p2.y);
                var max = Mathf.Max(p1.y, p2.y);
                if (FloatUtil.LessThanOrEqual(_point.y, min)) {
                    continue;
                }
                if (FloatUtil.GreaterThanOrEqual(_point.y, max)) {
                    continue;
                }
                var x = (_point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                if (FloatUtil.GreaterThan(x, _point.x))
                    count++;
            }

            return count % 2 > 0 ? true : false;
        }

        public static string GetAssetPath(string _filePath) {
            if (_filePath != null)
                return _filePath.Replace(Application.dataPath, "Assets");
            return null;
        }

        public static string GetVector3Detail(Vector3 _vector3) {
            return string.Format("{{{0:0.00000}, {1:0.00000}, {2:0.00000}}}", _vector3.x, _vector3.y, _vector3.z);
        }
    }

    public class FloatUtil {
        public static float Precision = 0.001f;
        // Fields 浮点型的误差
        //private const float FLOAT_DELTA = 0.001f;
        public static bool Equal(float _a, float _b) {
            return (_a == _b)
                || Mathf.Abs(_a - _b) < Precision;

        }

        public static bool GreaterThan(float _a, float _b) {
            return ((_a > _b) && !Equal(_a, _b));
        }

        public static bool GreaterThanOrEqual(float _a, float _b) {
            return (_a > _b) || Equal(_a, _b);
        }

        public static bool IsZero(float _value) {
            return (Math.Abs(_value) < Precision);
        }

        public static bool LessThan(float _a, float _b) {
            return ((_a < _b) && !Equal(_a, _b));
        }

        public static bool LessThanOrEqual(float _a, float _b) {
            return (_a < _b) || Equal(_a, _b);
        }
    }
}
