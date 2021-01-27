using System;
using System.ComponentModel;
using System.IO;
using LitJson;
public static class JsonHelper
{
    public static string ToJson(object obj)
    {
        return JsonMapper.ToJson(obj);
    }

    public static T FromJson<T>(string str)
    {
        T t = JsonMapper.ToObject<T>(str);
        ISupportInitialize iSupportInitialize = t as ISupportInitialize;
        if (iSupportInitialize == null)
        {
            return t;
        }
        iSupportInitialize.EndInit();
        return t;
    }
    public static T Clone<T>(T t)
    {
        return FromJson<T>(ToJson(t));
    }
    public static T LoadData<T>() where T : GameDataMeta
    {
        var fileName = typeof(T).ToString();
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        
        T data = default(T);
        string jsonString=LoadJson(path);
        StaticData.DebugGreen($"======json path UnSerializeField:{path} jsonString:{jsonString}");
        if (!string.IsNullOrEmpty(jsonString))
        {
            data = JsonMapper.ToObject<T>(jsonString);
        }
        return data;
    }
    public static void SaveDataToPersist<T>(T data) where T:GameDataMeta
    {
        var fileName = typeof(T).ToString();
        string json = JsonMapper.ToJson(data);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }
        string path = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        //StaticData.DebugGreen($"======json path SerializeField:{path} json:{json}");
        SaveJson(path, json);
    }
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
}