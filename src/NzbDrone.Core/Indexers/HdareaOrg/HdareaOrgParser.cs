using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using HtmlAgilityPack;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Indexers.Exceptions;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Indexers.HdareaOrg
{
    public class HdareaOrgParser : IParseIndexerResponse
    {
        private readonly HdareaOrgSettings _settings;
        private readonly Logger _logger;

        public HdareaOrgParser(HdareaOrgSettings settings, Logger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse)
        {
            if (indexerResponse.HttpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new IndexerException(indexerResponse,
                    "Unexpected response status {0} code from API request",
                    indexerResponse.HttpResponse.StatusCode);
            }

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(indexerResponse.Content);

                var rawLinks = doc.DocumentNode.Descendants(0).FirstOrDefault(n => n.HasClass("whitecontent"))?.Descendants("a")
                    .Where(n => n.HasAttributes && n.Attributes.Contains("title"));

                DateTime start = DateTime.UtcNow;
                var links = rawLinks.AsParallel().Select(a =>
                {
                    var href = a.Attributes["href"];
                    var title = a.Attributes["title"];
                    var detailPage = FetchLinkDetail(href.Value);
                    return new JDownloaderInfo
                    {
                        DownloadProtocol = DownloadProtocol.Dlc,
                        Guid = $"HdareaOrg-{GetId(href)}",
                        Title = title.Value,
                        PublishDate = GetDate(a).ToUniversalTime(),
                        InfoUrl = href.Value,
                        DownloadUrl = GetDownloadUrl(detailPage),
                        ShareOnlineBizUrl = GetDownloadUrl(detailPage),
                        ImdbId = GetImdb(detailPage),
                        Size = GetSize(detailPage),
                    };
                }).Cast<ReleaseInfo>().OrderBy(jdi => jdi.PublishDate).ToList();
                DateTime stop = DateTime.UtcNow;
                var elapsedTime = stop - start;
                _logger.Info(elapsedTime.ToString());

                return links;
            }
            catch (XmlException)
            {
                throw new IndexerException(indexerResponse, "An error occurred while processing feed, feed invalid");
            }
        }

        public Action<IDictionary<string, string>, DateTime?> CookiesUpdater { get; set; }

        private long GetSize(HtmlNode detailPage)
        {
            var sizeUnparsed = detailPage.Descendants(0)
                .SingleOrDefault(n => n.HasClass("main") && n.InnerText.Contains("Größe"))?.NextSibling.InnerText;

            if (string.IsNullOrEmpty(sizeUnparsed))
            {
                return 0;
            }

            double size = 0;
            var trim = sizeUnparsed.Trim();
            trim = trim.Substring(0, trim.IndexOf(' '));
            trim = trim.Replace(',', '.');
 
            size = double.Parse(trim);
            if (sizeUnparsed.ContainsIgnoreCase("GiB") || sizeUnparsed.ContainsIgnoreCase("GB"))
            {
                size *= 1024;
            }


            var l = (long)(size * Math.Pow(1024, 2));
            return l;
        }

        private int GetImdb(HtmlNode detailPage)
        {
            var singleOrDefault = detailPage.Descendants(0).SingleOrDefault(n => n.HasClass("boxrechts"));
            var imdbLink = singleOrDefault?.ChildNodes.SingleOrDefault(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && n.HasAttributes)?.Attributes["href"].Value;

            var imdbId = 0;
            if (imdbLink == null)
            {
                return imdbId;
            }

            var urlSplits = imdbLink?.TrimEnd('/').Split('/');
            for (int i = urlSplits.Length-1;i >= 0; i--)
            {
                var imdbFullId = urlSplits?[i];
                if (imdbFullId?.Length > 2)
                {
                    if (int.TryParse(imdbFullId.Substring(2), out imdbId))
                    {
                        break;
                    }
                }
            }
            return imdbId;
        }

        private HtmlNode FetchLinkDetail(string infoUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(infoUrl);

            var rootNodes = doc.DocumentNode.Descendants(0).FirstOrDefault(n => n.Id.Equals("content", StringComparison.OrdinalIgnoreCase));
            return rootNodes;
        }

        private DateTime GetDate(HtmlNode link)
        {
            var linkPreviousSibling = link.PreviousSibling.InnerText.Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
            DateTime date;
            if (DateTime.TryParse(linkPreviousSibling, out date))
            {
                return date;
            }

            return DateTime.MinValue;
        }

        private string GetId(HtmlAttribute href)
        {
            var query = href.Value.Split('?');
            if (query.Length != 2)
            {
                return string.Empty;
            }

            var idSplit = query[1].Split('=');
            if (idSplit.Length != 2)
            {
                return string.Empty;
            }

            return idSplit[1];
        }

        private string GetDownloadUrl(HtmlNode detailPage)
        {
            var linkList = detailPage.Descendants("span").Where(n =>
                n.Attributes.Any(a =>
                    a.Name.EqualsIgnoreCase("style") && a.Value.EqualsIgnoreCase("display:inline;")));

            foreach (var link in linkList.Where(l => l.FirstChild.InnerText.EqualsIgnoreCase("Share-Online.biz")))
            {
                var escapedValue = link.FirstChild.Attributes["href"].Value;
                var unescaped = HtmlEntity.DeEntitize(escapedValue);
                return unescaped;
            }

            return string.Empty;
        }
    }
}
