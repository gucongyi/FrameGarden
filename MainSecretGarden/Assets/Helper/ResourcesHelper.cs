using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public static class ResourcesHelper
{
    public static UnityEngine.Object Load(string path)
    {
        return Resources.Load(path);
    }
    public static GameObject InstantiatePrefabFromResourceSetDefault(string path,Transform parent=null)
    {
        if (parent == null)
        {
            parent = UIRoot.instance.GetUIRootCanvas().transform;
        }
        var prefab = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.SetRectTransformStretchAllWithParent(parent);
        return go;
    }
    public static GameObject InstantiatePrefabFromABSetDefault(GameObject goAB, Transform parent= null)
    {
        if (parent==null)
        {
            parent = UIRoot.instance.GetUIRootCanvas().transform;
        }
        GameObject go = GameObject.Instantiate(goAB);
        go.transform.SetRectTransformStretchAllWithParent(parent);
        return go;
    }

    public static GameObject InstantiatePrefabFromABSetDefaultNotStretch(GameObject goAB, Transform parent)
    {
        GameObject go = GameObject.Instantiate(goAB);
        go.transform.SetTransformDefalutWithParentNotStretch(parent);
        return go;
    }
    

    

    public static string[] GetDependencies(string assetBundleName)
    {
        string[] dependencies = new string[0];
        if (!StaticData.isUseAssetBundle)
        {
#if UNITY_EDITOR
            dependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, true);
#endif
        }
        else
        {
            dependencies = ResourcesComponent.AssetBundleManifestObject.GetAllDependencies(assetBundleName);
        }
        return dependencies;
    }

    public static string[] GetSortedDependencies(string assetBundleName)
    {
        Dictionary<string, int> info = new Dictionary<string, int>();
        List<string> parents = new List<string>();
        CollectDependencies(parents, assetBundleName, info);
        string[] ss = info.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
        return ss;
    }

    public static void CollectDependencies(List<string> parents, string assetBundleName, Dictionary<string, int> info)
    {
        parents.Add(assetBundleName);
        string[] deps = GetDependencies(assetBundleName);
        foreach (string parent in parents)
        {
            if (!info.ContainsKey(parent))
            {
                info[parent] = 0;
            }
            info[parent] += deps.Length;
        }


        foreach (string dep in deps)
        {
            if (parents.Contains(dep))
            {
                throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
            }
            CollectDependencies(parents, dep, info);
        }
        parents.RemoveAt(parents.Count - 1);
    }
}
