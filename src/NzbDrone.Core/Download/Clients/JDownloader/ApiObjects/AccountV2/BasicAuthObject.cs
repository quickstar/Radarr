﻿namespace NzbDrone.Core.Download.Clients.JDownloader.ApiObjects.AccountV2
{
    public class BasicAuthObject
    {
        public HostType Type { get; set; }
        public string Hostmask { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
