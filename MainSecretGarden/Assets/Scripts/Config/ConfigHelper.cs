using System;
using UnityEngine;
public static class ConfigHelper
{
    public static string GetGlobal()
    {
        try
        {
            TextAsset configText = null;
            if (StaticData.isABUsedYunSever)
            {
                configText = (TextAsset)ResourcesHelper.Load("GlobalProtoOuter");
            }
            else
            {
                configText = (TextAsset)ResourcesHelper.Load("GlobalProto");
            }
            string configStr = configText.text;
            return configStr;
        }
        catch (Exception e)
        {
            throw new Exception($"load global config file fail", e);
        }
    }
}