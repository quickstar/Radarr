using System.Collections.Generic;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.DownloadsV2;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public interface IJDownloaderProxy
    {
        bool AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings);

        void CheckPackage(JDownloaderSettings settings, string releaseTitle);

        IEnumerable<FilePackageObject> GetDownloadQueue(JDownloaderSettings settings);

        string GetGlobalStatus(JDownloaderSettings settings);

        string GetVersion(JDownloaderSettings settings);

        string[] GetDownloadFolder(JDownloaderSettings settings);
    }
}
