using System;
using System.Collections;
using System.IO;
using Lockstep.Game;
using Lockstep.Util;
using UnityEditor;
using UnityEngine;


public class EditorLockstepPaltformSetup : UnityEditor.Editor {
    static int BufferSize = 2048;

    static IEnumerator Task;


    static string entitasUrl = "https://github.com/sschmid/Entitas-CSharp/releases/download/1.13.0/Entitas.zip";
    static string roslynUrl = "https://github.com/JiepengTan/LockstepECSGenerator/raw/master/Tools/Roslyn.zip";

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
    
    [MenuItem("Tools/Init Setup")]
    static void InitSetup(){
        HttpUtil.DoInit();
        EditorCoroutineRunner.StartEditorCoroutine(_InitSetup());
    }

    static void Stop(){
        EditorCoroutineRunner.StopEditorCoroutine(Task);
        HttpUtil.Stop();
    }
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
            ZipUtil.Decompress(zipSavePath, decompressDir);
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