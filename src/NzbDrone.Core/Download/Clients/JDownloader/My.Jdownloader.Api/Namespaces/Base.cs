using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public abstract class Base
    {
        internal Base() {}

        internal JDownloaderApiHandler ApiHandler;
        public DeviceObject Device;
    }
}
