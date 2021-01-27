using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class UnityWebRequestAsync : MonoBehaviour,IDisposable
{
    public UnityWebRequest Request;

    public bool isCancel;

    public TaskCompletionSource<bool> tcs;

    public void Dispose()
    {

        this.Request?.Dispose();
        this.Request = null;
        this.isCancel = false;
    }

    public float Progress
    {
        get
        {
            if (this.Request == null)
            {
                return 0;
            }
            return this.Request.downloadProgress;
        }
    }

    public ulong ByteDownloaded
    {
        get
        {
            if (this.Request == null)
            {
                return 0;
            }
            return this.Request.downloadedBytes;
        }
    }

    public void Update()
    {
        if (this.Request == null)
        {
            return;
        }
        if (this.isCancel)
        {
            this.tcs.SetResult(false);
            return;
        }

        if (!this.Request.isDone)
        {
            return;
        }
        if (!string.IsNullOrEmpty(this.Request.error))
        {
            this.tcs.SetException(new Exception($"request error: {this.Request.error}"));
            return;
        }
        if (!this.tcs.Task.IsCompleted)
        {
            this.tcs.SetResult(true);
        }
    }

    public Task<bool> DownloadAsync(string url)
    {
        this.tcs = new TaskCompletionSource<bool>();

        url = url.Replace(" ", "%20");
        this.Request = UnityWebRequest.Get(url);
        Debug.Log("下载: " + url);
        //ZLog.Info("UnityWebRequest 默认超时: ", Request.timeout);
        //Request.timeout = 10;
        this.Request.SendWebRequest();

        return this.tcs.Task;
    }
}
