using NLog;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser;

namespace NzbDrone.Core.Indexers.HdareaOrg
{
    public class HdareaOrg : HttpIndexerBase<HdareaOrgSettings>
    {
        public override string Name => "hd-area.org";
        public override DownloadProtocol Protocol => DownloadProtocol.JDownloader;
        public override bool SupportsRss => false;
        public override bool SupportsSearch => true;
        public override int PageSize => 50;

        public HdareaOrg(IHttpClient httpClient, IIndexerStatusService indexerStatusService, IConfigService configService, IParsingService parsingService, Logger logger)
            : base(httpClient, indexerStatusService, configService, parsingService, logger)
        { }

        public override IIndexerRequestGenerator GetRequestGenerator()
        {
            return new HdareaOrgRequestGenerator() { Settings = Settings };
        }

        public override IParseIndexerResponse GetParser()
        {
            return new HdareaOrgParser(Settings, _logger);
        }
    }
}
