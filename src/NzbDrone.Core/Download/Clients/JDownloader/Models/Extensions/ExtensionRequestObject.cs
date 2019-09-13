namespace NzbDrone.Core.Download.Clients.JDownloader.Models.Extensions
{
    public class ExtensionRequestObject
    {
        public bool ConfigInterface { get; set; }
        public bool Description { get; set; }
        public bool Enabled { get; set; }
        public bool IconKey { get; set; }
        public bool Installed { get; set; }
        public bool Name { get; set; }
        public string Pattern { get; set; }
    }
}
