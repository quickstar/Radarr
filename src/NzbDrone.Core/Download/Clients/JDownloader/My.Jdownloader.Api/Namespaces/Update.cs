using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class Update : Base
    {
        internal Update(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
            : base(apiHandler, device, loginObject)
        { }

        /// <summary>
        /// Checks if the client has an update available.
        /// </summary>
        /// <returns>True if an update is available.</returns>
        public bool IsUpdateAvailable()
        {
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/update/isUpdateAvailable", null, LoginObject, true);
            return (response?.Data != null) && (bool)response.Data;
        }

        /// <summary>
        /// Restarts the client and starts the update.
        /// </summary>
        public void RestartAndUpdate()
        {
            ApiHandler.CallAction<object>(Device, "/update/restartAndUpdate", null, LoginObject, true);
        }

        /// <summary>
        /// Start the update check on the client.
        /// </summary>
        public void RunUpdateCheck()
        {
            ApiHandler.CallAction<object>(Device, "/update/runUpdateCheck", null, LoginObject, true);
        }
    }
}
