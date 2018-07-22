using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Download.Clients.JDownloader;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.RemotePathMappings;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.Download
{
    public abstract class DlcClientBase<TSettings> : DownloadClientBase<TSettings>
        where TSettings : IProviderConfig, new()
    {
        protected DlcClientBase(IJDownloaderProxy proxy,
                                   IConfigService configService,
                                   INamingConfigService namingConfigService,
                                   IDiskProvider diskProvider,
                                   IRemotePathMappingService remotePathMappingService,
                                   Logger logger)
            : base(configService, namingConfigService, diskProvider, remotePathMappingService, logger)
        {
            Proxy = proxy;
        }

        public IJDownloaderProxy Proxy { get; }

        public override DownloadProtocol Protocol => DownloadProtocol.Dlc;
    }
}
