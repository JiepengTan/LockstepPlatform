using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using Lockstep.Util;

namespace Lockstep.Game.Res {
    public class UnityResLoaderResources : IUnityResLoader {
        #region

        public class AssetLoaderTask {
            public AssetInfo info;
            public ObjectCallback callBack;
            public IsObjectOldFunc func;

            public AssetLoaderTask(AssetInfo info, ObjectCallback callBack, IsObjectOldFunc func){
                this.info = info;
                this.callBack = callBack;
                this.func = func;
            }
        }

        private Queue<AssetLoaderTask> pengdingLoadQueue = new Queue<AssetLoaderTask>();

        private const int MAX_ASYNC_LOADING_COUNT = 5;

        private const float MAX_LAOD_TIME = 10;

        private int _curLoadingCount = 0;

        private AssetInfoManager _assetInfoManager;

        #endregion

        #region Interface 

        public void DoInit(object trans){
            _assetInfoManager = AssetInfoManager.Instance;
        }

        public void DoUpdate(){ }

        public void DoExitScene(){
            pengdingLoadQueue.Clear();
            _curLoadingCount = 0;
        }

        public bool HasAsset(int assetId){
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            return info != null;
        }

        public byte[] LoadTextBytes(string resourcesPath){
            if (string.IsNullOrEmpty(resourcesPath)) {
                return null;
            }

            TextAsset asset = LoadAsset<TextAsset>(resourcesPath);
            if (asset == null) {
                return null;
            }

            return asset.bytes;
        }

        public T LoadAsset<T>(int assetId) where T : UnityEngine.Object{
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            if (info == null) {
                return null;
            }

            return LoadAsset<T>(info.ResourcesPath);
        }

        public T[] LoadAllAsset<T>(int assetId) where T : UnityEngine.Object{
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            return Resources.LoadAll<T>(info.ResourcesPath);
        }

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object{
            return Resources.Load<T>(assetPath);
        }

        public T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object{
            return Resources.LoadAll<T>(assetPath);
        }

        public void LoadAssetAsync(int assetId, ObjectCallback callBack, IsObjectOldFunc func){
            AssetInfo info = _assetInfoManager.GetAssetInfo(assetId);
            if (info == null) {
                AssetUtil.OnCallBack(callBack, null, false);
                return;
            }

            if (_curLoadingCount < MAX_ASYNC_LOADING_COUNT) {
                StartLoadAsync(info.ResourcesPath, callBack, func);
            }
            else {
                AssetLoaderTask task = new AssetLoaderTask(info, callBack, func);
                //添加异步加载任务
                pengdingLoadQueue.Enqueue(task);
            }
        }

        #endregion

        #region Impl

        private void StartLoadAsync(string resourcesPath, ObjectCallback callBack, IsObjectOldFunc oldFunc){
            _curLoadingCount++;
            CoroutineHelper.StartCoroutine(LoadAssetCoroutine(resourcesPath, (asset, isOld) => {
                AssetUtil.OnCallBack(callBack, asset, isOld);
                OnLoadFinishAndCheckNext();
            }, oldFunc));
        }

        private void OnLoadFinishAndCheckNext(){
            if (_curLoadingCount > 0)
                _curLoadingCount--;

            if (_curLoadingCount < MAX_ASYNC_LOADING_COUNT && pengdingLoadQueue.Count > 0) {
                AssetLoaderTask task = pengdingLoadQueue.Dequeue();
                StartLoadAsync(task.info.ResourcesPath, task.callBack, task.func);
            }
        }

        private IEnumerator LoadAssetCoroutine(string path, ObjectCallback callBack, IsObjectOldFunc func){
            ResourceRequest request = Resources.LoadAsync(path);
            float startTime = Time.time;
            while (request.isDone == false) {
                if ((Time.time - startTime) >= MAX_LAOD_TIME) {
                    Debug.LogError("LoadAsset time out" + path);
                    break;
                }

                yield return null;
            }

            UnityEngine.Object prefab = request.asset;
            AssetUtil.OnCallBack(callBack, prefab, func);
        }

        #endregion
    }
}