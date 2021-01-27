using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using static StaticData;

public static class LocalizationDefineHelper
{
    public static string GetStringNameById(int idDefineLocalize)
    {
        string localName = StaticData.GetMultilingual(idDefineLocalize);
        return localName;
    }

    public static string GetImageNameById(int idDefineLocalize)
    {
        string imageName = StaticData.GetMultilingual(idDefineLocalize);
        return imageName;
    }
}
