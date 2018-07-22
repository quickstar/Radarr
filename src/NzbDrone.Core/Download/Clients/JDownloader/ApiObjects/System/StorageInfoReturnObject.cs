namespace NzbDrone.Core.Download.Clients.JDownloader.ApiObjects.System
{
    public class StorageInfoReturnObject
    {
        public string Path { get; set; }
        public object Error { get; set; }
        public long Size { get; set; }
        public long Free { get; set; }
    }
}
