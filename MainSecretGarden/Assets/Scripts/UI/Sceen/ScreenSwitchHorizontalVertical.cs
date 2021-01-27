using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSwitchHorizontalVertical : MonoBehaviour
{
    public const float ScreenLong = 2688f;
    public const float ScreenWide = 1242f;
    private static ScreenSwitchHorizontalVertical instance;
    public static ScreenSwitchHorizontalVertical Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go=new GameObject("ScreenSwitch");
                instance = go.AddComponent<ScreenSwitchHorizontalVertical>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    /// <summary>
    /// 切换屏幕方向
    /// </summary>
    /// <param name="isHorizontal"></param>
    private void SwitchScreenOrientation(bool isHorizontal = true)
    {
        if (isHorizontal)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;//屏幕方向 
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    /// <summary>
    /// 切换横屏
    /// </summary>
    /// <returns></returns>
    public async Task ChangeSwitchHorizontal()
    {
        UIComponent.CreateUI(UIType.UISreenBlack);
        CanvasScaler[] canvasScalers = UIRoot.instance.GetComponentsInChildren<CanvasScaler>();
        SwitchScreenOrientation();
        for (int i = 0; i < canvasScalers.Length; i++)
        {
            canvasScalers[i].referenceResolution = new Vector2(ScreenLong, ScreenWide);
            //todo 设置缩放系数
        }
        await UniTask.Delay(300);
        UIComponent.RemoveUI(UIType.UISreenBlack);
    }
    /// <summary>
    /// 切换为竖屏
    /// </summary>
    /// <returns></returns>
    public async Task ChangeSwitchVertial()
    {
        UIComponent.CreateUI(UIType.UISreenBlack);
        CanvasScaler[] canvasScalers = UIRoot.instance.GetComponentsInChildren<CanvasScaler>();
        SwitchScreenOrientation(false);
        for (int i = 0; i < canvasScalers.Length; i++)
        {
            canvasScalers[i].referenceResolution = new Vector2(ScreenWide, ScreenLong);
            //todo 设置缩放系数
        }
        await UniTask.Delay(300);
        UIComponent.RemoveUI(UIType.UISreenBlack);
    }
}
