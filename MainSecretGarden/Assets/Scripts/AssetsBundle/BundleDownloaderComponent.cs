using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 用来对比web端的资源，比较md5，对比下载资源
/// </summary>
public class BundleDownloaderComponent : MonoBehaviour, IDisposable
{
    public VersionConfig remoteVersionConfig { get; private set; }

    public Queue<string> needDownLoadBundles;

    public long TotalSize;
    public BundleDownloadInfo DownloadInfo = new BundleDownloadInfo();

    public HashSet<string> downloadedBundles;

    public string downloadingBundle;

    public UnityWebRequestAsync webRequest;

    public TaskCompletionSource<bool> Tcs;

    public TaskCompletionSource<bool> FrameTcs;

    public Action<int> BundleRealProgress;

    public Action<int> BundleEachFrameProgress;

    public Action<string> FileServerNotReachCallBack;


    void Awake()
    {
        needDownLoadBundles = new Queue<string>();
        downloadedBundles = new HashSet<string>();
        downloadingBundle = "";
    }

    public string GetUrlWithPlatform(string url)
    {
#if UNITY_ANDROID
        url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#else
            url += "PC/";
#endif
        return url;
    }

    /// <summary>
    /// 下载bundle路径的根目录(StreamingAssets或Application.persistentDataPath的上一级目录)
    /// </summary>
    public string LoadBundlePathRoot()
    {
        string pathRootRemote = GlobalConfigComponent.intance.GlobalProto.GetUrl();
        string pathRootLocal = GetUrlWithPlatform(StaticData.SelfResourceServerIpAndPort + "/");
        return StaticData.IsABNotFromServer ? pathRootLocal : pathRootRemote;
    }

    /// <summary>
    /// 返回是否需要下载
    /// </summary>
    /// <returns></returns>
    public async Task<bool> LoadInfo()
    {
        UnityWebRequestAsync webRequestAsync = MonoBehaviourHelper.CreateTempComponent<UnityWebRequestAsync>();
        string remoteVersionText = string.Empty;
        try
        {
            //下载remote version.txt
            string versionUrl = LoadBundlePathRoot() + "StreamingAssets/Version.txt";
            await webRequestAsync.DownloadAsync(versionUrl);
            remoteVersionText = webRequestAsync.Request.downloadHandler.text;
        }
        catch (Exception e)
        {
            if (e.Message.Contains("request error"))
            {
                webRequestAsync.Dispose();
                Debug.Log($"load VersionText error:'{e.Message}'");
                StaticData.isUseStreamingAssetRes = true;
                OnFileServerNotReach(e.Message);
                return false;
            }
        }
        finally
        {
            Destroy(webRequestAsync.gameObject);
        }
        Debug.Log($"remoteVersionText:{remoteVersionText}");
        if (!remoteVersionText.StartsWith("{"))
        {
            Debug.Log("remote version text is not a correct json");
            this.remoteVersionConfig = null;
            return false;
        }
        this.remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(remoteVersionText);
        var needDown=await AnalyseVersionConfig();
        if (needDown == false)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 比较remoteVersionConfig和localVersionConfig,删除服务器端没有的本地ab,分析需要下载的bundle
    /// </summary>
    /// <returns></returns>
    private async Task<bool> AnalyseVersionConfig()
    {
        bool isNeedDown = true;
        VersionConfig localVersionConfig = await GetLocalVersionConfig();
        if (localVersionConfig != null)
        {
            foreach (FileVersionInfo fileVersionInfo in localVersionConfig.FileInfoDict.Values)
            {
                if (this.remoteVersionConfig.FileInfoDict.ContainsKey(fileVersionInfo.File))
                {
                    continue;
                }
                string abPath = Path.Combine(PathHelper.AppHotfixResPath, fileVersionInfo.File);
                if (File.Exists(abPath))
                    File.Delete(abPath);
            }
        }
        foreach (FileVersionInfo remoteFileVersionInfo in this.remoteVersionConfig.FileInfoDict.Values)
        {
            if (localVersionConfig != null && localVersionConfig.FileInfoDict.TryGetValue(remoteFileVersionInfo.File, out FileVersionInfo localFileVersionInfo))
            {
                if (remoteFileVersionInfo.MD5 == localFileVersionInfo.MD5)
                {
                    continue;
                }
            }
            if (remoteFileVersionInfo.File == "Version.txt")
            {
                continue;
            }
            this.needDownLoadBundles.Enqueue(remoteFileVersionInfo.File);
            this.TotalSize += remoteFileVersionInfo.Size;
        }
        DownloadInfo.TotalSize = TotalSize;
        if (DownloadInfo.TotalSize<=0)
        {
            isNeedDown = false;
        }
        return isNeedDown;
    }

    /// <summary>
    /// 返回local version config
    /// </summary>
    private async Task<VersionConfig> GetLocalVersionConfig()
    {
        VersionConfig localVersionConfig;
        string versionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
        if (File.Exists(versionPath))
        {
            localVersionConfig = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionPath));
        }
        else
        {
            versionPath = Path.Combine(PathHelper.AppResPath4Web, "Version.txt");
            UnityWebRequestAsync request = MonoBehaviourHelper.CreateTempComponent<UnityWebRequestAsync>();
            try
            {
                await request.DownloadAsync(versionPath);
                localVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
                localVersionConfig = null;
            }
            finally
            {
                Destroy(request.gameObject);
                request = null;
            }
        }
        return localVersionConfig;
    }

    private void OnFileServerNotReach(string error)
    {
        Debug.Log("资源服务器未开启");
        FileServerNotReachCallBack?.Invoke(error);
    }

    #region real download bundle
    public async Task Down()
    {
        var timeTask = DelayFrame();
        var t = this.RealDown();
        await timeTask;
        await t;
    }

    private Task<bool> DelayFrame()
    {
        FrameTcs = new TaskCompletionSource<bool>();
        UpdateFrames();
        return FrameTcs.Task;
    }

    private async void UpdateFrames()
    {
        int Frames = 0;
        while (Frames < 100)//100帧
        {
            await UniTask.Delay(20);//每帧20ms
            Frames++;
            this.BundleEachFrameProgress?.Invoke(Frames);
            //Debug.Log("Frames:"+ Frames);
        }
        this.FrameTcs.SetResult(true);
    }

    private async void UpdateAsync()
    {
        try
        {
            while (true)
            {
                if (this.needDownLoadBundles.Count == 0)
                {
                    TagDownloadFinish();
                    break;
                }

                this.downloadingBundle = this.needDownLoadBundles.Dequeue();

                await DownServerBundle();
                this.downloadedBundles.Add(this.downloadingBundle);
                this.downloadingBundle = "";
                this.webRequest = null;
            }

            using (FileStream fs = new FileStream(Path.Combine(PathHelper.AppHotfixResPath, "Version.txt"), FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonHelper.ToJson(this.remoteVersionConfig));
            }

            this.Tcs?.SetResult(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async Task DownServerBundle()
    {
        while (true)
        {
            try
            {
                this.webRequest = MonoBehaviourHelper.CreateTempComponent<UnityWebRequestAsync>();
                DownloadInfo.IsStart = true;
                var bundlePath = LoadBundlePathRoot() + "StreamingAssets/" + this.downloadingBundle;
                await this.webRequest.DownloadAsync(bundlePath);
                byte[] data = this.webRequest.Request.downloadHandler.data;
                string path = Path.Combine(PathHelper.AppHotfixResPath, this.downloadingBundle);
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                    Debug.Log($"更新Bundle:{path} 完成");
                }
                var p = this.Progress;
                BundleRealProgress?.Invoke(p);
                Destroy(this.webRequest.gameObject);
                this.webRequest = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"download bundle error: {this.downloadingBundle}\n{e}");
                //如果报错了,等1秒
                await UniTask.Delay(1);
                continue;
            }

            break;
        }
    }

    public void UpdateProgress()
    {
        var p = Progress;
    }

    private int Progress
    {
        get
        {
            if (this.remoteVersionConfig == null)
            {
                return 0;
            }
            if (this.TotalSize == 0)
            {
                return 0;
            }

            long alreadyDownloadBytes = 0;
            foreach (string downloadedBundle in this.downloadedBundles)
            {
                long size = this.remoteVersionConfig.FileInfoDict[downloadedBundle].Size;
                alreadyDownloadBytes += size;
            }
            if (this.webRequest != null)
            {
                alreadyDownloadBytes += (long)this.webRequest.Request.downloadedBytes;
            }

            var p = (int)(alreadyDownloadBytes * 100f / this.TotalSize);
            DownloadInfo.alreadyDownloadBytes = alreadyDownloadBytes;
            DownloadInfo.TotalSize = TotalSize;
            return p;
        }
    }

    private void TagDownloadFinish()
    {
        BundleRealProgress?.Invoke(100);//表示下载完了
    }

    private Task<bool> RealDown()
    {
        if (this.needDownLoadBundles.Count == 0 && this.downloadingBundle == "")
        {
            TagDownloadFinish();
            return Task.FromResult(true);
        }

        this.Tcs = new TaskCompletionSource<bool>();

        UpdateAsync();

        return this.Tcs.Task;
    }

    public void Dispose()
    {

    }
    #endregion
}