using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using Lockstep.Game;

namespace Lockstep.Game.Res {
    public class AssetInfoManager {
        public static AssetInfoManager _Instance = new AssetInfoManager();
        public static AssetInfoManager Instance => _Instance;
        private Dictionary<int, AssetInfo> _assetInfoDict = new Dictionary<int, AssetInfo>();

        public bool HasInited {
            get { return _assetInfoDict.Count > 0; }
        }

        public void Init(bool force = false){
            if (force == false && HasInited) {
                return;
            }

            List<AssetInfo> assetInfoList = GetAllAssetInfo();
            Clear();
            foreach (var item in assetInfoList) {
                int key = item.Id;
#if UNITY_EDITOR
                if (_assetInfoDict.ContainsKey(key)) {
                    Debug.LogError("has same Key = " + key);
                    continue;
                }
#endif
                _assetInfoDict.Add(key, item);
            }
        }

        public void Clear(){
            _assetInfoDict.Clear();
        }

        /// <summary>获取资源数据</summary>
        public AssetInfo GetAssetInfo(int assetId){
#if UNITY_EDITOR
            if (!HasInited) {
                Debug.LogError("AssetTable is empty,maybe you should call AssetInfo.Init() first");
                return null;
            }
#endif
            AssetInfo info = null;
            if (!_assetInfoDict.TryGetValue(assetId, out info)) {
                Debug.LogError("Can not find AssetInfo ID = " + assetId);
            }

            return info;
        }

        private List<AssetInfo> GetAllAssetInfo(){
            List<AssetInfo> ret = new List<AssetInfo>();
            return ret;
        }
    }

    public class AssetManager : IUnityResLoader {
        private UnityResLoaderResources _loaderResources = new UnityResLoaderResources();
        private IUnityResLoader _loaderReal = null;
        public bool IsForceUseBundleMode = false;

        #region Interface IResLoader
        public void DoInit(object trans){
            AssetInfoManager.Instance.Init();
#if UNITY_EDITOR
            if (IsForceUseBundleMode) {
                _loaderReal = new UnityResLoaderAssetBundle();
            }
            else {
                _loaderReal = new UnityResLoaderAssetDatabase();
            }
#else
            _loaderReal = new UnityResLoaderAssetBundle();
#endif
            _loaderResources.DoInit(trans);
            _loaderReal.DoInit(trans);
        }

        public void DoUpdate(){
            _loaderResources.DoUpdate();
            _loaderReal.DoUpdate();
        }

        public void DoExitScene(){
            _loaderResources.DoExitScene();
            _loaderReal.DoExitScene();
        }

        public byte[] LoadTextBytes(string filePath){
            byte[] ret = null;
            ret = _loaderReal.LoadTextBytes(filePath);
            if (ret == null) {
                ret = _loaderResources.LoadTextBytes(filePath);
            }

            return ret;
        }

        public bool HasAsset(int assetId){
            bool ret = false;
            ret = _loaderReal.HasAsset(assetId);
            if (!ret) {
                ret = _loaderResources.HasAsset(assetId);
            }

            return ret;
        }

        public T LoadAsset<T>(int assetId) where T : Object{
            var obj = GetFromCache(assetId);
            if (obj != null) {
                return obj as T;
            }

            T ret = null;
            ret = _loaderReal.LoadAsset<T>(assetId);
            if (ret == null) {
                ret = _loaderResources.LoadAsset<T>(assetId);
            }

            return ret;
        }

        public T[] LoadAllAsset<T>(int assetId) where T : Object{
            T[] ret = null;
            ret = _loaderReal.LoadAllAsset<T>(assetId);
            if (ret == null) {
                ret = _loaderResources.LoadAllAsset<T>(assetId);
            }

            return ret;
        }

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object{
            var obj = GetFromCache(assetPath);
            if (obj != null) {
                return obj as T;
            }

            T ret = null;
            ret = _loaderReal.LoadAsset<T>(assetPath);
            if (ret == null) {
                ret = _loaderResources.LoadAsset<T>(assetPath);
            }

            return ret;
        }

        public T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object{
            T[] ret = null;
            ret = _loaderReal.LoadAllAsset<T>(assetPath);
            if (ret == null) {
                ret = _loaderResources.LoadAllAsset<T>(assetPath);
            }

            return ret;
        }


        public void LoadAssetAsync(int assetId, ObjectCallback callBack, IsObjectOldFunc func){
            var obj = GetFromCache(assetId);
            if (obj != null) {
                AssetUtil.OnCallBack(callBack, obj, func);
                return;
            }

            ObjectCallback proxyCallback = (asset, isOld) => {
                if (asset != null) {
                    if (!_cacheAssets.ContainsKey(assetId)) {
                        PushToCache(assetId, asset);
                    }
                }

                AssetUtil.OnCallBack(callBack, asset, isOld);
            };
            _loaderReal.LoadAssetAsync(assetId, proxyCallback, func);
        }

        #endregion

        /// <summary> 缓存资源列表 key:AssetID</summary>
        private Dictionary<int, UnityEngine.Object> _cacheAssets = new Dictionary<int, UnityEngine.Object>();

        private Dictionary<string, UnityEngine.Object> _cacheAssetsStr = new Dictionary<string, UnityEngine.Object>();

        private Object GetFromCache(int assetId){
            Object info = null;
            if (!_cacheAssets.TryGetValue(assetId, out info)) {
                return null;
            }

            return info;
        }

        private Object GetFromCache(string assetPath){
            Object info = null;
            if (!_cacheAssetsStr.TryGetValue(assetPath, out info)) {
                return null;
            }

            return info;
        }

        private void PushToCache(int assetId, Object obj){
            if (obj == null) return;
            _cacheAssets[assetId] = obj;
            var path = GetPathFromID(assetId);
            PushToCache(path, obj);
        }

        private void PushToCache(string assetPath, Object obj){
            if (obj == null) return;
            _cacheAssetsStr[assetPath] = obj;
        }

        private string GetPathFromID(int assetID){
            return null;
        }

        private void ClearCaches(){
            _cacheAssets.Clear();
            _cacheAssetsStr.Clear();
        }
    }
}