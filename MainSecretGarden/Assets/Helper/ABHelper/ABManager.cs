using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
public class ABManager
{
    public class DicKey
    {
        public string assetName;
        public Type typeAsset;
    }

    public static Dictionary<DicKey, string> DicABRelation = new Dictionary<DicKey, string>();
    public static List<ResInfo> ListABRelation = new List<ResInfo>();
    public static void Init()
    {
        ListABRelation.Clear();
        DicABRelation.Clear();
        //从ab中读取
        ResourcesComponent.instance.LoadBundle("abconfig.unity3d");
        var ABConfig = GetAssetRes<TextAsset>("ABConfig", "abconfig.unity3d");
        //string str = File.ReadAllText(Path.Combine(Application.dataPath, "Bundles/ABConfig/ABConfig.txt"));
        var abConfig = JsonUtility.FromJson<ABConfig>(ABConfig.text);
        ListABRelation = abConfig.ListABRelation;
        for (int i = 0; i < ListABRelation.Count; i++)
        {
            AddAbRelation(ListABRelation[i].assetName, ListABRelation[i].abName, Type.GetType($"{ListABRelation[i].TypeRes},UnityEngine"));
        }
    }
    private static void AddAbRelation(string assetName, string abname, Type TypeAsset)
    {
        DicKey dicKey = new DicKey() { assetName = assetName, typeAsset = TypeAsset };
        DicABRelation.Add(dicKey, abname);
    }
    public static T GetAsset<T>(string assetName) where T : UnityEngine.Object
    {
        Type t = typeof(T);
        string bundleName = GetBundleNameByAssetNameAndType(assetName, t);
        bundleName = bundleName.ToLower();
        //加载bundle
        ResourcesComponent.instance.LoadBundle(bundleName);
        return GetAssetRes<T>(assetName, bundleName);
    }

    public static T GetAssetRes<T>(string assetName, string bundleName) where T : UnityEngine.Object
    {
        assetName = assetName.ToLower();
        UnityEngine.Object resource = null;

#if UNITY_EDITOR
        if (!StaticData.isUseAssetBundle)
        {
            var s = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (s.Length == 0)
            {
                throw new Exception("bundleName " + bundleName + "  AssetName " + assetName);
            }
            resource = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(s[0]);
        }
        else
        {
            //和#else一样
            var resourcesComponent = ResourcesComponent.instance;
            var bundles = resourcesComponent.GetBundle();
            if (!bundles.ContainsKey(bundleName))
            {
                throw new Exception($"读取资源{assetName} 没有找到 bundle  {bundleName} ");
            }
            resource = bundles[bundleName].AssetBundle.LoadAsset<T>(assetName);
        }

#else
            var resourcesComponent = ResourcesComponent.instance;
            var bundles = resourcesComponent.GetBundle();
            if (!bundles.ContainsKey(bundleName))
            {
                throw new Exception($"读取资源{assetName} 没有找到 bundle  {bundleName} ");
            }
            resource = bundles[bundleName].AssetBundle.LoadAsset<T>(assetName);
#endif
        if (resource == null)
        {
            throw new Exception($"not found asset: {bundleName}/{assetName}");
        }
        string path = $"{bundleName}/{assetName}".ToLower();
        return (T)resource;
    }

    public async static Task<T> GetAssetAsync<T>(string assetName) where T : UnityEngine.Object
    {
        Type t = typeof(T);
        string bundleName = GetBundleNameByAssetNameAndType(assetName, t);
        bundleName = bundleName.ToLower();
        //加载bundle
        await ResourcesComponent.instance.LoadBundleAsync(bundleName);
        return GetAssetRes<T>(assetName, bundleName);
    }

    public static string GetBundleNameByAssetNameAndType(string assetName, Type t)
    {
        string bundleName = string.Empty;
        foreach (var item in DicABRelation)
        {
            if (item.Key.typeAsset.Equals(t) && item.Key.assetName.Equals(assetName))
            {
                bundleName = item.Value;
            }
        }
        if (string.IsNullOrEmpty(bundleName))
        {
            Debug.LogError($"{assetName}对应的包名没找到，请先检查表格，然后检查配置文件！");
        }
        return bundleName;
    }
}
