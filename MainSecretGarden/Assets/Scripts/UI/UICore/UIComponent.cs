using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 管理所有UI
/// </summary>
public class UIComponent
{
    //用来阻塞异步加载
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 100);
    static Dictionary<string, GameObject> DicUI = new Dictionary<string, GameObject>();//每个UI只运行有一个
    static void AddListUI(string UIName, GameObject goUI)
    {
        if (!DicUI.ContainsKey(UIName))
        {
            DicUI.Add(UIName, goUI);
        }
    }
    public static bool IsHaveUI(string UIName)
    {
        return DicUI.ContainsKey(UIName);
    }
    public static GameObject GetExistUI(string UIName)
    {
        return DicUI[UIName];
    }
    static void RemoveFromListUI(string UIName)
    {
        if (DicUI.ContainsKey(UIName))
        {
            DicUI.Remove(UIName);
        }
    }
    public static GameObject CreateUI(string UIName, bool isTopCanvas = false, bool isBGCanvas = false,bool isGuideCanvas=false)
    {
        GameObject go = null;
        if (IsHaveUI(UIName))
        {
            go = GetExistUI(UIName);
            if (!go.activeInHierarchy)
            {
                go.SetActive(true);
            }
        }
        else
        {
            var prefab = ABManager.GetAsset<GameObject>(UIName);
            Transform parent = null;
            if (isTopCanvas)
            {
                parent = UIRoot.instance.GetUIRootCanvasTop().transform;
            }
            else if (isBGCanvas) 
            {
                parent = UIRoot.instance.GetUIRootCanvasBG().transform;
            }
            else if (isGuideCanvas)
            {
                parent = UIRoot.instance.GetUIRootCanvasGuide().transform;
            }
            else
            {
                parent = UIRoot.instance.GetUIRootCanvas().transform;
            }
            go = ResourcesHelper.InstantiatePrefabFromABSetDefault(prefab, parent);
            AddListUI(UIName, go);
        }
        //层级设置到最高
        go.transform.SetAsLastSibling();
        return go;
    }

    public static T GetComponentHaveExist<T>(string UIName) where T:MonoBehaviour
    {
        T com = default(T);
        if (IsHaveUI(UIName))
        {
            var go = DicUI[UIName];
            com = go.GetComponent<T>();
        }
        return com;
    }

    public static GameObject CreateUIFromResource(string UIName, bool isTopCanvas = false, bool isBGCanvas = false)
    {
        GameObject go = null;
        if (IsHaveUI(UIName))
        {
            go = GetExistUI(UIName);
            if (!go.activeInHierarchy)
            {
                go.SetActive(true);
            }
        }
        else
        {
            var prefab = Resources.Load<GameObject>(UIName);
            Transform parent = null;
            if (isTopCanvas)
            {
                parent = UIRoot.instance.GetUIRootCanvasTop().transform;
            }
            else if (isBGCanvas)
            {
                parent = UIRoot.instance.GetUIRootCanvasBG().transform;
            }
            else
            {
                parent = UIRoot.instance.GetUIRootCanvas().transform;
            }
            go = ResourcesHelper.InstantiatePrefabFromABSetDefault(prefab, parent);
            AddListUI(UIName, go);
        }
        //层级设置到最高
        go.transform.SetAsLastSibling();
        return go;
    }
    public static void RemoveUI(string UIName)
    {
        RemoveFromListUI(UIName);
        UIName = $"{UIName}(Clone)";
        var uiParentRoot1 = UIRoot.instance.GetUIRootCanvas().transform;
        Transform uiTrans1 = uiParentRoot1.Find(UIName);
        if (uiTrans1 != null)
        {
            GameObject.Destroy(uiTrans1.gameObject);
        }
        
        var uiParentRoot2 = UIRoot.instance.GetUIRootCanvasTop().transform;
        Transform uiTrans2 = uiParentRoot2.Find(UIName);
        if (uiTrans2 != null)
        {
            GameObject.Destroy(uiTrans2.gameObject);
        }

        var uiParentRoot3 = UIRoot.instance.GetUIRootCanvasBG().transform;
        Transform uiTrans3 = uiParentRoot3.Find(UIName);
        if (uiTrans3 != null)
        {
            GameObject.Destroy(uiTrans3.gameObject);
        }
        var uiParentRoot4 = UIRoot.instance.GetUIRootCanvasGuide().transform;
        Transform uiTrans4 = uiParentRoot4.Find(UIName);
        if (uiTrans4 != null)
        {
            GameObject.Destroy(uiTrans4.gameObject);
        }
        Resources.UnloadUnusedAssets();

        //
        StaticData.NotifyUIHideOrRemove(UIName);
    }

    /// <summary>
    /// 移除其它界面到登录界面
    /// </summary>
    public static void RemoveUIEnterLogin() 
    {
        var uiParentRoot1 = UIRoot.instance.GetUIRootCanvas().transform;

        List<string> waitRemoveUI = new List<string>();
            
        foreach (var item in DicUI)
        {
            //不移出加载界面
            if (item.Key == UIType.UISceneLoading)
                continue;
            waitRemoveUI.Add(item.Key);
        }
        //移除通过UIComponent生成的界面
        for (int i = 0; i < waitRemoveUI.Count; i++)
        {
            RemoveUI(waitRemoveUI[i]);
            DicUI.Remove(waitRemoveUI[i]);
        }

        string sceneLoadingName = string.Format("{0}(Clone)", UIType.UISceneLoading);
        int count = uiParentRoot1.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            if (uiParentRoot1.GetChild(i).name != "BgInit" && uiParentRoot1.GetChild(i).name != sceneLoadingName)
            {
                uiParentRoot1.GetChild(i).gameObject.SetActive(false);
                GameObject.Destroy(uiParentRoot1.GetChild(i).gameObject);
            }
        }


        Resources.UnloadUnusedAssets();
    }

    public static void HideUI(string UIName)
    {
        //
        StaticData.NotifyUIHideOrRemove(UIName);

        UIName = $"{UIName}(Clone)";
        var uiParentRoot1 = UIRoot.instance.GetUIRootCanvas().transform;
        Transform uiTrans1 = uiParentRoot1.Find(UIName);
        if (uiTrans1 != null)
        {
            uiTrans1.gameObject.SetActive(false);
            return;
        }
        var uiParentRoot2 = UIRoot.instance.GetUIRootCanvasTop().transform;
        Transform uiTrans2 = uiParentRoot2.Find(UIName);
        if (uiTrans2 != null)
        {
            uiTrans2.gameObject.SetActive(false);
            return;
        }
        var uiParentRoot3 = UIRoot.instance.GetUIRootCanvasBG().transform;
        Transform uiTrans3 = uiParentRoot3.Find(UIName);
        if (uiTrans3 != null)
        {
            uiTrans3.gameObject.SetActive(false);
            return;
        }
        Resources.UnloadUnusedAssets();
    }


    public static async Task<GameObject> CreateUIAsync(string UIName, bool isTopCanvas = false, bool isBGCanvas = false, bool isGuideCanvas = false)
    {
        GameObject go = null;
        if (IsHaveUI(UIName))
        {
            go = GetExistUI(UIName);
            if (!go.activeInHierarchy)
            {
                go.SetActive(true);
            }
            //层级设置到最高
            go.transform.SetAsLastSibling();
            return go;
        }
        else
        {
            bool isRelease = false;
            try
            {
                //打开遮罩，阻挡点击事件
                GameObject uiEventMask = CreateUI(UIType.UIEventMask, true);
                await semaphoreSlim.WaitAsync();
                //信号量释放后要重新刷新堵塞前的状态
                var prefab = await ABManager.GetAssetAsync<GameObject>(UIName);
                Transform parent = null;
                if (isTopCanvas)
                {
                    parent = UIRoot.instance.GetUIRootCanvasTop().transform;
                }
                else if (isBGCanvas)
                {
                    parent = UIRoot.instance.GetUIRootCanvasBG().transform;
                }
                else if (isGuideCanvas)
                {
                    parent = UIRoot.instance.GetUIRootCanvasGuide().transform;
                }
                else 
                {
                    parent = UIRoot.instance.GetUIRootCanvas().transform;
                }
                go = ResourcesHelper.InstantiatePrefabFromABSetDefault(prefab, parent);
                //层级设置到最高
                go.transform.SetAsLastSibling();
                AddListUI(UIName, go);
                ReleaseRelated(ref isRelease);
                return go;
            }
            catch (Exception e)
            {
                throw new Exception($"{UIName} UI 错误: {e}");
            }
            finally
            {
                ReleaseRelated(ref isRelease);
            }
        }
    }

    static void ReleaseRelated(ref bool isRelease)
    {
        if (!isRelease)
        {
            semaphoreSlim.Release();
            //关闭遮罩，可以继续点击
            HideUI(UIType.UIEventMask);
            isRelease = true;
        }
    }

    public static void ClearAllUI()
    {
        for (int i = 0; i < DicUI.Count; i++)
        {
            RemoveUI(DicUI.Keys.ToArray()[i]);
        }
        DicUI.Clear();
    }
}