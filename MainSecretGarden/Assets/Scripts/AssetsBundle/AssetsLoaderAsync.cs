using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;



public class AssetsLoaderAsync : MonoBehaviour, IDisposable
{
    private AssetBundle assetBundle;

    private AssetBundleRequest request;

    private TaskCompletionSource<bool> tcs;
    private Action<float> actionProgress;

    public void SetAB(AssetBundle ab)
    {
        this.assetBundle = ab;
    }

    public void Update()
    {
        if (!this.request.isDone)
        {
            actionProgress?.Invoke(request.progress);
            return;
        }
        actionProgress?.Invoke(1);
        TaskCompletionSource<bool> t = tcs;
        t.SetResult(true);
    }

    public void Dispose()
    {
        this.assetBundle = null;
        this.request = null;
    }

    public async Task<UnityEngine.Object[]> LoadAllAssetsAsync()
    {
        await InnerLoadAllAssetsAsync();
        return this.request.allAssets;
    }

    public async Task<UnityEngine.Object[]> LoadAllAssetsAsync(Action<float> actionProgress)
    {
        this.actionProgress = actionProgress;
        await InnerLoadAllAssetsAsync();
        return this.request.allAssets;
    }

    private Task<bool> InnerLoadAllAssetsAsync()
    {
        this.tcs = new TaskCompletionSource<bool>();
        this.request = assetBundle.LoadAllAssetsAsync();
        return this.tcs.Task;
    }

    public async Task<UnityEngine.Object[]> LoadAllAssetsAsync<T>()
    {
        await InnerLoadAllAssetsAsync<T>();
        return this.request.allAssets;
    }

    private Task<bool> InnerLoadAllAssetsAsync<T>()
    {
        this.tcs = new TaskCompletionSource<bool>();
        this.request = assetBundle.LoadAllAssetsAsync();
        return this.tcs.Task;
    }
}
