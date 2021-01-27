using System;
using UnityEngine;
#if UNITY_EDITOR
#endif
public class ABInfo : IDisposable
{
    private int refCount;
    public bool IsLoadAllAssets = false;
    public string Name { get; }

    public int RefCount
    {
        get
        {
            return this.refCount;
        }
        set
        {
            //Log.Debug($"{this.Name} refcount: {value}");
            this.refCount = value;
        }
    }

    public AssetBundle AssetBundle { get; }

    public ABInfo(string name, AssetBundle ab)
    {
        this.Name = name;
        this.AssetBundle = ab;
        this.RefCount = 1;
        //Log.Debug($"load assetbundle: {this.Name}");
    }

    public void Dispose()
    {
        this.AssetBundle?.Unload(true);
    }
}