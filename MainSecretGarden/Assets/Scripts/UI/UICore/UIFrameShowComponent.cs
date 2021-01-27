using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class UIFrameShowComponent : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern long _IOS_GetTaskUsedMemeory();
#endif
    public Button ButtonOpen;
    bool isClose;
    public GameObject goPerformance;
    public Text TextShowFrame;
    public Text TextShowMemory;
    public Button ButtonShowFps;

    private Transform trans;
    int TimesEachSeconds;
    float currTime;
    // Start is called before the first frame update
    void Start()
    {
        TimesEachSeconds = 0;
        currTime = 0f;
        TextShowFrame.text = Application.targetFrameRate + " FPS";//初始值
        TextShowMemory.text = "已使用内存：";
        isClose = true;
        goPerformance.SetActive(true);
        ButtonOpen.onClick.RemoveAllListeners();
        ButtonOpen.onClick.AddListener(OnButtonOpenClick);
        ButtonShowFps.onClick.RemoveAllListeners();
        ButtonShowFps.onClick.AddListener(OnButtonShowFpsClick);
    }

    private void OnButtonShowFpsClick()
    {
        int m = GetUseMemory();
        TextShowMemory.text = $"已占用内存：{m}M";
    }

    private void OnButtonOpenClick()
    {
        if (isClose)
        {
            goPerformance.SetActive(false);
        }
        else
        {
            goPerformance.SetActive(true);
        }
        isClose = !isClose;
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.unscaledDeltaTime;
        TimesEachSeconds++;
        if (currTime >= 1f)
        {
            TextShowFrame.text = TimesEachSeconds + " FPS " + Mathf.Round(10000f / TimesEachSeconds) / 10 + "ms";
            TimesEachSeconds = 0;
            currTime = 0f;
        }
    }
    public static int GetUseMemory()
    {
        int memory = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject unityPluginLoader = new AndroidJavaObject("com.ccgame.unitytool.UnityToast");
            float tempMemory = unityPluginLoader.CallStatic<float>("GetMemory", currentActivity);
            memory = (int)tempMemory;
        }
        catch (System.Exception e)
        {
        }
#elif UNITY_IOS && !UNITY_EDITOR
        memory = (int)_IOS_GetTaskUsedMemeory();
#endif
        return memory;
    }
}
