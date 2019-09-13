using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class LinkCrawler : Base
    {

        internal LinkCrawler(JDownloaderApiHandler apiHandler, DeviceObject device)
        {
            ApiHandler = apiHandler;
            Device = device;
        }

        /// <summary>
        /// Asks the client if the linkcrawler is still crawling.
        /// </summary>
        /// <returns>Ture if succesfull</returns>
        public bool IsCrawling()
        {
            var response =
                ApiHandler.CallAction<DefaultReturnObject>(Device, "/linkcrawler/isCrawling", null, JDownloaderHandler.LoginObject);
            if (response?.Data == null)
                return false;
            return (bool) response.Data;
        }
    }
}
