using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class CaptchaForward : Base
    {
        internal CaptchaForward(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
            : base(apiHandler, device, loginObject)
        { }

        /// <summary>
        /// Creates a recaptcha job.
        /// For more informations https://my.jdownloader.org/developers/#tag_53.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <param name="four"></param>
        /// <returns>Propably the id of the created job.</returns>
        public long CreateJobRecaptchaV2(string one, string two, string three, string four)
        {
            var param = new[] { one, two, three, four };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/captchaforward/createJobRecaptchaV2", param, LoginObject, true);

            if (response?.Data != null)
            {
                return (long)response.Data;
            }

            return -1;
        }

        /// <summary>
        /// Gets the result of the captcha by the given id.
        /// </summary>
        /// <param name="id">The id of the job.</param>
        /// <returns>String which contians the result of the captcha.</returns>
        public string GetResult(long id)
        {
            var param = new[] { id };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/captchaforward/getResult", param, LoginObject, true);

            if (response?.Data != null)
            {
                return response.Data.ToString();
            }

            return string.Empty;
        }
    }
}
