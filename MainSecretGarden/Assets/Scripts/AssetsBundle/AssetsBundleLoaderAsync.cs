using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System;
public class AssetsBundleLoaderAsync : MonoBehaviour, IDisposable
{
    private AssetBundleCreateRequest request;
    private TaskCompletionSource<AssetBundle> tcs;
    private string AbPath;
    private Action<float> updateProgress;
    public void Update()
    {
        if (!this.request.isDone)
        {
            updateProgress?.Invoke(request.progress);
            return;
        }
        updateProgress?.Invoke(1);
        TaskCompletionSource<AssetBundle> t = tcs;
        t.SetResult(this.request.assetBundle);
    }

    public void Dispose()
    {

    }

    public Task<AssetBundle> LoadAsync(string path, Action<float> updateProgress = null)
    {
        this.updateProgress = updateProgress;
        this.tcs = new TaskCompletionSource<AssetBundle>();
        AbPath = path;
        ResourcesComponent resComponent = ResourcesComponent.instance;
        if (resComponent.DicABRequest.ContainsKey(path))
        {
            if (resComponent.DicABRequest[path] == null)
            {
                resComponent.DicABRequest.Remove(path);
            }
            else if (resComponent.DicABRequest[path].assetBundle == null)
            {
                resComponent.DicABRequest.Remove(path);
            }
            else
            {
                this.request = resComponent.DicABRequest[path];
                return this.tcs.Task;
            }
        }
        this.request = AssetBundle.LoadFromFileAsync(path);
        if (!resComponent.DicABRequest.ContainsKey(AbPath))
            resComponent.DicABRequest.Add(AbPath, this.request);
        return this.tcs.Task;
    }
}
