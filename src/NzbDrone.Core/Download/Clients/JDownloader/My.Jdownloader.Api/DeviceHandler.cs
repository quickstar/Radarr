using System.Collections.Generic;
using System.Web;

using Newtonsoft.Json.Linq;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces;

using Extensions = NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces.Extensions;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api
{
    public class DeviceHandler
    {
        public Accounts Accounts;
        public AccountsV2 AccountsV2;
        public Captcha Captcha;
        public CaptchaForward CaptchaForward;
        public Config Config;
        public Dialogs Dialogs;
        public DownloadController DownloadController;
        public DownloadsV2 DownloadsV2;
        public Extensions Extensions;
        public Extraction Extraction;

        public bool IsConnected;
        public Jd Jd;
        public LinkCrawler LinkCrawler;
        public LinkGrabberV2 LinkgrabberV2;
        public Namespaces.System System;
        public Namespaces.Update Update;
        private readonly JDownloaderApiHandler _apiHandler;
        private readonly DeviceObject _device;
        private byte[] _deviceSecret;

        private readonly LoginObject _loginObject;

        private byte[] _loginSecret;

        internal DeviceHandler(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject, bool useJdownloaderApi = false)
        {
            _device = device;
            _apiHandler = apiHandler;
            _loginObject = DirectConnect(useJdownloaderApi, loginObject);

            Accounts = new Accounts(_apiHandler, _device, _loginObject);
            AccountsV2 = new AccountsV2(_apiHandler, _device, _loginObject);
            Captcha = new Captcha(_apiHandler, _device, _loginObject);
            CaptchaForward = new CaptchaForward(_apiHandler, _device, _loginObject);
            Config = new Config(_apiHandler, _device, _loginObject);
            Dialogs = new Dialogs(_apiHandler, _device, _loginObject);
            DownloadController = new DownloadController(_apiHandler, _device, _loginObject);
            DownloadsV2 = new DownloadsV2(_apiHandler, _device, _loginObject);
            Extensions = new Extensions(_apiHandler, _device, _loginObject);
            Extraction = new Extraction(_apiHandler, _device, _loginObject);
            LinkCrawler = new LinkCrawler(_apiHandler, _device, _loginObject);
            LinkgrabberV2 = new LinkGrabberV2(_apiHandler, _device, _loginObject);
            Update = new Namespaces.Update(_apiHandler, _device, _loginObject);
            Jd = new Jd(_apiHandler, _device, _loginObject);
            System = new Namespaces.System(_apiHandler, _device, _loginObject);
        }

        private LoginObject Connect(string apiUrl, LoginObject parentLogin)
        {
            //Calculating the Login and Device secret
            _loginSecret = Utils.GetSecret(parentLogin.Email, parentLogin.Password, Utils.ServerDomain);
            _deviceSecret = Utils.GetSecret(parentLogin.Email, parentLogin.Password, Utils.DeviceDomain);

            //Creating the query for the connection request
            string connectQueryUrl = $"/my/connect?email={HttpUtility.UrlEncode(parentLogin.Email)}&appkey={HttpUtility.UrlEncode(Utils.AppKey)}";
            _apiHandler.SetApiUrl(apiUrl);
            //Calling the query
            var response = _apiHandler.CallServer<LoginObject>(connectQueryUrl, _loginSecret);

            //If the response is null the connection was not successful
            if (response == null)
            {
                return null;
            }

            response.Email = parentLogin.Email;
            response.Password = parentLogin.Password;

            //Else we are saving the response which contains the SessionToken, RegainToken and the RequestId
            var loginObject = response;
            loginObject.ServerEncryptionToken = Utils.UpdateEncryptionToken(_loginSecret, loginObject.SessionToken);
            loginObject.DeviceEncryptionToken = Utils.UpdateEncryptionToken(_deviceSecret, loginObject.SessionToken);
            IsConnected = true;
            return loginObject;
        }

        /// <summary>
        /// Tries to directly connect to the JDownloader Client.
        /// </summary>
        private LoginObject DirectConnect(bool useJdownloaderApi, LoginObject parentLogin)
        {
            bool connected = false;
            if (useJdownloaderApi)
            {
                return Connect("http://api.jdownloader.org", parentLogin);
            }

            foreach (var conInfos in GetDirectConnectionInfos(parentLogin))
            {
                var directConnectionLoginInfo = Connect(string.Concat("http://", conInfos.Ip, ":", conInfos.Port), parentLogin);
                if (directConnectionLoginInfo != null)
                {
                    connected = true;
                    return directConnectionLoginInfo;
                }
            }

            return Connect("http://api.jdownloader.org", parentLogin);
        }

        private List<DeviceConnectionInfoObject> GetDirectConnectionInfos(LoginObject loginObject)
        {
            var tmp = _apiHandler.CallAction<DefaultReturnObject>(_device, "/device/getDirectConnectionInfos", null, loginObject, true);
            if ((tmp.Data == null) || string.IsNullOrEmpty(tmp.Data.ToString()))
            {
                return new List<DeviceConnectionInfoObject>();
            }

            var jobj = (JObject)tmp.Data;
            var deviceConInfos = jobj.ToObject<DeviceConnectionInfoReturnObject>();

            return deviceConInfos.Infos;
        }
    }
}
