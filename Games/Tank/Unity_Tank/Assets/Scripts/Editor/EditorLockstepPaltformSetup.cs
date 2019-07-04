using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using Lockstep.Game;
using Lockstep.Util;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;


public class EditorLockstepPaltformSetup : UnityEditor.Editor {
    static int BufferSize = 2048;

    /// <summary>
    /// 解压文件到指定文件夹
    /// </summary>
    /// <param name="sourceFile">压缩文件</param>
    /// <param name="destinationDirectory">目标文件夹，如果为空则解压到当前文件夹下</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public static bool Decompress(string sourceFile, string destinationDirectory = null, string password = null){
        bool result = false;

        if (!File.Exists(sourceFile)) {
            throw new FileNotFoundException("要解压的文件不存在", sourceFile);
        }

        if (string.IsNullOrWhiteSpace(destinationDirectory)) {
            destinationDirectory = Path.GetDirectoryName(sourceFile);
        }

        try {
            if (!Directory.Exists(destinationDirectory)) {
                Directory.CreateDirectory(destinationDirectory);
            }

            var stream = File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (ZipInputStream zipStream = new ZipInputStream(stream)) {
                zipStream.Password = password;
                ZipEntry zipEntry = zipStream.GetNextEntry();

                while (zipEntry != null) {
                    if (zipEntry.IsDirectory) //如果是文件夹则创建
                    {
                        Directory.CreateDirectory(Path.Combine(destinationDirectory,
                            Path.GetDirectoryName(zipEntry.Name)));
                    }
                    else {
                        string fileName = Path.GetFileName(zipEntry.Name);
                        if (!string.IsNullOrEmpty(fileName) && fileName.Trim().Length > 0) {
                            FileInfo fileItem = new FileInfo(Path.Combine(destinationDirectory, zipEntry.Name));
                            using (FileStream writeStream = fileItem.Create()) {
                                byte[] buffer = new byte[BufferSize];
                                int readLength = 0;

                                do {
                                    readLength = zipStream.Read(buffer, 0, BufferSize);
                                    writeStream.Write(buffer, 0, readLength);
                                } while (readLength == BufferSize);

                                writeStream.Flush();
                                writeStream.Close();
                            }

                            fileItem.LastWriteTime = zipEntry.DateTime;
                        }
                    }

                    zipEntry = zipStream.GetNextEntry(); //获取下一个文件
                }

                zipStream.Close();
            }

            result = true;
        }
        catch (System.Exception ex) {
            throw new Exception("文件解压发生错误", ex);
        }

        return result;
    }

    [MenuItem("Tools/Init Setup")]
    static void InitSetup(){
        HttpUtil.DoInit();
        EditorCoroutineRunner.StartEditorCoroutine(_InitSetup());
    }

    static IEnumerator Task;

    static void Stop(){
        EditorCoroutineRunner.StopEditorCoroutine(Task);
        HttpUtil.Stop();
    }

    static string entitasUrl = "https://github.com/sschmid/Entitas-CSharp/releases/download/1.13.0/Entitas.zip";
    static string roslynUrl = "https://github.com/JiepengTan/LockstepECSGenerator/raw/master/Tools/Roslyn.zip";

    static IEnumerator _InitSetup(){
        bigProgress = 0.8f;
        //yield return DownLoadFile(roslynUrl);
        bigProgress = 1f;
        if (isStop) yield break;
        yield return DownLoadFile(entitasUrl);
        EditorUtil.ShowMessage("Done");
        Debug.Log("Done ");
    }

    private static string tip = "";

    static IEnumerator DownLoadFile(string url){
        var fileName = Path.GetFileName(url);
        var zipSavePath = Path.Combine(Application.dataPath, "../" + fileName);
        if (File.Exists(zipSavePath))
            File.Delete(zipSavePath);
        var stream = new FileStream(zipSavePath, FileMode.OpenOrCreate);
        var task = new HttpTask() {
            stream = stream,
            url = url
        };

        HttpUtil.AddTask(task);
        while (task.progress < 1) {
            progress = 0.8f * task.progress;
            if (ShowProgress(
                $"下载文件 {fileName} {task.downloadSize / 1024}/{task.totalSize / 1024}KB", progress)) {
                yield break;
            }

            yield return null;
        }

        if (isStop) yield break;
        try {
            ShowProgress("解压中", progress);

            var decompressDir = Path.Combine(Application.dataPath, "../" + Path.GetFileNameWithoutExtension(url));
            if (Directory.Exists(decompressDir))
                Directory.Delete(decompressDir, true);
            Decompress(zipSavePath, decompressDir);
            File.Delete(zipSavePath);
            progress = 0.9f;
            ShowProgress("完成", progress);
        }
        catch (Exception e) {
            Debug.LogError("Failed !" + e);
        }
        finally {
            EditorUtility.ClearProgressBar();
        }
    }

    private static float progress = 0;
    private static float _lastBigProgress = 0;
    private static float _bigProgress = 0;

    private static float bigProgress {
        set {
            _lastBigProgress = _bigProgress;
            _bigProgress = value;
        }
    }

    private static bool isStop = false;

    static bool ShowProgress(string info, float progress){
        if (EditorUtility.DisplayCancelableProgressBar("Init LockstepPlatform ...", info,
            _lastBigProgress + progress * (_bigProgress - _lastBigProgress)
        )) {
            Stop();
            isStop = true;
            EditorUtility.ClearProgressBar();
            Debug.LogError("Canceled  !!");
            return true;
        }

        return false;
    }
}