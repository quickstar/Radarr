using System.Collections.Generic;
using System.Web;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api
{
    public class JDownloaderHandler
    {
        private readonly JDownloaderApiHandler _apiHandler = new JDownloaderApiHandler();
        private byte[] _deviceSecret;
        private LoginObject _loginObject;
        private byte[] _loginSecret;

        /// <summary>
        /// Initializes the DownloadHandler, and tries to connect immediately.
        /// </summary>
        /// <param name="email">Your email of your my.jdownloader.org account.</param>
        /// <param name="password">Your password of your my.jdownloader.org account.</param>
        /// <param name="appKey">The name of the app. Should be unique!</param>
        public JDownloaderHandler(string email, string password, string appKey)
        {
            Connect(email, password);
            Utils.AppKey = appKey;
        }

        public bool IsConnected { get; set; }

        /// <summary>
        /// Creates an instance of the DeviceHandler class.
        /// This is neccessary to call methods!
        /// </summary>
        /// <param name="device">The device you want to call the methods on.</param>
        /// <returns>An deviceHandler instance.</returns>
        public DeviceHandler GetDeviceHandler(DeviceObject device, bool useJdownloaderApi = false)
        {
            if (IsConnected)
            {
                //TODO: Make it possible to directly connect to the jdownloader client. If it's not working use the relay server.
                //var tmp = _apiHandler.CallAction<DefaultReturnObject>(device, "/device/getDirectConnectionInfos",
                //    null, _loginObject, true);
                return new DeviceHandler(_apiHandler, device, _loginObject, useJdownloaderApi);
            }

            return null;
        }

        /// <summary>
        /// Lists all Devices which are currently connected to your my.jdownloader.org account.
        /// </summary>
        /// <returns>Returns an enumerable of your currently connected devices.</returns>
        public IEnumerable<DeviceObject> GetDevices()
        {
            List<DeviceObject> devices = new List<DeviceObject>();
            string query = $"/my/listdevices?sessiontoken={HttpUtility.UrlEncode(_loginObject.SessionToken)}";
            var response = _apiHandler.CallServer<DeviceJsonReturnObject>(query, _loginObject.ServerEncryptionToken);
            if (response == null)
            {
                return devices;
            }

            foreach (DeviceObject device in response.Devices)
            {
                devices.Add(device);
            }

            return devices;
        }

        #region "Connection methods"

        /// <summary>
        /// Fires a connection request to the api.
        /// </summary>
        /// <param name="email">Email of the User</param>
        /// <param name="password">Password of the User</param>
        /// <returns>Return if the connection was succesful</returns>
        public bool Connect(string email, string password)
        {
            //Calculating the Login and Device secret
            _loginSecret = Utils.GetSecret(email, password, Utils.ServerDomain);
            _deviceSecret = Utils.GetSecret(email, password, Utils.DeviceDomain);

            //Creating the query for the connection request
            string connectQueryUrl = $"/my/connect?email={HttpUtility.UrlEncode(email)}&appkey={HttpUtility.UrlEncode(Utils.AppKey)}";

            //Calling the query
            var response = _apiHandler.CallServer<LoginObject>(connectQueryUrl, _loginSecret);

            //If the response is null the connection was not successfull
            if (response == null)
            {
                return false;
            }

            //Else we are saving the response which contains the SessionToken, RegainToken and the RequestId
            _loginObject = response;
            _loginObject.Email = email;
            _loginObject.Password = password;
            _loginObject.ServerEncryptionToken = Utils.UpdateEncryptionToken(_loginSecret, _loginObject.SessionToken);
            _loginObject.DeviceEncryptionToken = Utils.UpdateEncryptionToken(_deviceSecret, _loginObject.SessionToken);

            IsConnected = true;
            return true;
        }

        /// <summary>
        /// Tries to reconnect your client to the api.
        /// </summary>
        /// <returns>True if successful else false</returns>
        public bool Reconnect()
        {
            string query = $"/my/reconnect?appkey{HttpUtility.UrlEncode(Utils.AppKey)}&sessiontoken={HttpUtility.UrlEncode(_loginObject.SessionToken)}&regaintoken={HttpUtility.UrlEncode(_loginObject.RegainToken)}";
            var response = _apiHandler.CallServer<LoginObject>(query, _loginObject.ServerEncryptionToken);
            if (response == null)
            {
                return false;
            }

            _loginObject = response;
            _loginObject.ServerEncryptionToken = Utils.UpdateEncryptionToken(_loginSecret, _loginObject.SessionToken);
            _loginObject.DeviceEncryptionToken = Utils.UpdateEncryptionToken(_deviceSecret, _loginObject.SessionToken);
            IsConnected = true;
            return IsConnected;
        }

        /// <summary>
        /// Disconnects the your client from the api
        /// </summary>
        /// <returns>True if successful else false</returns>
        public bool Disconnect()
        {
            string query = $"/my/disconnect?sessiontoken={HttpUtility.UrlEncode(_loginObject.SessionToken)}";
            var response = _apiHandler.CallServer<object>(query, _loginObject.ServerEncryptionToken);
            if (response == null)
            {
                return false;
            }

            _loginObject = null;
            return true;
        }

        #endregion
    }
}
