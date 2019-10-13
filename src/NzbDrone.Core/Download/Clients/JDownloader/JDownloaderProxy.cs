using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NLog;

using NzbDrone.Common.Cache;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.DownloadsV2;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.LinkgrabberV2;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public class JDownloaderProxy : IJDownloaderProxy
    {
        private static readonly object _lockObject = new object();
        private readonly Logger _logger;
        private readonly ICached<string> _versionCache;
        private DeviceHandler _deviceHandler;

        public JDownloaderProxy(ICacheManager cacheManager, Logger logger)
        {
            _logger = logger;
            _versionCache = cacheManager.GetCache<string>(GetType(), "versions");
        }

        public bool AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return false;
            }

            var addLink = new AddLinkRequestObject
            {
                AutoExtract = true,
                AutoStart = false,
                Links = dlcLink,
                PackageName = packageName
            };
            return deviceHandler.LinkgrabberV2.AddLinks(addLink);
        }

        public void CheckPackage(JDownloaderSettings settings, string packageName)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return;
            }

            while (deviceHandler.LinkgrabberV2.IsCollecting())
            {
                Thread.Sleep(5000);
            }

            var queryPackagesResponseObjects = deviceHandler
                .LinkgrabberV2
                .QueryPackages(new QueryPackagesRequestObject());

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

        public IEnumerable<FilePackageObject> GetDownloadQueue(JDownloaderSettings settings)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return new List<FilePackageObject>();
            }

            var packages = deviceHandler.DownloadsV2.QueryPackages(new LinkQueryObject());
            var downloadLinkObjects = deviceHandler.DownloadsV2.QueryLinks(new LinkQueryObject());
            string[] downloadFolder = deviceHandler.LinkgrabberV2.GetDownloadFolderHistorySelectionBase();
            return packages;
        }

        public string GetGlobalStatus(JDownloaderSettings settings)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return "0";
            }

            var state = deviceHandler.DownloadController.GetCurrentState();

            return state;
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

        public string[] GetDownloadFolder(JDownloaderSettings settings)
        {
            var deviceHandler = GetDeviceHandler(settings);
            if (deviceHandler == null)
            {
                return null;
            }

            var downloadFolders = deviceHandler.LinkgrabberV2.GetDownloadFolderHistorySelectionBase();
            return downloadFolders;
        }

        private DeviceHandler GetDeviceHandler(JDownloaderSettings settings)
        {
            if (_deviceHandler != null)
            {
                return _deviceHandler;
            }

            lock (_lockObject)
            {
                if (_deviceHandler != null)
                {
                    return _deviceHandler;
                }

                JDownloaderHandler jdownloaderHandler = new JDownloaderHandler(settings.EMail, settings.Password, "JDownloaderProxy");
                if (jdownloaderHandler.IsConnected)
                {
                    var device = jdownloaderHandler.GetDevices().FirstOrDefault();
                    _deviceHandler = jdownloaderHandler.GetDeviceHandler(device);
                    return _deviceHandler;
                }
            }

            return null;
        }
    }
}
