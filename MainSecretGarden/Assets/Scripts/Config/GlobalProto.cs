
public class GlobalProto
{
    //存放版本的路径
    public string VersionCodeParentDir;


    public class VersionCodeInfo
    {
        public float GameVersionCode;
        public int ResVersionCode;
    }
    public string GetNowLineCodeInfoPath()
    {
        string path = VersionCodeParentDir;
#if UNITY_ANDROID
        path += "Android/";
#elif UNITY_IOS
			path += "IOS/";
#elif UNITY_WEBGL
			path += "WebGL/";
#else
			path += "PC/";
#endif
        path += "NowLineCodeInfo.txt";
        return path;
    }
    public string GetUpdatingCodeInfoPath()
    {
        string path = VersionCodeParentDir;
#if UNITY_ANDROID
        path += "Android/";
#elif UNITY_IOS
			path += "IOS/";
#elif UNITY_WEBGL
			path += "WebGL/";
#else
			path += "PC/";
#endif
        path += "UpdatingCodeInfo.txt";
        return path;
    }

    public VersionCodeInfo GetNowLineCodeInfo(string json)
    {
        VersionCodeInfo NowLineCodeInfo=JsonHelper.FromJson<VersionCodeInfo>(json);
        return NowLineCodeInfo;
    }
    public VersionCodeInfo GetUpdatingCodeInfo(string json)
    {
        VersionCodeInfo NowLineCodeInfo = JsonHelper.FromJson<VersionCodeInfo>(json);
        return NowLineCodeInfo;
    }

    public string GetUrl()
    {
        string url = VersionCodeParentDir;
#if UNITY_ANDROID
        url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#else
			url += "PC/";
#endif
        url += StaticData.intParentResABDirectory + "/";
        return url;
    }
}
