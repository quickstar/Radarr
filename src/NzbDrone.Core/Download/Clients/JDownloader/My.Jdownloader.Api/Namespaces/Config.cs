using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.ApiHandler;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Config;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Devices;
using NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Models.Login;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Namespaces
{
    public class Config : Base
    {
        internal Config(JDownloaderApiHandler apiHandler, DeviceObject device, LoginObject loginObject)
            : base(apiHandler, device, loginObject)
        { }

        /// <summary>
        /// Gets the value of the given interface
        /// </summary>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="storage">The storage name.</param>
        /// <param name="key">The key name.</param>
        /// <returns>The value of the given interface.</returns>
        public object Get(string interfaceName, string storage, string key)
        {
            var param = new[] { interfaceName, storage, key };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/get", param, LoginObject, true);

            return response?.Data;
        }

        /// <summary>
        /// Gets the default value of the given interface
        /// </summary>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="storage">The storage name.</param>
        /// <param name="key">The key name.</param>
        /// <returns>The default value of the given interface.</returns>
        public object GetDefault(string interfaceName, string storage, string key)
        {
            var param = new[] { interfaceName, storage, key };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/get", param, LoginObject, true);

            return response?.Data;
        }

        /// <summary>
        /// Lists all available config entries.
        /// </summary>
        /// <returns>An enumerable with all available config entries.</returns>
        public IEnumerable<AdvancedConfigApiEntry> List()
        {
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/list", null, LoginObject, true);
            var tmp = ((JArray)response.Data);

            return tmp?.ToObject<IEnumerable<AdvancedConfigApiEntry>>();
        }

        /// <summary>
        /// Lists all available config entries based on the pattern.
        /// </summary>
        /// <param name="pattern">A pattern as a regex string.</param>
        /// <param name="returnDescription">True if you want the description.</param>
        /// <param name="returnValues">True if you want the values.</param>
        /// <param name="returnDefaultValues">True if you want the default values.</param>
        /// <param name="returnEnumInfo">True if you want the enum infos</param>
        /// <returns>An enumerable with all available config entries based on the regex pattern.</returns>
        public IEnumerable<AdvancedConfigApiEntry> List(string pattern, bool returnDescription, bool returnValues, bool returnDefaultValues, bool returnEnumInfo)
        {
            var param = new object[] { pattern, returnDescription, returnValues, returnDefaultValues, returnEnumInfo };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/list",
                                                                      param, LoginObject, true);
            var tmp = ((JArray)response.Data);

            return tmp?.ToObject<IEnumerable<AdvancedConfigApiEntry>>();
        }

        /// <summary>
        /// Lists all possible enum values.
        /// </summary>
        /// <param name="type">The type of the enum.</param>
        /// <returns>An enumerable with all possible enum values.</returns>
        public IEnumerable<EnumOption> ListEnum(string type)
        {
            var param = new[] { type };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/listEnum", param, LoginObject, true);
            var tmp = ((JArray)response.Data);

            return tmp?.ToObject<IEnumerable<EnumOption>>();
        }

        /// <summary>
        /// Lists all available config entries based on the query object.
        /// </summary>
        /// <param name="query">The query object to filter the return.</param>
        /// <returns>An enumerable with all available config entries based on the query object.</returns>
        public IEnumerable<AdvancedConfigApiEntry> Query(AdvancedConfigQuery query)
        {
            var param = new[] { query };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/query", null, LoginObject, true);
            var tmp = ((JArray)response.Data);

            return tmp?.ToObject<IEnumerable<AdvancedConfigApiEntry>>();
        }

        /// <summary>
        /// Resets the interface by the key to its default value.
        /// </summary>
        /// <param name="interfaceName">The name of the interface.</param>
        /// <param name="storage">The storage of the interface.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if successful.</returns>
        public bool Reset(string interfaceName, string storage, string key)
        {
            var param = new[] { interfaceName, storage, key };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/reset", param, LoginObject, true);

            return (response?.Data != null) && (bool)response?.Data;
        }

        /// <summary>
        /// Sets the value of the interface by key.
        /// </summary>
        /// <param name="interfaceName">The name of the interface.</param>
        /// <param name="storage">The storage of the interface.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The new value.</param>
        /// <returns>True if successful.</returns>
        public bool Set(string interfaceName, string storage, string key, object value)
        {
            var param = new[] { interfaceName, storage, key, value };
            var response = ApiHandler.CallAction<DefaultReturnObject>(Device, "/config/set", param, LoginObject, true);

            return (response?.Data != null) && (bool)response?.Data;
        }
    }
}
