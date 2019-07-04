
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Lockstep.Game.Res
{
    public class UnityResLoaderAssetDatabase : IUnityResLoader {
        private AssetInfoManager _assetInfoManager;
        #region interface 

        public void DoInit(object trans){
            _assetInfoManager = AssetInfoManager.Instance;
        }
        public void DoUpdate() { }
        public void DoExitScene() { }

        public bool HasAsset(int assetId)
        {
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            return info != null;
        }

        public byte[] LoadTextBytes(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) { return null; }
            TextAsset asset = LoadAsset<TextAsset>(assetPath);
            if (asset == null) { return null; }
            return asset.bytes;
        }

        public T LoadAsset<T>(int assetId) where T : UnityEngine.Object
        {
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            if (info == null)
            {
                return null;
            }

            return LoadAsset<T>(info.AssetPath);
        }

        public T[] LoadAllAsset<T>(int assetId) where T : UnityEngine.Object
        {
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            if (info == null)
            {
                return null;
            }

            return LoadAllAsset<T>(info.AssetPath);
        }

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
#if UNITY_EDITOR
            if (asset is GameObject)
            {
                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                if (assetName != asset.name)
                {
                    Debug.LogErrorFormat("加载资源Asset名称和预设实际名称不一致, AssetDataBase 存在这个刷新bug, prefabName={0}, assetName={1}", asset.name, assetName);
                }
            }
#endif
            return asset;
        }

        public T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            T[] assets = null;
            if (Directory.Exists(assetPath))
            {
                assets = LoadAllFromFolder<T>(assetPath);
            }
            else
            {
                assets = LoadAllFromFile<T>(assetPath);
            }
            return assets;
        }

        public void LoadAssetAsync(int assetId, ObjectCallback callBack, IsObjectOldFunc func)
        {
            var obj = LoadAsset<UnityEngine.Object>(assetId);
            AssetUtil.OnCallBack(callBack, obj, func);
        }
        #endregion

        #region Impl
        private T[] LoadAllFromFile<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (objs is T[])
            {
                return objs as T[];
            }
            else
            {
                List<T> objList = new List<T>();
                for (int i = 0; i < objs.Length; i++)
                {
                    var obj = objs[i] as T;
                    if (obj != null)
                    {
                        objList.Add(obj);
                    }
                }
                return objList.ToArray();
            }
        }

        public T[] LoadAllFromFolder<T>(string folder) where T : UnityEngine.Object
        {
            string[] directoryEntries;
            List<T> objList = null;
            directoryEntries = System.IO.Directory.GetFileSystemEntries(folder);
            for (int i = 0; i < directoryEntries.Length; i++)
            {
                string path = directoryEntries[i].Replace("\\", "/");
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                T tempTex = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                if (tempTex != null)
                {
                    if (objList == null)
                    {
                        objList = new List<T>();
                    }
                    objList.Add(tempTex);
                }
            }
            if (objList.Count > 0)
                return objList.ToArray();
            return null;
        }
        #endregion
    }
}
#endif