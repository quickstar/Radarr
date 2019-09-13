using System.Collections.Generic;
using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Dialog
{
    public class DialogTypeInfo
    {
        [JsonProperty(PropertyName = "in")]
        public Dictionary<string, string> In { get; set; }
        [JsonProperty(PropertyName = "out")]
        public Dictionary<string, string> Out { get; set; }
    }
}
