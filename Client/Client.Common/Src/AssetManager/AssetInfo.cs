#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lockstep.Game.Res
{
    public class AssetInfo
    {
        private string _folderPath = null;
        public string AssetName { get; private set; }
        public string Suffix { get; private set; }
        public int Id { get; private set; }
        public AssetInfo(int id, string folder, string assetName, string suffix)
        {
            this.Id = id;
            this._folderPath = folder.Replace('_', '/');
            this.AssetName = assetName;
            this.Suffix = suffix;
        }

        private string _resourcesPath = null;
        public string ResourcesPath
        {
            get
            {
                if (_resourcesPath == null)
                {
                    _resourcesPath = $"{_folderPath}/{AssetName}";
                }
                return _resourcesPath;
            }
        }

        private string _assetPath = null;
        public string AssetPath
        {
            get
            {
                if (_assetPath == null)
                {
                    _assetPath = $"{AssetPathDefine.ResFolder}{ResourcesPath}{Suffix}";
                }
                return _assetPath;
            }
        }

        public override string ToString()
        {
            return $"id={Id}, assetPath={AssetPath}";
        }
    }
}