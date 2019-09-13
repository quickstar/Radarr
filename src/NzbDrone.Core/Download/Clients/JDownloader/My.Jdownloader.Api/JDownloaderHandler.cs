using System.Collections.Generic;
using System.Web;
using NzbDrone.Core.Download.Clients.JDownloader.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader
{

        public class JDownloaderHandler
        {
            internal static LoginObject LoginObject;
            private readonly JDownloaderApiHandler _ApiHandler = new JDownloaderApiHandler();
            private byte[] _DeviceSecret;

            private byte[] _LoginSecret;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appkey">The name of the app. Should be unique!</param>
            public JDownloaderHandler(string appkey)
            {
                Utils.AppKey = appkey;
                InitializeClasses();
            }

            /// <summary>
            /// </summary>
            /// <param name="email">Your email of your my.jdownloader.org account.</param>
            /// <param name="password">Your password of your my.jdownloader.org account.</param>
            /// <param name="appKey">The name of the app. Should be unique!</param>
            public JDownloaderHandler(string email, string password, string appKey)
            {
                Connect(email, password);
                Utils.AppKey = appKey;
                InitializeClasses();
            }

            public bool IsConnected { get; set; }

            private void InitializeClasses()
            {
                //AccountsV2 = new AccountsV2(_ApiHandler);
                //DownloadController = new DownloadController(_ApiHandler);
                //Extensions = new Extensions(_ApiHandler);
                //Extraction = new Extraction(_ApiHandler);
                //LinkgrabberV2 = new LinkgrabberV2(_ApiHandler);
                //Update = new Update(_ApiHandler);
            }


            /// <summary>
            /// Lists all Devices which are currently connected to your my.jdownloader.org account.
            /// </summary>
            /// <returns>Returns a list of your currently connected devices.</returns>
            public List<DeviceObject> GetDevices()
            {
                List<DeviceObject> devices = new List<DeviceObject>();
                string query = $"/my/listdevices?sessiontoken={HttpUtility.UrlEncode(LoginObject.SessionToken)}";
                var response = _ApiHandler.CallServer<DeviceJsonReturnObject>(query, LoginObject.ServerEncryptionToken);
                if (response == null)
                    return devices;

                foreach (DeviceObject device in response.Devices)
                {
                    devices.Add(device);
                }

                return devices;
            }

            /// <summary>
            /// Creates an instance of the DeviceHandler class. 
            /// This is neccessary to call methods!
            /// </summary>
            /// <param name="device">The device you want to call the methods on.</param>
            /// <returns>An deviceHandler instance.</returns>
            public DeviceHandler GetDeviceHandler(DeviceObject device)
            {
                if (IsConnected)
                {
                    //TODO: Make it possible to directly connect to the jdownloader client. If it's not working use the relay server.
                    //var tmp = _ApiHandler.CallAction<DefaultReturnObject>(device, "/device/getDirectConnectionInfos",
                    //    null, LoginObject, true);
                    return new DeviceHandler(device, _ApiHandler, LoginObject);
                }
                return null;
            }

            #region "Connection methods"

            /// <summary>
            /// Fires a connection request to the api.
            /// </summary>
            /// <param name="email">Email of the User</param>
            /// <param name="password">Password of the User</param>
            /// <returns>Return if the Connection was succesfull</returns>
            public bool Connect(string email, string password)
            {
                //Calculating the Login and Device secret
                _LoginSecret = Utils.GetSecret(email, password, Utils.ServerDomain);
                _DeviceSecret = Utils.GetSecret(email, password, Utils.DeviceDomain);

                //Creating the query for the connection request
                string connectQueryUrl =
                    $"/my/connect?email={HttpUtility.UrlEncode(email)}&appkey={HttpUtility.UrlEncode(Utils.AppKey)}";

                //Calling the query
                var response = _ApiHandler.CallServer<LoginObject>(connectQueryUrl, _LoginSecret);

                //If the response is null the connection was not successfull
                if (response == null)
                    return false;

                //Else we are saving the response which contains the SessionToken, RegainToken and the RequestId
                LoginObject = response;
                LoginObject.Email = email;
                LoginObject.Password = password;
                LoginObject.ServerEncryptionToken = Utils.UpdateEncryptionToken(_LoginSecret, LoginObject.SessionToken);
                LoginObject.DeviceEncryptionToken = Utils.UpdateEncryptionToken(_DeviceSecret, LoginObject.SessionToken);
                IsConnected = true;
                return true;
            }

            /// <summary>
            /// Tries to reconnect your client to the api.
            /// </summary>
            /// <returns>True if successfull else false</returns>
            public bool Reconnect()
            {
                string query =
                    $"/my/reconnect?appkey{HttpUtility.UrlEncode(Utils.AppKey)}&sessiontoken={HttpUtility.UrlEncode(LoginObject.SessionToken)}&regaintoken={HttpUtility.UrlEncode(LoginObject.RegainToken)}";
                var response = _ApiHandler.CallServer<LoginObject>(query, LoginObject.ServerEncryptionToken);
                if (response == null)
                    return false;

                LoginObject = response;
                LoginObject.ServerEncryptionToken = Utils.UpdateEncryptionToken(_LoginSecret, LoginObject.SessionToken);
                LoginObject.DeviceEncryptionToken = Utils.UpdateEncryptionToken(_DeviceSecret, LoginObject.SessionToken);
                IsConnected = true;
                return IsConnected;
            }

            /// <summary>
            /// Disconnects the your client from the api
            /// </summary>
            /// <returns>True if successfull else false</returns>
            public bool Disconnect()
            {
                string query = $"/my/disconnect?sessiontoken={HttpUtility.UrlEncode(LoginObject.SessionToken)}";
                var response = _ApiHandler.CallServer<object>(query, LoginObject.ServerEncryptionToken);
                if (response == null)
                    return false;

                LoginObject = null;
                return true;
            }

            #endregion
        }
}
