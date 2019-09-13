using System.Collections.Generic;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices
{
    public class DeviceConnectionInfoReturnObject
    {
        public List<DeviceConnectionInfoObject> Infos { get; set; }
        public bool RebindProtectionDetected { get; set; }
        public string Mode { get; set; }
    }
}
