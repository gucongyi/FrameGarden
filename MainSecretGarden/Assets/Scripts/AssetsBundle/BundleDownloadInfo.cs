
public class BundleDownloadInfo
{
    public long TotalSize = 0;
    public long alreadyDownloadBytes = 0;
    public int Progress => (int)(alreadyDownloadBytes * 100f / this.TotalSize);
    public string AlreadyDownloadString => string.Format("{0:N2}KB/{1:N2}KB", alreadyDownloadBytes / 1024f, TotalSize / 1024f);
    /// <summary>
    /// 每秒下载多少byte
    /// .Format("{0,50}", theObj)
    /// </summary>
    public float DownLoadSpeed;
    //public string SpeedString => $"正在下载{string.Format("{0,8}", (int)DownLoadSpeed/1024)}KB/s ";
    public string SpeedString => $"{(int)DownLoadSpeed / 1024}KB/s ";
    public bool IsStart;
    internal bool IsEnd = false;
}
