namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Extraction
{
    public enum ControllerStatus
    {
        /// <summary>
        /// Extraction is currently running
        /// </summary>
        RUNNING,
        /// <summary>
        /// Archive is queued for extraction and will run as soon as possible
        /// </summary>
        QUEUED,
        /// <summary>
        /// No controller assigned
        /// </summary>
        NA
    }
}
