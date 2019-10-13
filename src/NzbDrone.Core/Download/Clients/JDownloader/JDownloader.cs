using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

using Jdownloader.Api.Models.DownloadsV2;

using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RemotePathMappings;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public class JDownloader : DlcClientBase<JDownloaderSettings>
    {
        public JDownloader(IJDownloaderProxy proxy,
                      IConfigService configService,
                      INamingConfigService namingConfigService,
                      IDiskProvider diskProvider,
                      IRemotePathMappingService remotePathMappingService,
                      Logger logger)
            : base(proxy, configService, namingConfigService, diskProvider, remotePathMappingService, logger)
        {
            proxy.InitApi(Settings);
        }

        public override string Name => "JDownloader";

        public override string Download(RemoteMovie remoteMovie)
        {
            if (Proxy.AddDlcFromUrl(remoteMovie.Release.DownloadUrl, remoteMovie.Release.Title, Settings))
            {
                Proxy.CheckPackage(Settings, remoteMovie.Release.Title);
            }

            return string.Empty;
        }

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            var globalStatus = Proxy.GetGlobalStatus(Settings);
            var packages = Proxy.GetDownloadQueue(Settings);
            var downloadItems = packages.Select(p => new DownloadClientItem
            {
                DownloadId = p.UUID.ToString(),
                CanBeRemoved = true,
                CanMoveFiles = true,
                DownloadClient = Definition.Name,
                TotalSize = p.BytesTotal,
                Title = p.Name,
                Status = GetDownloadItemStatus(p, globalStatus),
                RemainingSize = p.BytesTotal - p.BytesLoaded,
                RemainingTime = TimeSpan.FromSeconds(p.Eta)
            });
            var completedList = downloadItems.Where(p => p.Status == DownloadItemStatus.Completed).ToList();
            var downloadFolders = Proxy.GetDownloadFolder(Settings);
            foreach (var completedPackage in completedList)
            {
                var completeRemotePath = downloadFolders.FirstOrDefault().TrimEnd("/") + "/" + completedPackage.Title;
                var osPath = new OsPath(completeRemotePath);
                var remappedPath = _remotePathMappingService.RemapRemoteToLocal(Settings.Host, osPath);
                completedPackage.OutputPath = remappedPath;
            }
            return completedList;
        }

        private DownloadItemStatus GetDownloadItemStatus(FilePackageDto package, string status)
        {
            if (package.Status.Contains("Extraction OK"))
            {
                return DownloadItemStatus.Completed;
            }

            if (status == "PAUSE")
            {
                return DownloadItemStatus.Paused;
            }
            if (status == "STOPPED_STATE")
            {
                return DownloadItemStatus.Queued;
            }

            if (package.Running)
            {
                return DownloadItemStatus.Downloading;
            }

            return DownloadItemStatus.Failed;
        }

        public override void RemoveItem(string downloadId, bool deleteData)
        {
        }

        public override DownloadClientStatus GetStatus()
        {
            var downloadFolders = Proxy.GetDownloadFolder(Settings);
            return new DownloadClientStatus
            {
                IsLocalhost = false,
                OutputRootFolders = downloadFolders.Select(df => new OsPath(df)).ToList()
            };
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(TestConnection());
        }

        private ValidationFailure TestConnection()
        {
            try
            {
                var version = Proxy.GetVersion(Settings);
                var versionP = int.Parse(version);

                if (versionP < 2)
                {
                    return new ValidationFailure(string.Empty, "JDownlaoder version too low, need 2.0 or higher");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ContainsIgnoreCase("Authentication failed"))
                {
                    return new ValidationFailure("Username", "Authentication failed");
                }
                _logger.Error(ex, ex.Message);
                return new ValidationFailure("Host", "Unable to connect to NZBGet");
            }

            return null;
        }
    }
}
