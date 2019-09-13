using System;

namespace NzbDrone.Core.Download.Clients.JDownloader.My.Jdownloader.Api.Exceptions
{
    public class InvalidRequestIdException: Exception
    {
        public InvalidRequestIdException(string msg) : base(msg)
        { 
        }
    }
}
