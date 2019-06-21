using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Lockstep.Game {
    public static partial class UnityPathHelper {
        public static string GetFilePath(string dirName, string fileName){
            var defaultDir = GetStreamingAssetsPath();
            var dir = System.IO.Path.Combine(defaultDir, dirName.Replace("\\", "/"));
            var path = System.IO.Path.Combine(dir, fileName).Replace("\\", "/");
            return path;
        }

        public static string GetFilePath(string path){
            var defaultDir = GetStreamingAssetsPath();
            return System.IO.Path.Combine(defaultDir, path.Replace("\\", "/"));
        }


        public static string GetFileName(string path){
            path = path.Replace("\\", "/");
            path = path.Remove(0, path.LastIndexOf("/", StringComparison.Ordinal) + 1);
            return path.Remove(path.LastIndexOf(".", StringComparison.Ordinal));
        }

        public static string GetFileNameWithPostfix(string path){
            path = path.Replace("\\", "/");
            return path.Remove(0, path.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        public static void DeleteFile(string deletePath, bool isRefresh = true){
            var path = deletePath;
            if (deletePath.IndexOf(Application.dataPath, StringComparison.Ordinal) == -1) {
                path = System.IO.Path.Combine(Application.dataPath, deletePath);
            }

            if (System.IO.File.Exists(path)) {
                System.IO.File.Delete(path);
            }
#if UNITY_EDITOR
            if (isRefresh)
                AssetDatabase.Refresh();
#endif
        }

        public static void DeleteDir(string dir, bool isRefresh = true){
            if (System.IO.Directory.Exists(dir)) {
                System.IO.Directory.Delete(dir, true);
            }
#if UNITY_EDITOR
            if (isRefresh)
                AssetDatabase.Refresh();
#endif
        }

        public static void SaveToFile(string SavePath, string content, bool isRefresh = true){
            var path = System.IO.Path.Combine(Application.dataPath, SavePath);
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir)) {
                System.IO.Directory.CreateDirectory(dir);
            }

            System.IO.File.WriteAllText(path, content);
#if UNITY_EDITOR
            if (isRefresh)
                AssetDatabase.Refresh();
#endif
        }


        public static string GetStreamingAssetsPath(){
            var path = "";
#if UNITY_EDITOR
            path = Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
            path = Application.dataPath + "/Raw/"; 
#elif UNITY_ANDROID
            path = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_STANDALONE_WIN
            path = Application.dataPath + "/StreamingAssets/";
#endif
            return path;
        }

    }
}