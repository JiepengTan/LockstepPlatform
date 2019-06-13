using System;
using System.IO;
using System.Linq;
using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {

    
    public static class DirUtil {
        public static Vector2Int GetDirVec(EDir dir){
            switch (dir) {
                case EDir.Up: return Vector2Int.up;
                case EDir.Right: return Vector2Int.right;
                case EDir.Down: return Vector2Int.down;
                case EDir.Left: return Vector2Int.left;
            }

            return Vector2Int.up;
        }
        public static LVector2 GetDirLVec(EDir dir){
            switch (dir) {
                case EDir.Up: return LVector2.up;
                case EDir.Right: return LVector2.right;
                case EDir.Down: return LVector2.down;
                case EDir.Left: return LVector2.left;
            }

            return LVector2.up;
        }
        public static int GetDirDeg(EDir dir){
            return ((int) dir) * 90;
        }

        public static Vector2Int GetBorderDir(EDir dir){
            var isUpDown = (int) (dir) % 2 == 0;
            var borderDir = Vector2Int.up;
            if (isUpDown) {
                borderDir = Vector2Int.right;
            }

            return borderDir;
        }
           // 遍历所选目录或文件，递归
    public static void Walk(string path, string exts, System.Action<string> callback, bool _is_save_assets = false, bool _is_all_directories = true)
    {
        bool isAll = string.IsNullOrEmpty(exts) || exts == "*" || exts == "*.*";
        string[] extList = exts.Replace("*", "").Split('|');

        if (Directory.Exists(path))
        {
            // 如果选择的是文件夹
            SearchOption searchOption = _is_all_directories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] files = Directory.GetFiles(path, "*.*", searchOption).Where(file =>
            {
                if (isAll)
                    return true;
                foreach (var ext in extList)
                {
                    if (file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }).ToArray();

            foreach (var item in files)
            {
                if (callback != null)
                {
                    callback(item);
                }
            }
            if (_is_save_assets)
            {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }
        }
        else
        {
            if (isAll)
            {
                if (callback != null)
                {
                    callback(path);
                }
            }
            else
            {
                // 如果选择的是文件
                foreach (var ext in extList)
                {
                    if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        if (callback != null)
                        {
                            callback(path);
                        }
                    }
                }
            }
            if (_is_save_assets)
            {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }
        }
    }

    }
}