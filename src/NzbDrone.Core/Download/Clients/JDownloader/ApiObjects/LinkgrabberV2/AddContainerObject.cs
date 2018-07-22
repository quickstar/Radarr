using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.ApiObjects.LinkgrabberV2
{
    public class AddContainerObject
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}
