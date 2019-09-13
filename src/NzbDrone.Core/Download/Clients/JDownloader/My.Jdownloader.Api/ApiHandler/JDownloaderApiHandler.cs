using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Newtonsoft.Json;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Exceptions;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Action;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler
{
    internal class JDownloaderApiHandler
    {
        private string _apiUrl = "http://api.jdownloader.org";
        private int _requestId = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        public T CallAction<T>(DeviceObject device, string action, object param, LoginObject loginObject,
                               bool decryptResponse = false)
        {
            if (device == null)
            {
                throw new ArgumentNullException("The device can't be null.");
            }

            if (string.IsNullOrEmpty(device.Id))
            {
                throw new ArgumentException(
                                            "The id of the device is empty. Please call again the GetDevices Method and try again.");
            }

            string query = $"/t_{HttpUtility.UrlEncode(loginObject.SessionToken)}_{HttpUtility.UrlEncode(device.Id)}{action}";
            CallActionObject callActionObject = new CallActionObject
            {
                ApiVer = 1,
                Params = param,
                RequestId = GetUniqueRid(),
                Url = action
            };

            string url = _apiUrl + query;
            string json = JsonConvert.SerializeObject(callActionObject);
            json = Encrypt(json, loginObject.DeviceEncryptionToken);
            string response = PostMethod(url, json, loginObject.DeviceEncryptionToken);

            if ((response == null) || (!response.Contains(callActionObject.RequestId.ToString()) && !response.Contains("\"rid\" : -1")))
            {
                if (decryptResponse)
                {
                    string tmp = Decrypt(response, loginObject.DeviceEncryptionToken);
                    return (T)JsonConvert.DeserializeObject(tmp, typeof(T));
                }

                throw new InvalidRequestIdException("The 'RequestId' differs from the 'Requestid' from the query.");
            }

            return (T)JsonConvert.DeserializeObject(response, typeof(T));
        }

        public T CallServer<T>(string query, byte[] key, string param = "")
        {
            string rid;
            if (!string.IsNullOrEmpty(param))
            {
                if (key != null)
                {
                    param = Encrypt(param, key);
                }

                rid = _requestId.ToString();
            }
            else
            {
                rid = GetUniqueRid().ToString();
            }

            if (query.Contains("?"))
            {
                query += "&";
            }
            else
            {
                query += "?";
            }

            query += "rid=" + rid;
            string signature = GetSignature(query, key);
            query += "&signature=" + signature;

            string url = _apiUrl + query;
            if (!string.IsNullOrWhiteSpace(param))
            {
                param = string.Empty;
            }

            string response = PostMethod(url, param, key);
            if (response == null)
            {
                return default;
            }

            return (T)JsonConvert.DeserializeObject(response, typeof(T));
        }

        private int GetUniqueRid()
        {
            return _requestId++;
        }

        private string PostMethod(string url, string body = "", byte[] ivKey = null)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = 2000;
                if (!string.IsNullOrEmpty(body))
                {
                    request.Method = "POST";
                    request.ContentType = "application/aesjson-jd; charset=utf-8";
                    byte[] content = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = content.Length;
                    var postStream = request.GetRequestStream();
                    postStream.Write(content, 0, content.Length);
                    postStream.Close();
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        response.Close();
                        return null;
                    }

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            string result = null;
                            using (var myStreamReader = new StreamReader(responseStream))
                            {
                                result = myStreamReader.ReadToEnd();
                            }

                            response.Close();
                            if (ivKey != null)
                            {
                                result = Decrypt(result, ivKey);
                            }

                            return result;
                        }
                    }
                }
                catch (WebException exception)
                {
                    if (exception.Response != null)
                    {
                        Debug.WriteLine(exception.Message);
                        if (exception.Response != null)
                        {
                            Stream respsone = exception.Response.GetResponseStream();
                            if (respsone != null)
                            {
                                string resp = new StreamReader(respsone).ReadToEnd();
                                Debug.WriteLine(resp);
                            }
                        }

                        return null;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SetApiUrl(string newApiUrl)
        {
            _apiUrl = newApiUrl;
        }

        #region "Encrypt, Decrypt and Signature"

        private string GetSignature(string data, byte[] key)
        {
            if (key == null)
            {
                throw new Exception(
                                    "The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
            }

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var hmacsha256 = new HMACSHA256(key);
            hmacsha256.ComputeHash(dataBytes);
            var hash = hmacsha256.Hash;
            string binaryString = hash.Aggregate("", (current, t) => current + t.ToString("X2"));
            return binaryString.ToLower();
        }

        private string Encrypt(string data, byte[] ivKey)
        {
            if (ivKey == null)
            {
                throw new Exception("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
            }

            var iv = new byte[16];
            var key = new byte[16];
            for (int i = 0; i < 32; i++)
            {
                if (i < 16)
                {
                    iv[i] = ivKey[i];
                }
                else
                {
                    key[i - 16] = ivKey[i];
                }
            }

            var rj = new RijndaelManaged
            {
                Key = key,
                IV = iv,
                Mode = CipherMode.CBC,
                BlockSize = 128
            };
            ICryptoTransform encryptor = rj.CreateEncryptor();
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            byte[] encrypted = msEncrypt.ToArray();
            return Convert.ToBase64String(encrypted);
        }

        private string Decrypt(string data, byte[] ivKey)
        {
            if (ivKey == null)
            {
                throw new Exception("The ivKey is null. Please check your login informations. If it's still null the server may has disconnected you.");
            }

            var iv = new byte[16];
            var key = new byte[16];
            for (int i = 0; i < 32; i++)
            {
                if (i < 16)
                {
                    iv[i] = ivKey[i];
                }
                else
                {
                    key[i - 16] = ivKey[i];
                }
            }

            try
            {
                byte[] cypher = Convert.FromBase64String(data);
                using (var rj = new RijndaelManaged
                {
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    IV = iv,
                    Key = key
                })
                {
                    using (var ms = new MemoryStream(cypher))
                    {
                        string result;
                        using (var cs = new CryptoStream(ms, rj.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                result = sr.ReadToEnd();
                            }
                        }

                        return result;
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        #endregion
    }
}
