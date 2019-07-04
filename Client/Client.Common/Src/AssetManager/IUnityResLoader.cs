using UnityEngine;
using System.Collections.Generic;

namespace Lockstep.Game.Res {
    public delegate bool IsObjectOldFunc();
    public delegate void ObjectCallback(UnityEngine.Object asset, bool isOld);
    public delegate void GameObjectCallback(UnityEngine.GameObject go, bool isOld);
    public delegate void ObjectListCallback();
    public delegate List<AssetInfo> GetAssetTableHandler();

    public interface IUnityResLoader : IService {
        void DoInit(object trans);
        void DoUpdate();
        void DoExitScene();
        bool HasAsset(int assetId);
        byte[] LoadTextBytes(string filePath);
        T LoadAsset<T>(int assetId) where T : Object;
        T[] LoadAllAsset<T>(int assetId) where T : Object;
        T LoadAsset<T>(string assetPath) where T : UnityEngine.Object;
        T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object;
        void LoadAssetAsync(int assetId, ObjectCallback callBack, IsObjectOldFunc oldFunc);
    }
}