using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Ram_Display : MonoBehaviour {

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern long _IOS_GetTaskUsedMemeory();
#endif

    ////总内存
    //public Text TotalReserved_M;
    ////已占用内存
    //public Text TotalAllocated_M;
    ////除去Mono堆内存已占用内存
    //public Text TotalnoMonoAllocated_M;
    ////空闲中内存
    //public Text TotalUnusedReserved_M;
    ////总Mono堆内存
    //public Text MonoHeap_S;
    ////已占用Mono堆内存
    //public Text MonoUsed_S;

    private System.DateTime firetime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

    private GUIStyle bb;

    [Range(0.1f, 60)]
    public float UpdateTime = 1;
    [Range(0, 150)]
    public int fontsize = 40;

    public Color fontcolor = new Color(0, 0, 0);
    void Awake()
    {

        bb = new GUIStyle();
        //这是设置背景填充的  
        bb.normal.background = null;
        //设置字体颜色的    
        bb.normal.textColor = fontcolor;
        //当然，这是字体颜色   
        bb.fontSize = fontsize;        
    }

    // Update is called once per frame  
    void Update()
    {
    }
    float TotalReservedMemory = 0;
    float TotalAllocatedMemory = 0;
    float TotalnoMonoAllocatedMemory = 0;
    float TotalUnusedReservedMemory = 0;
    float MonoHeapSize = 0;
    float MonoUsedSize = 0;
    private void OnGUI()
    {

        //System.TimeSpan dic = System.DateTime.UtcNow - firetime;
        //if (dic.TotalSeconds > UpdateTime)
        //{
        //    firetime = System.DateTime.UtcNow;
        //    TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / (float)1000000;
        //    TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (float)1000000;
        //    TotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / (float)1000000;
        //    MonoHeapSize = Profiler.GetMonoHeapSizeLong() / (float)1000000;
        //    MonoUsedSize = Profiler.GetMonoUsedSizeLong() / (float)1000000;
        //    TotalnoMonoAllocatedMemory = TotalAllocatedMemory - MonoUsedSize;
        //    bb.normal.textColor = fontcolor;
        //    bb.fontSize = fontsize;
        //}
        //GUI.Label(new Rect(0, 0, 100, 40), "总内存：" + TotalReservedMemory + "MB", bb);
        if(GUI.Button(new Rect(0, fontsize, 200, 50), "刷新内存")){
            TotalReservedMemory = GetUseMemory();
        }
        GUI.Label(new Rect(0, fontsize, 100, 40), "已占用内存：" + TotalReservedMemory + "MB", bb);
     //   GUI.Label(new Rect(0, fontsize * 2, 100, 40), "除去Mono堆内存已占用内存：" + TotalnoMonoAllocatedMemory + "MB", bb);
        //GUI.Label(new Rect(0, fontsize * 2, 100, 40), "空闲中内存：" + TotalUnusedReservedMemory + "MB", bb);
     //   GUI.Label(new Rect(0, fontsize * 4, 100, 40), "总Mono堆内存：" + MonoHeapSize + "MB", bb);
     //  GUI.Label(new Rect(0, fontsize * 5, 100, 40), "已占用Mono堆内存：" + MonoUsedSize + "MB", bb);
    }


    public static int GetUseMemory()
    {
        int memory = -1;
#if UNITY_ANDROID
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
#elif UNITY_IOS&& !UNITY_EDITOR
        memory = (int)_IOS_GetTaskUsedMemeory();
#endif
        return memory;
    }

}
