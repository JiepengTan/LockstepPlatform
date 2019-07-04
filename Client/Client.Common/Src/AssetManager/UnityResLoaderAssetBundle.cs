using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Game.Res;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Lockstep.Game.Res
{
    public class UnityResLoaderAssetBundle : IUnityResLoader
    {
        public void DoInit(object trans) { }
        public void DoUpdate() { }
        public void DoExitScene() { }
        public byte[] LoadTextBytes(string filePath) { return null; }
        public bool HasAsset(int assetId) { return false; }

        public T LoadAsset<T>(int assetId) where T : Object { return null; }

        public T[] LoadAllAsset<T>(int assetId) where T : Object { return null; }
        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object { return null; }
        public T[] LoadAllAsset<T>(string assetPath) where T : UnityEngine.Object { return null; }

        public void LoadAssetAsync(int assetId, ObjectCallback callBack, IsObjectOldFunc func) { }
    }
}