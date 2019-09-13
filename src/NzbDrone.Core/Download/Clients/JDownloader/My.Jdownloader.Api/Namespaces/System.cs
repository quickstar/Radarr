using Newtonsoft.Json.Linq;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.System;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class System : Base
    {
        internal System(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
            : base(apiHandler, device, loginObject)
        { }

        /// <summary>
        /// Closes the JDownloader client.
        /// </summary>
        public void ExitJd()
        {
            ApiHandler.CallAction<object>(Device, "/system/exitJD", null, LoginObject, true);
        }

        /// <summary>
        /// Gets storage informations of the given path.
        /// </summary>
        /// <param name="path">The Path you want to check.</param>
        /// <returns>An array with storage informations.</returns>
        public StorageInfoReturnObject[] GetStorageInfos(string path)
        {
            var param = new[] { path };
            var tmp = ApiHandler.CallAction<DefaultReturnObject>(Device, "/system/getStorageInfos", param, LoginObject, true);

            var data = (JArray)tmp?.Data;
            return data?.ToObject<StorageInfoReturnObject[]>();
        }

        /// <summary>
        /// Gets information of the system the JDownloader client is running on.
        /// </summary>
        /// <returns></returns>
        public SystemInfoReturnObject GetSystemInfos()
        {
            var tmp = ApiHandler.CallAction<DefaultReturnObject>(Device, "/system/getSystemInfos", null, LoginObject, true);

            var data = (JObject)tmp?.Data;
            return data?.ToObject<SystemInfoReturnObject>();
        }

        /// <summary>
        /// Hibernates the current os the JDownloader client is running on.
        /// </summary>
        public void HibernateOs()
        {
            ApiHandler.CallAction<object>(Device, "/system/hibernateOS", null, LoginObject, true);
        }

        /// <summary>
        /// Restarts the JDownloader client.
        /// </summary>
        public void RestartJd()
        {
            ApiHandler.CallAction<object>(Device, "/system/restartJD", null, LoginObject, true);
        }

        /// <summary>
        /// Shutsdown the current os the JDownloader client is running on.
        /// </summary>
        /// <param name="force">True if you want to force the shutdown process.</param>
        public void ShutdownOs(bool force)
        {
            ApiHandler.CallAction<object>(Device, "/system/shutdownOS", new[] { force }, LoginObject, true);
        }

        /// <summary>
        /// Sets the current os the JDownloader client is running on in standby.
        /// </summary>
        public void StandbyOs()
        {
            ApiHandler.CallAction<object>(Device, "/system/standbyOS", null, LoginObject, true);
        }
    }
}
