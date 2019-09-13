namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Captcha
{
    public enum SkipRequest
    {
        SINGLE,
        BLOCK_HOSTER,
        BLOCK_ALL_CAPTCHAS,
        BLOCK_PACKAGE,
        REFRESH,
        STOP_CURRENT_ACTION,
        TIMEOUT
    }
}
