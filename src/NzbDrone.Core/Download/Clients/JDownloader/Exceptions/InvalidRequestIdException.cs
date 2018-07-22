using System;

namespace NzbDrone.Core.Download.Clients.JDownloader.Exceptions
{
    public class InvalidRequestIdException: Exception
    {
        public InvalidRequestIdException(string msg) : base(msg)
        { 
        }
    }
}
