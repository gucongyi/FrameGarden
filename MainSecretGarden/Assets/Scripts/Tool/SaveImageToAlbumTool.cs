using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;
using BestHTTP.Extensions;

/// <summary>
/// 保存图片到相册
/// </summary>
public class SaveImageToAlbumTool : MonoBehaviour
{
    private static SaveImageToAlbumTool _instance;

    public static SaveImageToAlbumTool Instance 
    {
        get { 
            if (_instance == null)
                SaveImageToAlbumTool.Attch(Camera.main.gameObject);
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
            _instance = user.AddComponent<SaveImageToAlbumTool>();
    }


#if UNITY_IPHONE //与调用ios 里面的保存相册接口
    [DllImport("__Internal")]
    private static extern void _SavePhoto(string readAddr);
#endif


    /// <summary>
    /// 下载完成 回调
    /// </summary>
    public static Action DownloadCompleted;


    /// <summary>
    /// 下载图片到相册
    /// </summary>
    /// <param name="t"></param>
    public void SaveIconToPlayAibum(Texture2D icon, Action downloadCom = null)
    {
        //测试代码 2020/10/23 后续需要张聪提供支持
        downloadCom?.Invoke();
        return;

        DownloadCompleted = downloadCom;
        
        StartCoroutine(DownLoadTexture2d(icon));
    }

    private static int getAndroidSDKINT()
    {
        using (var os = new AndroidJavaClass("com.zcc.unitybase.Util"))
        {
            return os.CallStatic<int>("getAndroidSDKINT");
        }
    }

    private static void toastAblumImg(string filePath, string fileName)
    {
        using (var os = new AndroidJavaClass("com.zcc.unitybase.Util"))
        {
            var _Context =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            os.CallStatic("toastAblumImg", _Context, filePath, fileName);
        }
    }

    //传的参数是自己获取的图片
    private IEnumerator DownLoadTexture2d(Texture2D texture)
    {
        //截图操作  
        yield return UniversalTool.StaticWaitForEndOfFrame;

        //截图保存的图片
        byte[] bytes = texture.EncodeToPNG();

        //获取系统时间  
        System.DateTime now = new System.DateTime();
        now = System.DateTime.Now;
        string filename = string.Format("image{0}{1}{2}{3}.png", now.Day, now.Hour, now.Minute, now.Second);
        filename = "/PhotoAlbum" + filename;
        //
        string Path_save = UniversalTool.GetSaveFilePath(filename);
        string destination = Path_save.Replace(filename, "/PhotoAlbum");
        //
        filename = filename.Replace("/PhotoAlbum", "");

        //本地存储
        File.WriteAllBytes(Path_save, bytes);

        //应用平台判断，路径选择  
        if (Application.platform == RuntimePlatform.Android)
        {
            //destination = "/mnt/sdcard/DCIM/Screenshots";
#if UNITY_ANDROID
            Debug.Log("将文件图片写入地址！");
            toastAblumImg(destination, filename);

            // 安卓在这里需要去 调用原生的接口去 刷新一下，不然相册显示不出来
            using (AndroidJavaClass playerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = playerActivity.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    Debug.Log("scanFile:m_androidJavaObject ");
                    jo.Call("scanFile", Path_save);
                }
            }
#endif
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
            _SavePhoto(Path_save);
#endif
        }
        //下载完成
        DownloadCompleted?.Invoke();
    }
}