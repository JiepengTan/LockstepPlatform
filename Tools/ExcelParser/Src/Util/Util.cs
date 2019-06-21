using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSVGenCode {

    public class Util {

        public static void Walk(string path, string exts, System.Action<string> callback, bool isEditor = false) {
            bool isAll = string.IsNullOrEmpty(exts) || exts == "*" || exts == "*.*";
            string[] extList = exts.Replace("*", "").Split('|');

            if (Directory.Exists(path)) {
                // 如果选择的是文件夹

                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(file => {
                    if (isAll)
                        return true;
                    foreach (var ext in extList) {
                        if (file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                            return true;
                        }
                    }
                    return false;
                }).ToArray();

                foreach (var item in files) {
                    if (callback != null) {
                        callback(item);
                    }
                }
                if (isEditor) {
#if UNITY_EDITOR
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#endif
                }
            } else {
                if (isAll) {
                    if (callback != null) {
                        callback(path);
                    }
                } else {
                    // 如果选择的是文件
                    foreach (var ext in extList) {
                        if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                            if (callback != null) {
                                callback(path);
                            }
                        }
                    }
                }
                if (isEditor) {
#if UNITY_EDITOR
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#endif
                }
            }
        }
    }
}
