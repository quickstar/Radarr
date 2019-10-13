using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Jdownloader.Api;
using Jdownloader.Api.Models;
using Jdownloader.Api.Models.DownloadsV2;
using Jdownloader.Api.Models.LinkgrabberV2;

using NLog;

using NzbDrone.Common.Cache;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public class JDownloaderProxy : IJDownloaderProxy
    {
        private JDownloaderApi _jDownloaderApi;
        private readonly Logger _logger;
        private readonly ICached<string> _versionCache;

        public JDownloaderProxy(ICacheManager cacheManager, Logger logger)
        {
            _logger = logger;
            _versionCache = cacheManager.GetCache<string>(GetType(), "versions");
        }

        public void InitApi(JDownloaderSettings settings)
        {
            _jDownloaderApi = GetJDownloaderApi(settings);
        }

        public bool AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings)
        {
            var addLink = new AddLinkRequestDto
            {
                AutoExtract = true,
                AutoStart = false,
                Links = dlcLink,
                PackageName = packageName
            };
            return _jDownloaderApi.LinkgrabberV2.AddLinks(addLink);
        }

        public void CheckPackage(JDownloaderSettings settings, string packageName)
        {
            while (_jDownloaderApi.LinkgrabberV2.IsCollecting())
            {
                Thread.Sleep(5000);
            }

            var queryPackagesResponseObjects = _jDownloaderApi.LinkgrabberV2.QueryPackages(new QueryPackagesRequestDto());

            foreach (var package in queryPackagesResponseObjects)
            {
                _jDownloaderApi.LinkgrabberV2.RenamePackage(package.Uuid, packageName);
            }

            var packageToDownload = _jDownloaderApi
                .LinkgrabberV2
                .QueryPackages(new QueryPackagesRequestDto())
                .FirstOrDefault().Uuid;

            var queryLinks = _jDownloaderApi.LinkgrabberV2.QueryLinks();
            _jDownloaderApi.LinkgrabberV2.MoveToDownloadlist(queryLinks.Select(l => l.Id).ToArray(), new[] { packageToDownload });
        }

        public string[] GetDownloadFolder(JDownloaderSettings settings)
        {
            var downloadFolders = _jDownloaderApi.LinkgrabberV2.GetDownloadFolderHistorySelectionBase();
            return downloadFolders;
        }

        public string GetGlobalStatus(JDownloaderSettings settings)
        {
            var state = _jDownloaderApi.DownloadController.GetCurrentState();

            return state;
        }

        public string GetVersion(JDownloaderSettings settings)
        {
            var version = _jDownloaderApi.Jd.Version().ToString();
            return version;
        }

        public IEnumerable<FilePackageDto> GetDownloadQueue(JDownloaderSettings settings)
        {
            var packages = _jDownloaderApi.DownloadsV2.QueryPackages(new LinkQueryDto());
            return packages;
        }

        private JDownloaderApi GetJDownloaderApi(JDownloaderSettings settings)
        {
            var auth = new JDownloaderCredentials { Username = settings.EMail, Password = settings.Password };
            var jdContext = new JDownloaderFactory().Create(auth);
            DevicesDto availableDevices = jdContext.GetDevices();

            var deviceApi = jdContext.SetDevice(availableDevices.List.First());
            return deviceApi;
        }
    }
}
