using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Download.Clients.Nzbget;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RemotePathMappings;
using NzbDrone.Core.Validation;

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
        }

        public override string Download(RemoteMovie remoteMovie)
        {
            var packageId = Proxy.AddDlcFromUrl(remoteMovie.Release.DownloadUrl, remoteMovie.Release.Title, Settings);

            Proxy.CheckPackage(Settings, packageId, remoteMovie.Release.Title);

            return string.Empty;
        }

        public override string Name => "JDownloader";

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            return new List<DownloadClientItem>();
        }

        public override void RemoveItem(string downloadId, bool deleteData)
        {
        }

        public override DownloadClientStatus GetStatus()
        {
            return new DownloadClientStatus() { };
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
