using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 权限请求工具
/// 1.android 权限请求
/// 2.其它默认返回 true
/// </summary>
public class RequestPermissionTool : MonoBehaviour
{
    private static RequestPermissionTool _instance;

    public static RequestPermissionTool Instance
    {
        get
        {
            if (_instance == null)
                RequestPermissionTool.Attch(Camera.main.gameObject);
            return _instance;
        }
    }

    /// <summary>
    /// 附加分享脚本
    /// </summary>
    /// <param name="user"></param>
    public static void Attch(GameObject user)
    {
        if (_instance == null)
            _instance = user.AddComponent<RequestPermissionTool>();
    }

#if UNITY_ANDROID

    private AndroidJavaObject activity;
    private Action<int, string> PermissionResult;

    public bool RequestAndroidPermission(string permission, Action<int, string> callback)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        PermissionResult = callback;
        if (activity != null) return activity.Call<bool>("RequestPermission", gameObject.name, permission);
        return false;
    }

    public void RequestPermissionCallback(string msg)
    {
        CBResult result = JsonUtility.FromJson<CBResult>(msg);
        if (result.code == 1)
        {
            PermissionResult(1, result.msg);
        }
        else if (result.code == 2)
        {
            PermissionResult(2, result.msg);
        }
    }
#else
        public static void Init(){}
        public static bool RequestPermission(string permission, Action<int, string> callback){return true;}
#endif
}

[System.Serializable]
public class CBResult
{
    public int code;
    public string msg;
}

