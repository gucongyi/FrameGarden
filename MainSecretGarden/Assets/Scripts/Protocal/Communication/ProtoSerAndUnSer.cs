using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ProtoSerAndUnSer
{
    // byte 反序列化成protobuf对象
    public static T UnSerialize<T>(byte[] data) where T : class, IMessage, new()
    {
        // 如果为空直接返回
        if (data == null || data.Length < 1)
        {
            return null;
        }
        try
        {
            return new T().Descriptor.Parser.ParseFrom(data) as T;
        }
        catch (Exception ex)
        {
            Debug.LogError("反序列化失败: " + ex.ToString());
            return null;
        }
    }

    // protobuf对象序列化成byte
    public static byte[] Serialize(IMessage obj)
    {
        // 如果为空直接返回
        if (obj == null)
        {
            return null;
        }
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                obj.WriteTo(ms);
                return ms.ToArray();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("序列化失败: " + ex.ToString());
            return null;
        }
    }
}
