using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

public static class BundleHelper
{
    public static async Task DownloadBundle()
    {
        await StartDownLoadResources();
    }
    public static async Task<bool> IsGameVersionCodeEqual()
    {
        UnityWebRequestAsync webRequestAsync = MonoBehaviourHelper.CreateTempComponent<UnityWebRequestAsync>();
        try
        {
            GlobalProto.VersionCodeInfo NowLineCodeInfo = null;
            GlobalProto.VersionCodeInfo UpdatingCodeInfo = null;
            string PathNowLineCodeInfo = GlobalConfigComponent.intance.GlobalProto.GetNowLineCodeInfoPath();
            //下载PathNowLineCodeInfo
            await webRequestAsync.DownloadAsync(PathNowLineCodeInfo);
            NowLineCodeInfo = GlobalConfigComponent.intance.GlobalProto.GetNowLineCodeInfo(webRequestAsync.Request.downloadHandler.text);
            string PathUpdatingCodeInfo = GlobalConfigComponent.intance.GlobalProto.GetUpdatingCodeInfoPath();
            //下载PathUpdatingCodeInfo
            await webRequestAsync.DownloadAsync(PathUpdatingCodeInfo);
            UpdatingCodeInfo = GlobalConfigComponent.intance.GlobalProto.GetUpdatingCodeInfo(webRequestAsync.Request.downloadHandler.text);
            webRequestAsync.Dispose();
            //比较VersionCode
            StaticData.DebugGreen($"local versionGameCode:{StaticData.localVersionGameCode},remote NowLineCodeInfo GameVersionCode:{NowLineCodeInfo.GameVersionCode} remote NowLineCodeInfo ResVersionCode:{NowLineCodeInfo.ResVersionCode},remote UpdatingCodeInfo GameVersionCode:{UpdatingCodeInfo.GameVersionCode} remote UpdatingCodeInfo ResVersionCode:{UpdatingCodeInfo.ResVersionCode}");
            if (StaticData.localVersionGameCode < NowLineCodeInfo.GameVersionCode)
            {
                //强更
                TipsDifferentVersion();
                StaticData.intParentResABDirectory = NowLineCodeInfo.ResVersionCode;
                StaticData.isUsePlatformUpdatingGateWay = false;
                return false;
            }
            else if (StaticData.localVersionGameCode == NowLineCodeInfo.GameVersionCode)
            {
                //热更本地资源
                StaticData.intParentResABDirectory = NowLineCodeInfo.ResVersionCode;
                StaticData.isUsePlatformUpdatingGateWay = false;
                return true;
            }
            else
            {
                //已经是最新包，只需要更新最新包资源即可
                StaticData.intParentResABDirectory = UpdatingCodeInfo.ResVersionCode;
                StaticData.isUsePlatformUpdatingGateWay = true;
                return true;
            }
            
        }
        catch (Exception e)
        {
            if (e.Message.Contains("request error"))
            {
                Debug.Log($"load VersionGameCode error:'{e.Message}'");
                return true;
            }
        }
        finally
        {
            GameObject.Destroy(webRequestAsync.gameObject);
        }
        return false;
    }

    /// <summary>
    /// 提示版本号不一样
    /// </summary>
    private static void TipsDifferentVersion()
    {
        ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UITipUpdateVersion", UIRoot.instance.GetUIRootCanvas().transform);
    }
    

    public static async Task StartDownLoadResources()
    {
        if (StaticData.isUseAssetBundle)
        {
            BundleDownloaderComponent bundleDownloaderComponent = null;
            try
            {

                bundleDownloaderComponent = MonoBehaviourHelper.CreateTempComponent<BundleDownloaderComponent>();
                var t = bundleDownloaderComponent.LoadInfo();
                var needDown = await t;
                StaticData.isNotWifiDownload = true;
                if (needDown)
                {
                    GameObject goUpdateProcess = ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UISceneLoading", UIRoot.instance.GetUIRootCanvas().transform);
                    UITipABUpdateProgressComponent uiTipABUpdateProgressComponent = goUpdateProcess.GetComponent<UITipABUpdateProgressComponent>();
                    uiTipABUpdateProgressComponent.DownLoadInfo = bundleDownloaderComponent.DownloadInfo;
                    var x1 = bundleDownloaderComponent.DownloadInfo.TotalSize / 1024;
                    var x = x1 / 1024f;
                    StaticData.isNotWifiDownloadClick = false;
                    //如果大于1m 不是wifi才弹提示
                    if (x > 1 && Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                    {
                        GameObject goWifiConfirm = ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UITipABUpdateNotWifi", UIRoot.instance.GetUIRootCanvas().transform);
                        UITipABUpdateNotWifiComponent uiTipABUpdateNotWifiComponent = goWifiConfirm.GetComponent<UITipABUpdateNotWifiComponent>();
                        //取两位小数
                        int j = (int)(x * 100);
                        x = j / 100f;
                        uiTipABUpdateNotWifiComponent.TextShow.text = $"当前不是wifi环境, 更新需要消耗{x}M流量,\n是否更新 ? (点击取消将退出游戏)";
                        await UniTask.WaitUntil(() => StaticData.isNotWifiDownloadClick == true);
                    }


                    if (!StaticData.isNotWifiDownload)
                    {
                        StaticData.QuitApplication();
                        //永远不返回
                        await UniTask.WaitUntil(() => StaticData.isNotWifiDownload == true);
                    }
                    await bundleDownloaderComponent.Down();
                    uiTipABUpdateProgressComponent.DownLoadInfo.IsEnd = true;
                    GameObject.DestroyImmediate(goUpdateProcess);

                }
                

                await ResourcesComponent.instance.LoadOneBundleAsync("StreamingAssets");

                ResourcesComponent.AssetBundleManifestObject = (AssetBundleManifest)ResourcesComponent.instance.GetAsset("StreamingAssets", "AssetBundleManifest");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                GameObject.Destroy(bundleDownloaderComponent.gameObject);
            }

        }
    }
}
