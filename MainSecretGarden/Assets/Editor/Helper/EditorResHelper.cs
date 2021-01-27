using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorResHelper
{
    /// <summary>
    /// 获取文件夹内所有的预制跟场景路径
    /// </summary>
    /// <param name="srcPath">源文件夹</param>
    /// <param name="subDire">是否获取子文件夹</param>
    /// <returns></returns>
    public static List<string> GetPrefabsAndScenes(string srcPath)
    {
        List<string> paths = new List<string>();
        FileHelper.GetAllFiles(paths, srcPath);

        List<string> files = new List<string>();
        foreach (string str in paths)
        {
            if (str.EndsWith(".prefab") || str.EndsWith(".unity") || str.EndsWith(".bytes"))
            {
                files.Add(str);
            }
        }
        return files;
    }

    /// <summary>
    /// 获取文件夹内所有资源路径
    /// </summary>
    /// <param name="srcPath">源文件夹</param>
    /// <param name="subDire">是否获取子文件夹</param>
    /// <returns></returns>
    public static List<string> GetAllResourcePath(string srcPath, bool subDire)
    {
        List<string> paths = new List<string>();
        string[] files = Directory.GetFiles(srcPath);
        foreach (string str in files)
        {
            if (str.EndsWith(".meta"))
            {
                continue;
            }
            paths.Add(str);
        }
        if (subDire)
        {
            foreach (string subPath in Directory.GetDirectories(srcPath))
            {
                List<string> subFiles = GetAllResourcePath(subPath, true);
                paths.AddRange(subFiles);
            }
        }
        return paths;
    }

    private static List<TextureImporterType> findTextureType = new List<TextureImporterType>() {
             TextureImporterType.Sprite,
             //TextureImporterType.Default,
        };

    private static List<string> findTextureRejectPath = new List<string>() {
             "UIResource/Anim",
             "UIResource/CustomFont",
             "UIResource/Effect",
             "UIResource/NumberFont",
             "UIResource/TimeLine",
             "UIResource/Video",
        };
    [MenuItem("Tools/查找替换黑底")]
    public static void FindBlack()
    {
        var tag = "BgBlack";
        var commonSprite = AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>("Assets/UIResource/CommonUI/BgMask.png");
        var s = new UnityEngine.Vector2(1280, 720);
        var prefabs = UIPrefabs();
        //ZLog.Info(prefabs[0]);
        for (int j = 0; j < prefabs.Count; j++)
        {
            var prefab = prefabs[j];
            var ui = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefab);
            var images = ui.transform.GetComponentsInChildren<UnityEngine.UI.Image>(true);
            var prefabName = Path.GetFileName(prefab);
            for (int i = 0; i < images.Length; i++)
            {
                var item = images[i];
                if (item.rectTransform.sizeDelta == s)
                {
                    if (item.sprite != null)
                    {
                        if (item.GetComponent<UnityEngine.UI.Mask>() != null) continue;
                        if (item.sprite.name.Contains("mask") || item.sprite.name.Contains("Mask"))
                        {

                        }
                        else
                        {
                            continue;
                            //ZLog.Info($"找到 {item.name} 引用 {item.sprite.name}  {prefabName}");
                        }
                        //if (item.sprite.rect.size != s)
                        //{
                        //    if (item.sprite.name != "BgMask")
                        //    {
                        //        ZLog.Info($"尺寸不一样的 {item.name}  {item.sprite.name}  {prefabName}");
                        //        continue;
                        //    }
                        //}
                        if (item.tag == tag)
                        {
                            if (item.sprite != commonSprite)
                            {
                                item.sprite = commonSprite;
                                Debug.Log($"手动指定tag的修改sprite图片 {item.name}  {prefabName}");
                            }
                            continue;
                        };
                        item.tag = tag;
                        item.sprite = commonSprite;
                        //item.sprite = 
                        Debug.Log($"设置 {item.name} 引用 {item.sprite.name}  {prefabName}");
                    }
                    else
                    {
                        //ZLog.Error($"找到 {item.name} {prefabName} 未设置 sprite");

                    }
                }
                else
                {
                    if (item.sprite != null &&
                        (item.sprite.name.Contains("mask")
                        || item.sprite.name.Contains("Mask")
                        ))
                    {
                        if (item.GetComponent<UnityEngine.UI.Mask>() != null) continue;
                        //ZLog.Error($"尺寸不对? {item.name} 引用 {item.sprite.name}  " +
                        //    $"{prefabName} size {item.sprite.rect.size}" +
                        //    $"sizeDelta {item.rectTransform.sizeDelta}");
                    }
                }
            }
        }

        //ZLog.Info($"{prefabs[0]}  count: {images.Length}");

    }
    [MenuItem("Tools/查找替换字体")]
    public static void ReplaceFontStyle()
    {
        var newFontStyle = AssetDatabase.LoadAssetAtPath<Font>("Assets/Bundles/Font/YangRenDongZhuShiTi-Light-2.ttf");
        var oldFontStyle = AssetDatabase.LoadAssetAtPath<Font>("Assets/Bundles/Font/STZHONGS.TTF");
        var prefabs = UIAllInAssets();
        int amount = 0;
        for (int j = 0; j < prefabs.Count; j++)
        {
            var prefab = prefabs[j];
            var ui = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
            var texts = ui.transform.GetComponentsInChildren<UnityEngine.UI.Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                var item = texts[i];
                if (item.font == oldFontStyle) 
                {
                    amount += 1;
                    item.font = newFontStyle;
                    EditorUtility.SetDirty(ui);
                }

            }
        }
        Debug.Log($"字体替换完成，共替换了{amount}个text中的font！！！");
    }


    [MenuItem("Tools/查找没有引用的ui图片")]
    public static void FindNoRefrenceSprite()
    {
        var prefabs = UIPrefabs();
        var sprites = UISprites();
        var dependencies = AssetDatabase.GetDependencies(prefabs.ToArray());
        foreach (var item in dependencies)
        {
            if (!item.EndsWith(".png"))
                continue;
            if (!item.Contains("UIResource"))
                continue;
            if (!OkType(item))
                continue;
            if (!sprites.Remove(item))
                UnityEngine.Debug.LogError("依赖的图片在UIResource里面没有:" + item);
        }
        //丢弃 排除一些路径
        foreach (var item in findTextureRejectPath)
        {
            sprites.RemoveAll(x => x.Contains(item));
        }
        foreach (var item in sprites)
        {
            UnityEngine.Debug.LogError("没有引用的ui图片:" + item);
        }
        UnityEngine.Debug.LogError("完成");
    }

    private static List<string> UISprites()
    {
        List<string> paths = new List<string>();
        var temp = GetAllResourcePath(UnityEngine.Application.dataPath + @"\UIResource", true);
        foreach (var item in temp)
        {
            if (!item.EndsWith(".png"))
            {
                continue;
            }
            var path = ProcessPath(item);
            if (!OkType(path))
                continue;
            paths.Add(path);
        }
        return paths;
    }

    private static List<string> UIPrefabs()
    {
        List<string> paths = new List<string>();

        var temp = GetAllResourcePath(UnityEngine.Application.dataPath + @"\Bundles\UI", true);
        foreach (var item in temp)
        {
            if (!item.EndsWith(".prefab"))
            {
                continue;
            }
            paths.Add(ProcessPath(item));
        }
        return paths;
    }

    private static List<string> UIAllInAssets()
    {
        List<string> paths = new List<string>();

        var temp = GetAllResourcePath(UnityEngine.Application.dataPath + @"\", true);
        foreach (var item in temp)
        {
            if (!item.EndsWith(".prefab"))
            {
                continue;
            }
            paths.Add(ProcessPath(item));
        }
        return paths;
    }



    private static string ProcessPath(string path)
    {
        var newString = path.Replace("\\", "/");
        var index = newString.IndexOf("Asset");
        newString = newString.Remove(0, index);
        return newString;
    }

    private static bool OkType(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        return findTextureType.Contains(textureImporter.textureType);
    }


    [MenuItem("Tools/查找同名的ui图片")]
    public static void FindRepeatLastNameSprite()
    {
        Dictionary<string, string> repeatedMap = new Dictionary<string, string>();
        var sprites1 = UISprites();
        var sprites2 = UISprites();
        foreach (var item1 in sprites1)
        {
            var item1lastIndexOf = item1.LastIndexOf('/');
            var item1Name = item1.Substring(item1lastIndexOf + 1);
            var item1Directory = item1.Remove(item1lastIndexOf);
            if (repeatedMap.ContainsKey(item1Name))
            {
                continue;
            }
            foreach (var item2 in sprites2)
            {
                if (string.Equals(item1, item2))
                {
                    continue;
                }
                var item2lastIndexOf = item2.LastIndexOf('/');
                var item2Name = item2.Substring(item2lastIndexOf + 1);
                var item2Directory = item2.Remove(item2lastIndexOf);
                if (item2Name == item1Name)
                {
                    if (!repeatedMap.ContainsKey(item1Name))
                    {
                        repeatedMap.Add(item1Name, item1Directory);
                    }
                    repeatedMap[item1Name] += ("、" + item2Directory);
                }
            }
        }
        foreach (var item in repeatedMap)
        {
            UnityEngine.Debug.LogError($"同名的ui图片: <color=#9400D3>{item.Key}、路径:{item.Value}</color>");
        }
        UnityEngine.Debug.LogError("完成");
    }
}
