using System.Collections.Generic;
using System.Threading;
using NLog;
using NzbDrone.Common.Cache;
using NzbDrone.Core.Download.Clients.JDownloader.ApiObjects.LinkgrabberV2;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public interface IJDownloaderProxy
    {
        long AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings);
        string GetVersion(JDownloaderSettings settings);
        void CheckPackage(JDownloaderSettings settings, long packageId, string packageName);
    }

    public class JDownloaderProxy : IJDownloaderProxy
    {
        private readonly Logger _logger;
        private readonly ICached<string> _versionCache;

        public JDownloaderProxy(ICacheManager cacheManager, Logger logger)
        {
            _logger = logger;
            _versionCache = cacheManager.GetCache<string>(GetType(), "versions");
        }

        public long AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings)
        {
            JDownloaderHandler jdownloaderHandler = new JDownloaderHandler(settings.EMail, settings.Password, "JDownloaderProxy");
            if (jdownloaderHandler.IsConnected)
            {
                var devices = jdownloaderHandler.GetDevices();
                foreach (var device in devices)
                {
                    var deviceHandler = jdownloaderHandler.GetDeviceHandler(device);
                    var addLink = new AddLinkRequestObject { AutoExtract = true, AutoStart = false, Links = dlcLink, PackageName = packageName };
                    var packageId = deviceHandler.LinkgrabberV2.AddLinks(addLink);
                    return packageId;
                }
            }
            return 0;
        }

        public string GetVersion(JDownloaderSettings settings)
        {
            JDownloaderHandler jdownloaderHandler = new JDownloaderHandler(settings.EMail, settings.Password, "JDownloaderProxy");
            if (jdownloaderHandler.IsConnected)
            {
                var devices = jdownloaderHandler.GetDevices();
                foreach (var device in devices)
                {
                    var deviceHandler = jdownloaderHandler.GetDeviceHandler(device);
                    var version = deviceHandler.Jd.Version().ToString();
                    return version;
                }
            }

            return "0";
        }

        public void CheckPackage(JDownloaderSettings settings, long packageId, string packageName)
        {
            JDownloaderHandler jdownloaderHandler = new JDownloaderHandler(settings.EMail, settings.Password, "JDownloaderProxy");
            if (jdownloaderHandler.IsConnected)
            {
                var devices = jdownloaderHandler.GetDevices();
                foreach (var device in devices)
                {
                    var deviceHandler = jdownloaderHandler.GetDeviceHandler(device);
                    while (deviceHandler.LinkgrabberV2.IsCollecting())
                    {
                        Thread.Sleep(5000);
                    }

                    List<QueryPackagesResponseObject> queryPackagesResponseObjects = deviceHandler.LinkgrabberV2.QueryPackages(new QueryPackagesRequestObject());
                    foreach (var package in queryPackagesResponseObjects)
                    {
                        if (packageId.ToString().Substring(0, 7) == package.Uuid.ToString().Substring(0, 7))
                        {
                        }

                        deviceHandler.LinkgrabberV2.RenamePackage(package.Uuid, packageName);
                    }
                }
            }
        }
    }
}
