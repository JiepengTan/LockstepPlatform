using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Text;
using Lockstep.Util;

public class EditorUtil {
    
    public static void ShowMessage(string content){
        EditorWindow window = null;
        if (EditorWindow.focusedWindow == null) {
            window = EditorWindow.mouseOverWindow;
        }
        window?.ShowNotification(new GUIContent(content));
    }
    /// <summary>
    /// 对目录下面相应的类型asset进行操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filter"></param>
    /// <param name="callback"></param>
    /// <param name="path"></param>
    public static void WalkAllAssets<T>(string filter, Action<T> callback, params string[] path)
        where T : UnityEngine.Object{
#if UNITY_EDITOR
        try {
            string[] matGuids = null;
            if (path == null || path.Length == 0) {
                matGuids = AssetDatabase.FindAssets(filter);
            }
            else {
                matGuids = AssetDatabase.FindAssets(filter, path);
            }

            int num = matGuids.Length;
            int idx = 0;
            UnityEngine.Debug.Log("Total num " + num);
            foreach (string guid in matGuids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    callback(asset);
                }
                else {
                    Debug.LogErrorFormat("guid asset null path = {0} guid = {1}", assetPath, guid);
                }

                idx++;
                if (EditorUtility.DisplayCancelableProgressBar("Batch Modify Prefabs",
                    string.Format("{0}/{1} path = {2}", idx, num, assetPath), (idx * 1.0f / num))) {
                    UnityEngine.Debug.LogError("Cancle Task!");
                    return;
                }
            }
        }
        catch (Exception) {
            throw;
        }
        finally {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public static void WalkWithProcessBar(string path, string exts, System.Action<string> callback,string title = "Batch Modify Prefabs"){
        var count = 0;
        PathUtil.Walk(path, exts, (_cPath) => { ++count; });
        if (count == 0) return;
        var idx = 0;
        bool isNeedCancel = false;
#if UNITY_EDITOR
        try {
            PathUtil.Walk(path, exts, (_cPath) => {
                ++idx;
                if(isNeedCancel) return;
                if (EditorUtility.DisplayCancelableProgressBar(title, "Process " + path + "Exts:" + exts,
                    idx * 0.1f / count)) {
                    isNeedCancel = true;
                    return;
                }
            });
        }
        catch (Exception ) {
            throw;
        }
        finally {
            if (isNeedCancel) {
                Debug.Log($" Task {path}:{exts} Canceled ");
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public static void WalkWithProcessBar(Func<bool> callback, Func<string> processBarInfo, Func<float> process,
        string title = "Batch Modify Prefabs"){
#if UNITY_EDITOR
        try {
            while (callback()) {
                if (EditorUtility.DisplayCancelableProgressBar(title, processBarInfo(), process())) {
                    UnityEngine.Debug.LogError("Cancle Task!");
                    return;
                }
            }
        }
        catch (Exception) {
            throw;
        }
        finally {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
    }

    /// <summary>
    /// 对选中的所有gameobject 进行操作
    /// </summary>
    /// <param name="callback"></param>
    public static void WalkAllSelect(Action<GameObject> callback){
#if UNITY_EDITOR
        try {
            int num = Selection.gameObjects.Length;
            int idx = 0;
            UnityEngine.Debug.Log("Total num " + num);
            foreach (var go in Selection.gameObjects) {
                UnityEngine.Object prefab = null;
                var type = PrefabUtility.GetPrefabType(go);
                if (type == PrefabType.PrefabInstance) {
                    prefab = PrefabUtility.GetPrefabParent(go);
                }
                else if (type == PrefabType.Prefab) {
                    prefab = go;
                }

                idx++;
                if (prefab != null) {
                    callback(go);
                    if (prefab != go) {
                        PrefabUtility.ReplacePrefab(go, prefab);
                    }

                    if (EditorUtility.DisplayCancelableProgressBar("Batch Modify Prefabs",
                        string.Format("{0}/{1} path = {2}", idx, num, AssetDatabase.GetAssetPath(prefab)),
                        (idx * 1.0f / num))) {
                        UnityEngine.Debug.LogError("Cancle Task!");
                        return;
                    }
                }
            }
        }
        catch (Exception) {
            throw;
        }
        finally {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public static void WalkAllChild<T>(Transform parent, Action<T> callback) where T : Component{
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0) {
            var trans = queue.Dequeue();
            var comp = trans.gameObject.GetComponent<T>();
            if (comp != null) {
                callback(comp);
            }

            foreach (Transform childTran in trans) {
                queue.Enqueue(childTran);
            }
        }
    }

    /// <summary>
    /// 遍历相应目录下的所有prefab
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public static void WalkAllPrefab(string pPath, Func<GameObject, bool> callback, bool includechildfloder = true){
#if UNITY_EDITOR
        if (!pPath.StartsWith("/") && !pPath.Contains(Application.dataPath)) {
            pPath = Path.Combine(Application.dataPath, pPath);
        }

        try {
            bool isreturn = false;
            int num = 0;
            Action<string> GatherNum = (path) => { ++num; };
            int idx = 0;
            Action<string> DealFunc = (path) => {
                idx++;
                if (isreturn)
                    return;
                var assetPath = path.Replace(Application.dataPath, "Assets");
                assetPath = assetPath.Replace("\\", "/");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null) {
                    GameObject go = GameObject.Instantiate(prefab);
                    if (callback(go)) {
                        Debug.Log("prefab 替换成功" + prefab.name);
                        PrefabUtility.ReplacePrefab(go, prefab);
                    }

                    GameObject.DestroyImmediate(go);
                    if (EditorUtility.DisplayCancelableProgressBar("Batch Modify Prefabs",
                        string.Format("{0}/{1} path = {2}", idx, num, assetPath), (idx * 1.0f / num))) {
                        isreturn = true;
                    }
                }
            };
            PathUtil.Walk(pPath, "*.prefab", GatherNum, includechildfloder);
            UnityEngine.Debug.Log("Total num " + num);
            PathUtil.Walk(pPath, "*.prefab", DealFunc, includechildfloder);
            if (isreturn) {
                UnityEngine.Debug.LogError("Cancled Task!");
            }
        }
        catch (Exception) {
            throw;
        }
        finally {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif
    }
}