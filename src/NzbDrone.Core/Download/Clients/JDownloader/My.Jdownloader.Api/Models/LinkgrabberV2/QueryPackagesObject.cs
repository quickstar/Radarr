using System.Collections.Generic;
using Newtonsoft.Json;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.LinkgrabberV2
{
    internal class QueryPackagesObject
    {
        public List<QueryPackagesResponseObject> Data { get; set; }
        [JsonProperty(PropertyName = "rid")]
        public int RequestId { get; set; }
    }
}
