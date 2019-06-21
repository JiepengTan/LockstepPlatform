using UnityEngine;

namespace Lockstep.Game.Res
{
    /// <summary>
    /// 资源目录定义
    /// </summary>
    public static class AssetPathDefine
    {
        public static string dataFloderName = "data";
        public static string NoHotUpdateDataZipName = "Data.zip";
        public static string HotUpdateDataZipNamePrefix = "GUData";

        public static string HotUpdateDataZipName
        {
            get { return string.Format("{0}{1}.zip", HotUpdateDataZipNamePrefix, "GameSetting.serverIndex"); }
        }

        /// <summary>
        /// 项目中存放资源的目录
        /// </summary>
        public static string ResFolder
        {
            get { return "Assets/Res/"; }
        }

        private static string m_webBasePath = Application.dataPath + "/../HotUpdate/";

        /// <summary>
        /// 资源存放的基本目录（持久化目录）
        /// </summary>
        public static string webBasePath
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                return Application.dataPath + "/../HotUpdate/";
#else
                return Application.persistentDataPath;
#endif
            }
        }

        /// <summary>
        /// 存放下载资源的目录
        /// </summary>
        private static string m_ExternalFilePath = string.Empty;
        public static string externalFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalFilePath))
                {
                    m_ExternalFilePath = System.IO.Path.Combine(webBasePath, "http_res");
                }
                return m_ExternalFilePath;
            }
        }

        /// <summary>
        /// 存放数据文件的目录
        /// </summary>
        private static string m_ExternalDataPath = string.Empty;
        public static string externalDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalDataPath))
                {
                    m_ExternalDataPath = System.IO.Path.Combine(externalFilePath, dataFloderName.ToLower());
                }

                return m_ExternalDataPath;
            }
        }

        /// <summary>
        /// 项目中数据文件路径
        /// </summary>
        private static string m_ProjectDataPath = string.Empty;
        public static string projectDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ProjectDataPath))
                {
                    m_ProjectDataPath = ResFolder + dataFloderName;
                }
                return m_ProjectDataPath;
            }
        }

        /// <summary>
        /// Resources中的数据文件路径
        /// </summary>
        public static string resourcesDataPath
        {
            get { return "Assets/Resources/Data"; }
        }

        /// <summary>
        /// 打包后的数据文件路径(非热更数据压缩包)
        /// </summary>
        private static string m_PackedDataPath = string.Empty;
        public static string packedDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackedDataPath))
                {
                    m_PackedDataPath = dataFloder + "/" + NoHotUpdateDataZipName;
                }
                return m_PackedDataPath;
            }
        }

        /// <summary>
        /// 打包后的数据文件路径(非热更数据压缩包)
        /// </summary>
        private static string m_GUPackedDataPath = string.Empty;
        /// <summary>
        /// 热更的数据压缩包
        /// </summary>
        public static string guPackedDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_GUPackedDataPath))
                {
                    m_GUPackedDataPath = dataFloder + "/" + HotUpdateDataZipName;
                }
                return m_GUPackedDataPath;
            }
        }

        /// <summary>
        /// 数据zip目录
        /// </summary>
        public static string dataFloder
        {
            get { return Application.streamingAssetsPath + "/" + dataFloderName; }
        }

        /// <summary>
        /// 存放bundle的文件夹名
        /// </summary>
        public static string assetBundleFolder
        {
            get { return "AssetBundles"; }
        }

        /// <summary>
        /// bundle表命名
        /// </summary>
        public static string bundleTableFileName
        {
            get { return "BundleTable.json"; }
        }

        /// <summary>
        /// 依赖bundle表
        /// </summary>
        public static string depBundleTableFileName
        {
            get { return "DepBundleTable.json"; }
        }

        /// <summary>
        /// 常驻内存列表
        /// </summary>
        public static string residentBundleTableName
        {
            get { return "ResidentBundles.json"; }
        }

        /// <summary>
        /// 指定的Bundle外部路径
        /// </summary>
        private static string m_ExternalBundlePath = string.Empty;
        public static string externalBundlePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalBundlePath))
                {
                    m_ExternalBundlePath = System.IO.Path.Combine(externalFilePath, assetBundleFolder);
                }
                return m_ExternalBundlePath;
            }
        }
    }
}
