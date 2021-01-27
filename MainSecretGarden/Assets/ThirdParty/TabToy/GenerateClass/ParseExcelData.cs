using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ParseExcelData
{
    public Config config;
    private byte[] bytes;

    /// <summary>
    /// 初始化数据
    /// 运行时改为手动调用
    /// </summary>
    public void InitData()
    {
        //var t = Time.realtimeSinceStartup;
        MemoryStream ms = new MemoryStream(bytes);
        var reader = new tabtoy.DataReader(ms, ms.Length);
        if (!reader.ReadHeader())
        {
            return;
        }

        Config.Deserialize(config, reader);
    }
    // Start is called before the first frame update
    public Config Init()
    {
        config = new Config();
        bytes = ABManager.GetAsset<TextAsset>("tabtoyData").bytes;
        InitData();
        return config;
    }

    public Config InitEditor()
    {
        config = new Config();
        bytes = ABManager.GetAssetRes<TextAsset>("tabtoyData", "tabtoydata.unity3d").bytes;
        InitData();
        return config;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
