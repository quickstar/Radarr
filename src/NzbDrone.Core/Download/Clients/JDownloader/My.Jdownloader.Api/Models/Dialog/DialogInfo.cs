using System.Collections.Generic;
using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Dialog
{
    public class DialogInfo
    {
        [JsonProperty(PropertyName = "properties")]
        public Dictionary<string,string> Properties{ get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type{ get; set; }
    }
}
