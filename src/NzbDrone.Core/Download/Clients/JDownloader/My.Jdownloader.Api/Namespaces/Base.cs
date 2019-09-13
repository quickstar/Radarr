using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public abstract class Base
    {
        internal Base(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
        {
            ApiHandler = apiHandler;
            Device = device;
            LoginObject = loginObject;
        }

        internal JDownloaderApiHandler ApiHandler { get; }

        public DeviceObject Device { get; }

        public LoginObject LoginObject { get; }
    }
}
