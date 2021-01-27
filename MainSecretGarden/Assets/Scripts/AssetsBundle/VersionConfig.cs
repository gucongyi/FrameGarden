using System.Collections.Generic;
public class FileVersionInfo
{
    public string File;
    public string MD5;
    public long Size;
}

public class VersionConfig
{
    public int Version;

    public long TotalSize;
    public Dictionary<string, FileVersionInfo> FileInfoDict = new Dictionary<string, FileVersionInfo>();

    public void EndInit()
    {
        foreach (FileVersionInfo fileVersionInfo in this.FileInfoDict.Values)
        {
            this.TotalSize += fileVersionInfo.Size;
        }
    }
}