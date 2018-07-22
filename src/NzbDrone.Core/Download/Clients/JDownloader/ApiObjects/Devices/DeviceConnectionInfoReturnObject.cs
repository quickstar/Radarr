﻿using System.Collections.Generic;

namespace NzbDrone.Core.Download.Clients.JDownloader.ApiObjects.Devices
{
    public class DeviceConnectionInfoReturnObject
    {
        public List<DeviceConnectionInfoObject> Infos { get; set; }
        public bool RebindProtectionDetected { get; set; }
        public string Mode { get; set; }
    }
}
