using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 分享工具
/// </summary>
public class ShareTool : MonoBehaviour
{
    private static ShareTool _instance;

    /// <summary>
    /// 是否正在加载分享
    /// </summary>
    private bool isProcessing = false;

    /// <summary>
    /// 是否拥有焦点
    /// </summary>
    private bool _isFocus;

    /// <summary>
    /// 分享文本
    /// </summary>
    private string _shareDefaultText = "";

    public static ShareTool Instance
    {
        get
        {
            if (_instance == null)
                ShareTool.Attch(Camera.main.gameObject);
            return _instance;
        }
    }

    /// <summary>
    /// 附加分享脚本
    /// </summary>
    /// <param name="user"></param>
    public static void Attch(GameObject user)
    {
        if(_instance == null)
            _instance = user.AddComponent<ShareTool>();
    }


#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _ShareAction(string text, string encodeedMedia);
#endif
    private static AndroidJavaObject currentActivity
    {
        get
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>(
                "currentActivity");
        }
    }

    private Action ShareEnd;

    /// <summary>
    /// 分享
    /// </summary>
    public void OnShareNative(Texture2D icon, Action shareEnd = null) 
    {
        //测试代码 2020/10/23 后续需要张聪提供支持
        shareEnd?.Invoke();
        return;

        ShareEnd = shareEnd;
#if UNITY_ANDROID
        RequestPermissionTool.Instance.RequestAndroidPermission("android.permission.WRITE_EXTERNAL_STORAGE", (i, s) => { });
#endif
        if (isProcessing)
            return;
        StopAllCoroutines();

        Debug.Log("分享 开始");
        StartCoroutine(TaskShare(icon));   
    }

    private IEnumerator TaskShare(Texture2D shareIcon) 
    {
        isProcessing = true;
        yield return UniversalTool.StaticWaitForEndOfFrame;

#if UNITY_IOS && !UNITY_EDITOR
        Texture2D picture = shareIcon;
        byte[] val = picture.EncodeToPNG();
        string bytesString = System.Convert.ToBase64String(val);
        _ShareAction(shareDefaultText, bytesString);
#endif

#if UNITY_ANDROID 
        if (!Application.isEditor)
        {
            Texture2D picture = shareIcon;
            string Path_save = "";

            var destination = Application.persistentDataPath;
            Path_save = destination + "/img.png";
            var bytes = picture.EncodeToPNG();
            File.WriteAllBytes(Path_save, bytes);
            using (var util = new AndroidJavaClass("com.zcc.unitybase.Util"))
            {
                util.CallStatic("toastAblumImg", currentActivity, destination, "img.png");
            }

            AndroidJavaObject uri =
                new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", Path_save);
            AndroidJavaObject sharingIntent = new AndroidJavaObject("android.content.Intent");
            sharingIntent.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");
            sharingIntent.Call<AndroidJavaObject>("setType", "image/*");
            sharingIntent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.STREAM", uri);
            sharingIntent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", _shareDefaultText);
            string shareText = "Share to";
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                    shareText = "分享到";
                    break;
                case SystemLanguage.ChineseSimplified:
                    shareText = "分享到";
                    break;
                case SystemLanguage.ChineseTraditional:
                    shareText = "分享到";
                    break;
            }

            AndroidJavaObject createChooser =
                sharingIntent.CallStatic<AndroidJavaObject>("createChooser", sharingIntent, shareText);
            currentActivity.Call("startActivity", createChooser);

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitUntil(() => _isFocus);
#endif
        isProcessing = false;

        //分享回调
        ShareEnd?.Invoke();
        Debug.Log("分享 完成！");
    }

    void OnApplicationFocus(bool focusStatus)
    {
        _isFocus = focusStatus;
    }
}
