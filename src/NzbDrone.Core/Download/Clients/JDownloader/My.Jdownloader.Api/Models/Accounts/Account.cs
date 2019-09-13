using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Accounts
{
    public class Account
    {
        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; }
        [JsonProperty(PropertyName = "uuid")]
        public long Uuid { get; set; }
    }
}
