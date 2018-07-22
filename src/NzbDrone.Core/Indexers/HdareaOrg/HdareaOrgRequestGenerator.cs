using System;
using System.Collections.Generic;
using NzbDrone.Common.Http;
using NzbDrone.Core.IndexerSearch.Definitions;

namespace NzbDrone.Core.Indexers.HdareaOrg
{
    public class HdareaOrgRequestGenerator : IIndexerRequestGenerator
    {
        public HdareaOrgSettings Settings { get; set; }

        public virtual IndexerPageableRequestChain GetRecentRequests()
        {
            var pageableRequests = new IndexerPageableRequestChain();
            pageableRequests.Add(GetRequest(null));
            return pageableRequests;
        }

        public IndexerPageableRequestChain GetSearchRequests(MovieSearchCriteria searchCriteria)
        {
            var pageableRequests = new IndexerPageableRequestChain();
            pageableRequests.Add(GetRequest(RemoveSpecialChars(searchCriteria.Movie.Title)));
            searchCriteria.Movie.AlternativeTitles.ForEach(t => pageableRequests.Add(GetRequest(RemoveSpecialChars(t.Title))));
            return pageableRequests;
        }

        private static string RemoveSpecialChars(string title)
        {
            return title.Replace("'", string.Empty).Replace(":", string.Empty);
        }

        public Func<IDictionary<string, string>> GetCookies { get; set; }
        public Action<IDictionary<string, string>, DateTime?> CookiesUpdater { get; set; }

        private IEnumerable<IndexerRequest> GetRequest(string searchParameters)
        {
            if (searchParameters != null)
            {
                searchParameters = Parser.Parser.ReplaceGermanUmlauts(searchParameters).Replace(" ", "+");
                yield return new IndexerRequest($"{Settings.BaseUrl.Trim().TrimEnd('/')}/?s=search&q={searchParameters}", HttpAccept.Html);
            }
            else
            {
                yield return new IndexerRequest($"{Settings.BaseUrl.Trim().TrimEnd('/')}/?s=top-rls", HttpAccept.Html);
            }

        }
    }
}
