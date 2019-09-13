using NzbDrone.Core.Download.Clients.JDownloader.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.Models;
using NzbDrone.Core.Download.Clients.JDownloader.Models.Devices;

namespace NzbDrone.Core.Download.Clients.JDownloader.Namespaces
{
    public class LinkCrawler
    {
        private readonly JDownloaderApiHandler _ApiHandler;
        private readonly DeviceObject _Device;

        internal LinkCrawler(JDownloaderApiHandler apiHandler, DeviceObject device)
        {
            _ApiHandler = apiHandler;
            _Device = device;
        }

        /// <summary>
        /// Asks the client if the linkcrawler is still crawling.
        /// </summary>
        /// <returns>Ture if succesfull</returns>
        public bool IsCrawling()
        {
            var response =
                _ApiHandler.CallAction<DefaultReturnObject>(_Device, "/linkcrawler/isCrawling", null, JDownloaderHandler.LoginObject);
            if (response?.Data == null)
                return false;
            return (bool) response.Data;
        }
    }
}
