namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public enum JDownloaderPriority
    {
        Lowest = -150,
        Lower = -100,
        Low = -50,
        Default = 0,
        High = 50,
        Higher = 100,
        Highest = 900
    }
}
