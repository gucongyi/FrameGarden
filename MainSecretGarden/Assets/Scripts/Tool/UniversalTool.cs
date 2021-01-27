using DG.Tweening;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用工具 ws 2020/10/22
/// 1. 文件 地址 存储 加载 操作
/// 2. 字符串操作
/// 3. 图片 与 base64 转换
/// 4. 截屏 及保存图片
/// 5. 杂项
/// </summary>
public static class UniversalTool
{
    #region 变量

    /// <summary>
    /// 等待帧结束
    /// </summary>
    public static WaitForEndOfFrame StaticWaitForEndOfFrame = new WaitForEndOfFrame();

    #endregion

    #region 文件 地址 存储 加载 操作

    /// <summary>
    /// 加载json文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string LoadJson(string path)
    {
        //文件信息操作类
        FileInfo info = new FileInfo(path);
        //判断路径是否存在
        if (!info.Exists)
        {
            return null;
        }

        //流读取器
        StreamReader sr = info.OpenText();

        //读取文本内容，直到结束
        string json = sr.ReadToEnd();
        sr.Close();
        return json;
    }

    /// <summary>
    /// 保存json文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="json"></param>
    public static void SaveJson(string path, string json)
    {
        //流写入器
        StreamWriter sw;
        //文件信息操作类
        FileInfo info = new FileInfo(path);
        //判断路径是否存在
        if (!info.Exists)
        {
            sw = info.CreateText();
        }
        else
        {
            //先删除再创建
            info.Delete();
            sw = info.CreateText();
        }

        sw.WriteLine(json);
        sw.Close();
    }

    /// <summary>
    /// 获取保存文件路径
    /// </summary>
    /// <param name="fileName"> 例：fileName.json</param>
    /// <param name="isSave"></param>
    /// <returns></returns>
    public static string GetSaveFilePath(string fileName, bool isSave = true)
    {
        string basePath = Application.persistentDataPath + "/" + StaticData.Uid;

        string filename = basePath + "/" + fileName;
        Debug.Log("获取保存文件路径 filename:" + filename);
        if (!isSave)
        {
            //判断文件是否存在
            if (!System.IO.File.Exists(filename))
            {
                filename = null;
            }
        }
        else
        {
            var flie = basePath;
            if (!Directory.Exists(flie))//文件夹是否存在
            {
                Directory.CreateDirectory(flie);
                Debug.Log("创建路径：" + flie);
            }
        }

        return filename;
    }

    #endregion

    #region 字符串操作

    /// <summary>
    /// 将字符串转通过分隔符 换成int集合
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <param name="split">分隔符</param>
    /// <returns></returns>
    public static List<int> StringSplit(string str, char split)
    {
        string[] idStrs = str.Split(split);
        List<int> ints = new List<int>();
        for (int i = 0; i < idStrs.Length; i++)
        {
            int index = 0;
            if (int.TryParse(idStrs[i], out index))
            {
                ints.Add(index);
            }
            else
            {
                Debug.Log("字符转换失败：" + idStrs[i]);
            }
        }
        return ints;
    }
    #endregion

    #region 图片 与 base64 转换

    /// <summary>
    /// 图片转换成base64编码文本
    /// </summary>
    public static void ImgToBase64String(string path, out string recordBase64String)
    {
        recordBase64String = null;
        try
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            //Image kl;
            //kl.sprite.texture.EncodeToPNG();
            string base64String = Convert.ToBase64String(buffer);
            Debug.Log("获取当前图片base64为---" + base64String);
            recordBase64String = base64String;
        }
        catch (Exception e)
        {
            Debug.Log("ImgToBase64String 转换失败:" + e.Message);
        }
    }

    /// <summary>
    /// 图片转换成base64编码文本 需要传入的图片属性上是 可读写的
    /// </summary>
    /// <param name="image"></param>
    /// <param name="recordBase64String"></param>
    public static void ImgToBase64String(Sprite image, out string base64String)
    {
        base64String = null;
        try
        {
            byte[] buffer = image.texture.EncodeToPNG();
            base64String = Convert.ToBase64String(buffer);
            Debug.Log("获取当前图片base64为---" + base64String);
        }
        catch (Exception e)
        {
            Debug.Log("ImgToBase64String 转换失败:" + e.Message);
        }
    }

    /// <summary>
    /// base64编码文本转换成图片
    /// </summary>
    public static void Base64ToImg(string recordBase64String, ref Image imgComponent)
    {
        byte[] bytes = Convert.FromBase64String(recordBase64String);
        Texture2D tex2D = new Texture2D(100, 100);
        tex2D.LoadImage(bytes);
        Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        imgComponent.sprite = s;
        Resources.UnloadUnusedAssets();
    }


    #endregion

    #region 截屏 及保存图片

    /// <summary>
    /// 截取屏幕全屏 并直接保存
    /// </summary>
    /// <param name="path">路径 例：Application.streamingAssetsPath + "/ScreenShot.png"</param>
    /// <returns></returns>
    public static void CaptureScreenAndSave(string path = null)
    {
        ScreenCapture.CaptureScreenshot(path);
    }

    /// <summary>
    /// 截取屏幕全屏 要等摄像机渲染完，再从帧上截图 
    /// 执行函数 1. OnPostRender() 内  2.协程 yield return new WaitForEndOfFrame(); 后
    /// </summary>
    /// <param name="path">路径 例：Application.streamingAssetsPath + "/ScreenShot.png"</param>
    /// <returns></returns>
    public static Texture2D CaptureScreen(string path = null)
    {

        Texture2D screenShot = null;
        screenShot = ScreenCapture.CaptureScreenshotAsTexture();
        if (!string.IsNullOrEmpty(path))
            SaveTexture2DToFile(screenShot, path);
        return screenShot;
    }

    /// <summary>
    /// 自定义截图的大小（包括UI）
    /// rect = new Rect(new Vector2(leftX, leftY), new Vector2(s_Width, s_Height)); (0,0)点，左下角
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Texture2D CaptureScreen(Rect rect, string path = null)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0); //要等摄像机渲染完，再从帧上截图 
        screenShot.Apply();
        if (!string.IsNullOrEmpty(path))
            SaveTexture2DToFile(screenShot, path);
        return screenShot;
    }

    /// <summary>
    /// 针对指定的相机进行截屏
    /// </summary>
    /// <param name="came"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static Texture2D CaptureScreen(Camera came, Rect r, string path = null)
    {
        RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 0);

        came.targetTexture = rt;
        came.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false);

        screenShot.ReadPixels(r, 0, 0);
        screenShot.Apply();

        came.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        if (!string.IsNullOrEmpty(path))
            SaveTexture2DToFile(screenShot, path);

        return screenShot;
    }


    /// <summary>
    /// 保存图片到文件路径
    /// </summary> 
    /// <param name="texture2D"></param>
    /// <param name="path"> 路径 例：Application.streamingAssetsPath + "/ScreenShot.png"</param>
    public static void SaveTexture2DToFile(Texture2D texture2D, string path)
    {
        byte[] bytes = texture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }

    #endregion

    #region 杂项

    /// <summary>
    /// 退出游戏
    /// </summary>
    public static void OnQuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filePath"></param>
    public static void FileDelete(string filePath)
    {
        //判断是否存在
        bool isExist = File.Exists(filePath);
        if (isExist)
        {
            File.Delete(filePath);
            Debug.Log("删除文件");
        }
    }

    /// <summary>
    /// 获取屏幕适配比例因子
    /// </summary>
    /// <param name="scaleFactor"></param>
    public static void GetAdaptGcreenScaleFactor(ref float scaleFactor)
    {
        scaleFactor = 0.0f;
        //当前屏幕的比率
        float f = (float)Screen.height / (float)Screen.width;
        if (f < 1.775) //1.77866667 //iphone 6 比率
        {
            //float standard = 1242f / 2688f;
        }
    }

    #endregion

    #region 弹窗动画 Pop-up animation 从无到大，再从大缩小到正常尺寸类似于0%—120%—100%

    /// <summary>
    /// 准备弹窗动画
    /// </summary>
    public static void ReadyPopupAnim(Transform tra)
    {
        tra.localScale = Vector3.zero;
    }

    /// <summary>
    /// 开始弹窗动画 从无到大，再从大缩小到正常尺寸类似于0%—120%—100% // 先大在缩小到无，类似于100%—120%—0%
    /// </summary>
    public static void StartPopupAnim(Transform tra, Action action = null)
    {
        tra.DOScale(Vector3.one * 1.2f, 0.15f).OnComplete(() => { tra.DOScale(Vector3.one, 0.1f).OnComplete(() => { action?.Invoke(); }); });
    }

    /// <summary>
    /// 取消弹窗动画 先大在缩小到无，类似于100%—120%—0%
    /// </summary>
    public static void CancelPopAnim(Transform tra, Action action)
    {
        tra.DOScale(Vector3.one * 1.2f, 0.1f).OnComplete(() => { tra.DOScale(new Vector3(0, 0, 0), 0.15f).OnComplete(() => { action?.Invoke(); }); });
    }

    /// <summary>
    /// 开始弹窗动画 从无到大，再从大缩小到正常尺寸类似于0%—120%—100%
    /// </summary>
    public static void StartPopupAnim(Transform tra, Vector3 targetScale, Action action = null)
    {

        tra.DOScale(targetScale * 1.2f, 0.15f).OnComplete(() => { tra.DOScale(targetScale, 0.1f).OnComplete(() => { action?.Invoke(); }); });
    }

    /// <summary>
    /// 取消弹窗动画 先大在缩小到无，类似于100%—120%—0%
    /// </summary>
    public static void CancelPopAnim(Transform tra, Action action, Vector3 targetScale)
    {
        tra.DOScale(targetScale * 1.2f, 0.1f).OnComplete(() => { tra.DOScale(new Vector3(0, 0, 0), 0.15f).OnComplete(() => { action?.Invoke(); }); });
    }

    #endregion

    #region 渐隐放大出现，放大比例较小，类似于80%—100% // 渐隐缩小消失，类似于100%—80%

    /// <summary>
    /// 准备ui动画2
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="scale"></param>
    public static void ReadyUIAnimTwo(CanvasGroup canvasGroup, Transform tra/*, float scale = 1.0f*/)
    {
        canvasGroup.alpha = 0.0f;
        tra.localScale = Vector3.one * 0.8f;
    }

    /// <summary>
    /// 开始ui动画2 渐隐放大出现，放大比例较小，类似于80%—100%
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="tra"></param>
    /// <param name="scale"></param>
    public static void StartUIAnimTwo(CanvasGroup canvasGroup, Transform tra, Action action = null, float scale = 1.0f)
    {
        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 1.0f, 0.35f);
        tra.DOScale(Vector3.one * scale, 0.35f).OnComplete(() => { action?.Invoke(); });
    }


    /// <summary>
    /// 关闭/取消ui动画2 渐隐缩小消失，类似于100%—80%
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="tra"></param>
    /// <param name="scale"></param>
    public static void CancelUIAnimTwo(CanvasGroup canvasGroup, Transform tra, Action action, float scale = 1.0f)
    {
        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 0.0f, 0.35f).OnComplete(() => { action?.Invoke(); });
        tra.DOScale(Vector3.one * scale * 0.8f, 0.25f);
    }

    #endregion

    #region 加载/保存本地存储数据

    /// <summary>
    /// 加载本地存储数据
    /// </summary>
    public static void LoadLocalSaveData()
    {
        string path = UniversalTool.GetSaveFilePath("LocalSaveData.json");
        string json = UniversalTool.LoadJson(path);
        if (!string.IsNullOrEmpty(json))
        {
            StaticData.playerInfoData.CurLocalSaveData = JsonMapper.ToObject<LocalSaveData>(json);
        }
    }

    /// <summary>
    /// 保存本地存储数据
    /// </summary>
    public static void SaveLocalSaveData()
    {
        string path = UniversalTool.GetSaveFilePath("LocalSaveData.json");
        string jsonStr = JsonMapper.ToJson(StaticData.playerInfoData.CurLocalSaveData);
        UniversalTool.SaveJson(path, jsonStr);
    }

    #endregion

}
