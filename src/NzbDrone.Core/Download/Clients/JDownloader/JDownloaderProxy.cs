using System.Linq;
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
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return 0;
            }

            var addLink = new AddLinkRequestObject
            {
                AutoExtract = true,
                AutoStart = false,
                Links = dlcLink,
                PackageName = packageName
            };
            var packageId = deviceHandler.LinkgrabberV2.AddLinks(addLink);
            return packageId;
        }

        public string GetVersion(JDownloaderSettings settings)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return "0";
            }
            var version = deviceHandler.Jd.Version().ToString();
            return version;
        }

        public void CheckPackage(JDownloaderSettings settings, long packageId, string packageName)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return;
            }

            while (deviceHandler.LinkgrabberV2.IsCollecting())
            {
                Thread.Sleep(2000);
            }

            var queryPackagesResponseObjects = deviceHandler
                .LinkgrabberV2
                .QueryPackages(new QueryPackagesRequestObject())
                .Where(p => packageId.ToString().Substring(0, 7) == p.Uuid.ToString().Substring(0, 7));

            foreach (var package in queryPackagesResponseObjects)
            {
                deviceHandler.LinkgrabberV2.RenamePackage(package.Uuid, packageName);
            }

            var packageToDownload = deviceHandler
                .LinkgrabberV2
                .QueryPackages(new QueryPackagesRequestObject())
                .FirstOrDefault().Uuid;

            var queryLinks = deviceHandler.LinkgrabberV2.QueryLinks();
            deviceHandler.LinkgrabberV2.MoveToDownloadlist(queryLinks.Select(l => l.Id).ToArray(), new[] { packageToDownload });
        }

        public void GetDownloadQueue(JDownloaderSettings settings)
        {

        }

        private DeviceHandler GetDeviceHandler(JDownloaderSettings settings)
        {
            JDownloaderHandler jdownloaderHandler = new JDownloaderHandler(settings.EMail, settings.Password, "JDownloaderProxy");
            if (jdownloaderHandler.IsConnected)
            {
                var device = jdownloaderHandler.GetDevices().FirstOrDefault();
                return jdownloaderHandler.GetDeviceHandler(device);
            }

            return null;
        }
    }
}
