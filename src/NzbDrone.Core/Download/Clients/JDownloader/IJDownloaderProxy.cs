using System.Collections.Generic;

using Jdownloader.Api.Models.DownloadsV2;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public interface IJDownloaderProxy
    {
        bool AddDlcFromUrl(string dlcLink, string packageName, JDownloaderSettings settings);

        void CheckPackage(JDownloaderSettings settings, string releaseTitle);

        string[] GetDownloadFolder(JDownloaderSettings settings);

        IEnumerable<FilePackageDto> GetDownloadQueue(JDownloaderSettings settings);

        string GetGlobalStatus(JDownloaderSettings settings);

        string GetVersion(JDownloaderSettings settings);

        void InitApi(JDownloaderSettings settings);
    }
}
