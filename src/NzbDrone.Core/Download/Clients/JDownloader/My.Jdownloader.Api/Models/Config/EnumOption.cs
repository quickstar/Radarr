using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Config
{
    public class EnumOption
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
