using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class DownloadController : Base
    {
        internal DownloadController(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
            : base(apiHandler, device, loginObject)
        { }

        /// <summary>
        /// Forces JDownloader to start downloading the given links/packages
        /// </summary>
        /// <param name="linkIds">The ids of the links you want to force download.</param>
        /// <param name="packageIds">The ids of the packages you want to force download.</param>
        /// <returns>True if successfull</returns>
        public bool ForceDownload(long[] linkIds, long[] packageIds)
        {
            var param = new[] { linkIds, packageIds };
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/forceDownload", param, LoginObject, true);
            return result != null;
        }

        /// <summary>
        /// Gets the current state of the device
        /// </summary>
        /// <returns>The current state of the device.</returns>
        public string GetCurrentState()
        {
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/getCurrentState", null, LoginObject, true);
            if (result != null)
            {
                return (string)result.Data;
            }

            return "UNKOWN_STATE";
        }

        /// <summary>
        /// Gets the actual download speed of the client.
        /// </summary>
        /// <returns>The actual download speed.</returns>
        public long GetSpeedInBps()
        {
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/getSpeedInBps", null, LoginObject, true);
            if (result != null)
            {
                return (long)result.Data;
            }

            return 0;
        }

        /// <summary>
        /// Pauses all downloads.
        /// </summary>
        /// <param name="pause">True if you want to pause the download</param>
        /// <returns>True if successfull.</returns>
        public bool Pause(bool pause)
        {
            var param = new[] { pause };
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/pause", param, LoginObject, true);
            if (result != null)
            {
                return (bool)result.Data;
            }

            return false;
        }

        /// <summary>
        /// Starts all downloads.
        /// </summary>
        /// <returns>True if successfull.</returns>
        public bool Start()
        {
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/stop", null, LoginObject, true);
            if (result != null)
            {
                return (bool)result.Data;
            }

            return false;
        }

        /// <summary>
        /// Stops all downloads.
        /// </summary>
        /// <returns>True if successfull.</returns>
        public bool Stop()
        {
            var result = ApiHandler.CallAction<DefaultReturnObject>(Device, "/downloadcontroller/start", null, LoginObject, true);
            if (result != null)
            {
                return (bool)result.Data;
            }

            return false;
        }
    }
}
