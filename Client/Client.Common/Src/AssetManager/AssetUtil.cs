namespace Lockstep.Game.Res
{
    public class AssetUtil
    {
        public static bool IsCollectionNullOrEmpty(System.Collections.ICollection collection)
        {
            return collection == null || collection.Count <= 0;
        }

        public static string ToAssetPath(string resourcesPath, string suffix)
        {
            return $"{AssetPathDefine.ResFolder}{resourcesPath}{suffix}";
        }

        public static void OnCallBack(ObjectCallback callBack, UnityEngine.Object asset, bool isOld){
            callBack?.Invoke(asset, isOld);
        }

        public static void OnCallBack(ObjectCallback callBack, UnityEngine.Object asset, IsObjectOldFunc func)
        {
            if (callBack != null)
            {
                bool isOld = func != null && func();
                callBack(asset, isOld);
            }
        }

        public static void OnCallBack(GameObjectCallback callBack, UnityEngine.GameObject go, bool isOld){
            callBack?.Invoke(go, isOld);
        }

        public static void OnCallBack(GameObjectCallback callBack, UnityEngine.GameObject go, IsObjectOldFunc func)
        {
            if (callBack != null)
            {
                bool isOld = func != null && func();
                callBack(go, isOld);
            }
        }

        public static void OnCallBack(ObjectListCallback callBack){
            callBack?.Invoke();
        }
    }
}