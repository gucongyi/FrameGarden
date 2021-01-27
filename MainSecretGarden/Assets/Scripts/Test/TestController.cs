using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Protocal;
using System;
using System.IO;

/// <summary>
/// 测试控制类 用于编写测试代码
/// </summary>
public class TestController : MonoBehaviour
{

    #region 变量


    #endregion

    #region 方法

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //测试代码
        if (Input.GetKeyDown(KeyCode.P))
        {
            StaticData.EnterUIDeal();

            //Compounding(46, 0.05f, 61);
            //OpenZillionaire();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenUILevelUp();
            //SaveScreenShot(this, new Rect(0, 0, Screen.width, Screen.height));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            LogoutAccount();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            ServerForcedUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            //OpenZilliEventEffect();
            SetGameTiem();
        }
    }

    /// <summary>
    /// 打开升级奖励界面
    /// </summary>
    private void OpenUILevelUp()
    {
        StaticData.RequestUpgradeSuccess(1, 2);
    }


    /// <summary>
    /// 打开大富翁
    /// </summary>
    private async void OpenZillionaire()
    {
        await StaticData.OpenMonopoly();
    }

    /// <summary>
    /// 注销账号
    /// </summary>
    private void LogoutAccount()
    {
        StaticData.NotifyServerLogoutAccount(LogoutAccountCallback);
    }

    private void LogoutAccountCallback(bool isSuccess)
    {
        if (isSuccess && WebSocketComponent._instance != null)
        {
            WebSocketComponent._instance.QuitUILogin();
            //StaticData.ReturnUILogin();
        }
    }

    /// <summary>
    /// 服务器强制更新
    /// </summary>
    private void ServerForcedUpdate()
    {
        //_isAutoDisconnect = true;
        ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UITipUpdateVersion", UIRoot.instance.GetUIRootCanvas().transform);
    }

    private async void OpenZilliEventEffect()
    {
        GameObject obj = null;
        ////int index = Random.Range(0, 2);
        //if (index == 0)
        //{
        //    obj = await ABManager.GetAssetAsync<GameObject>("ZillionaireOrdinaryEventEffect") as GameObject;
        //    var effect = Instantiate(obj, UIRoot.instance.GetUIRootCanvas().transform);
        //    effect.GetComponent<ZillionaireOrdinaryEventEffectController>().InitValue(1002);
        //}
        //else
        //{
        //    obj = await ABManager.GetAssetAsync<GameObject>("ZillionaireRandomEventEffect") as GameObject;
        //    var effect = Instantiate(obj, UIRoot.instance.GetUIRootCanvas().transform);
        //    effect.GetComponent<ZillionaireOrdinaryEventEffectController>().InitValue(2008);
        //}
    }

    private void SetGameTiem()
    {
        UnityEngine.Time.timeScale = 0.1f;
    }

    //unity渲染截图
    public static IEnumerator ScreenShot(Rect rect, Action<Texture2D> outTextrue)
    {
        //只在每一帧渲染完成后才读取屏幕信息
        yield return new WaitForEndOfFrame();
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        // 读取屏幕像素信息并存储为纹理数据，  
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        outTextrue?.Invoke(screenShot);
    }

    //保存图片的app目录下
    public static void SaveScreenShot(MonoBehaviour behaviour, Rect rect, Action<string> outPath = null)
    {
        behaviour.StartCoroutine(ScreenShot(rect, texture2D =>
        {
            string imgDir = Application.persistentDataPath + "/IMG";
            if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);
            string fileName = $"{Application.productName}_{ TimeHelper.ServerTimeStampNow}.png";
            string imgPath = Path.Combine(imgDir, fileName);
            byte[] imgData = texture2D.EncodeToPNG();
            File.WriteAllBytes(imgPath, imgData);
            outPath?.Invoke(imgPath);
        }));
    }


    private void Compounding(float principal, float interestRate, int years) 
    {
        float def = principal;
        for (int i = 0; i < years; i++)
        {
            principal += principal * interestRate;
        }
        Debug.Log(string.Format("本金{0},利率{1},年限{2},最终金额{3}", def, interestRate, years, principal));
    }
    #endregion
}
