using System.Collections.Generic;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices
{
    public class DeviceJsonReturnObject
    {
        [Newtonsoft.Json.JsonProperty(PropertyName ="list")]
        public List<DeviceObject> Devices { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "rid")]
        public int RequestId { get; set; }
    }
}
