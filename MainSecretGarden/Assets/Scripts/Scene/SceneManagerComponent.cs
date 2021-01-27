using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public enum EnumScene
{
    /// <summary>
    /// 空场景 默认大厅
    /// </summary>
    Empty,
    /// <summary>
    /// 
    /// </summary>
    TestPlant,
    /// <summary>
    /// 庄园
    /// </summary>
    Manor,
    /// <summary>
    /// 大富翁
    /// </summary>
    Zillionaire,
    /// <summary>
    /// 晚会
    /// </summary>
    Party
}

public class SceneManagerComponent : MonoBehaviour
{
    const int radioBundle = 10;
    public static SceneManagerComponent _instance;
    AsyncOperation loadSceneAsyncOperation;
    AsyncOperation unloadSceneAsyncOperation;
    TaskCompletionSource<bool> tcsLoad;
    TaskCompletionSource<bool> tcsUnload;
    TaskCompletionSource<bool> tcsFrame;
    bool isFrameFinish;
    public float deltaTime;
    public int lastProgress = 0;
    private bool isLoadSceneTaskFinish;
    private bool isUnLoadSceneTaskFinish;
    float lerpInterval = 0;
    /// <summary>
    /// 当前处于场景
    /// </summary>
    public EnumScene _currSceneTage = EnumScene.Empty;
    UISceneLoadingComponent uISceneLoadingComponent = null;
    public List<string> listAddScene = new List<string>();
    Dictionary<EnumScene, List<string>> preLoadMap = new Dictionary<EnumScene, List<string>>()
        {
            { EnumScene.TestPlant, new List<string>(){ "female_horse.unity3d", "male_horse.unity3d", "canvascamerafade.unity3d", "matchfield.unity3d" } },
            //大富翁预加载 角色 地图 骰子 主页
            { EnumScene.Zillionaire, new List<string>(){ "assets/bundles/role/zillionaire/qvnewnv/prefab.unity3d",
                "assets/bundles/zillionaire2d/prefabs/map/map0/prefab.unity3d", 
                "assets/bundles/zillionaire2d/prefabs/map/dice/prefab.unity3d", 
                "assets/bundles/zillionaire2d/prefabs/ui/uihomepagebg/prefab.unity3d",
                "assets/bundles/zillionaire2d/prefabs/ui/uihomepage/prefab.unity3d" } },
        };

    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        Reset();
    }
    private void Start()
    {
        GameObject go = UIComponent.CreateUI(UIType.UISceneLoading,isTopCanvas:true);
        uISceneLoadingComponent = go.GetComponent<UISceneLoadingComponent>();
        UIComponent.HideUI(UIType.UISceneLoading);
    }

    private void Reset()
    {
        listAddScene.Clear();
        loadSceneAsyncOperation = null;
        unloadSceneAsyncOperation = null;
        isFrameFinish = false;
        tcsLoad = null;
        tcsFrame = null;
        tcsUnload = null;
        isLoadSceneTaskFinish = false;
        isUnLoadSceneTaskFinish = false;
        lerpInterval = 0;
    }

    public void Update()
    {
        if (loadSceneAsyncOperation != null)
        {
            if (!isLoadSceneTaskFinish && loadSceneAsyncOperation.isDone && isFrameFinish)
            {
                isLoadSceneTaskFinish = true;
                tcsLoad.SetResult(true);
            }
            else if (!loadSceneAsyncOperation.isDone)
            {
                if (loadSceneAsyncOperation.progress < 0.9f)
                {
                    uISceneLoadingComponent?.OnRealProgress(radioBundle + (100 - radioBundle) * loadSceneAsyncOperation.progress);
                }
                else
                {
                    lerpInterval += 0.05f;
                    uISceneLoadingComponent?.OnRealProgress(radioBundle + (100 - radioBundle) * Mathf.Lerp(0.9f, 1, lerpInterval));
                }
            }
            else
            {
                if (!isLoadSceneTaskFinish)
                {
                    uISceneLoadingComponent?.OnRealProgress(radioBundle + (100 - radioBundle) * loadSceneAsyncOperation.progress);
                }
            }
        }


        if (unloadSceneAsyncOperation != null && !isUnLoadSceneTaskFinish && unloadSceneAsyncOperation.isDone)
        {
            tcsUnload.SetResult(true);
            isUnLoadSceneTaskFinish = true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneType"></param>
    /// <param name="isCloseLoadingAfter">为true需要自己关闭进度条</param>
    /// <returns></returns>
    public async Task ChangeSceneAsync(EnumScene sceneType,bool isCloseLoadingAfter=false)
    {
        //加载进度条
        await UIComponent.CreateUIAsync(UIType.UISceneLoading,isTopCanvas:true);
        uISceneLoadingComponent.Reset();

        //关闭聊天相关界面
        ChatTool.CloseChat();
        var scencePath = EnumScene.Empty.ToString();
        _currSceneTage = sceneType;
        //加载空场景
        scencePath = "Scenes/" + scencePath;
        SceneManager.LoadScene(scencePath);
        var bundleType = sceneType.ToString();
        if (sceneType != EnumScene.Empty)//空场景为退出到大厅
        {
            List<string> bundleNameList = new List<string>();

            if (preLoadMap.ContainsKey(sceneType))
                bundleNameList.AddRange(preLoadMap[sceneType]);
            //加上自己的场景ab包
            var sceneBundleName = ABManager.GetBundleNameByAssetNameAndType(sceneType.ToString(), typeof(GameObject));
            bundleNameList.Add($"{sceneBundleName}");

            List<string> allBundleNameList = new List<string>();
            foreach (var bundleName in bundleNameList)
            {
                string[] dependencies = ResourcesHelper.GetSortedDependencies(bundleName.ToLower());
                if (dependencies != null)
                {
                    foreach (var item in dependencies)
                    {
                        if (allBundleNameList.Contains(item))
                            continue;
                        allBundleNameList.Add(item);
                    }
                }
            }
            ResourcesComponent resourcesComponent = ResourcesComponent.instance;
            for (int i = 0; i < allBundleNameList.Count; i++)
            {
                await resourcesComponent.LoadOneBundleAsync(allBundleNameList[i], (real) => UpdateProgress(i, allBundleNameList.Count, real, allBundleNameList[i]));
            }
        }


        await AddSceneAsyncTask(bundleType, LoadSceneMode.Single);
        //await StaticData.CreateChatMini();
        if (isCloseLoadingAfter == false)
        {
            UIComponent.HideUI(UIType.UISceneLoading);
        }
    }

    void UpdateProgress(int index, int count, float progressReal, string bundleName)
    {
        float state = (float)radioBundle / count;
        float progressInit = state * index;
        float progressFake = progressInit + progressReal * state;
        uISceneLoadingComponent?.OnRealProgress(progressFake);
    }
    public void AddSceneList(string sceneString)
    {
        if (!listAddScene.Contains(sceneString))
        {
            listAddScene.Add(sceneString);
        }
    }
    public void RemoveFromSceneList(string sceneString)
    {
        if (listAddScene.Contains(sceneString))
        {
            listAddScene.Remove(sceneString);
        }
    }

    public Task<bool> AddSceneAsyncTask(string sceneName, LoadSceneMode loadMode)
    {
        Reset();
        //GameSoundPlayer.Instance.PlaySoundEffect();
        //SoundTools.PlaySoundEffect(EffectSoundNames.Transition);
        DelayFrame();//启动帧事件
        this.tcsLoad = new TaskCompletionSource<bool>();
        // 加载map
        this.loadSceneAsyncOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
        return this.tcsLoad.Task;
    }


    public Task<bool> DelayFrame()
    {
        isFrameFinish = false;
        tcsFrame = new TaskCompletionSource<bool>();
        UpdateFrames();
        return tcsFrame.Task;
    }

    public async void UpdateFrames()
    {
        if (isFrameFinish) return;
        int Frames = radioBundle;
        while (Frames < 100)//100帧,2s
        {
            await UniTask.Delay(20);//每帧20ms
            Frames++;
            uISceneLoadingComponent?.OnFrameProgress(Frames);
        }
        isFrameFinish = true;
        this.tcsFrame.TrySetResult(true);
        //this.tcsFrame.SetResult(true);
    }



    public void Dispose()
    {
        Resources.UnloadUnusedAssets();
    }
}