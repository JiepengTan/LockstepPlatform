using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HttpTask {
    public string url;
    public long downloadSize;
    public long totalSize;
    public float progress;
    public FileStream stream;
}

public class HttpUtil {
    private const int BufferSize = 2048;
    public static Thread downloadThread;

    
    public static void DoInit(){
        downloadThread = new Thread(ThreadUpdate);
        smp = new Semaphore(0, 1);
        downloadThread.Start();
    }

    public static void Stop(){
        if (downloadThread != null) {
            downloadThread.Abort();
        }

        downloadThread = null;
    }

    private static Semaphore smp;
    static Queue<HttpTask> tasks = new Queue<HttpTask>();

    public static void AddTask(HttpTask task){
        if (downloadThread == null) {
            Debug.LogError(" HttpUtil do not has init!");
            task.progress = 1;
            return;
        }

        lock (tasks) {
            tasks.Enqueue(task);
            smp.Release(1);
        }
    }

    static void ThreadUpdate(){
        while (true) {
            HttpTask task = null;
            lock (tasks) {
                if (tasks.Count > 0) {
                    task = tasks.Dequeue();
                }
            }

            if (task == null) {
                smp.WaitOne();
                continue;
            }
            DownLoadFile(task);
        }
    }

    static byte[] _tempBuffer = new byte[BufferSize];
    public static void DownLoadFile(HttpTask task){
        var url = task.url;
        task.progress = 0;
        FileStream outputStream = task.stream;
        WebRequest request = WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        Stream httpStream = response.GetResponseStream();
        task.totalSize = response.ContentLength;
        int readCount = httpStream.Read(_tempBuffer, 0, BufferSize);
        task.downloadSize = 0;
        var initTimer = DateTime.Now;
        while (readCount > 0) {
            outputStream.Write(_tempBuffer, 0, readCount);
            readCount = httpStream.Read(_tempBuffer, 0, BufferSize);
            task.downloadSize += readCount;
            task.progress = (1.0f * task.downloadSize) / task.totalSize;
        }
        task.progress = 1;
        httpStream.Close();
        outputStream.Close();
        response.Close();
    }
}