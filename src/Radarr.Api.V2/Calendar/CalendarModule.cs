using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Radarr.Api.V2.Movies;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Movies;
using NzbDrone.Core.Movies.Events;
using NzbDrone.Core.Validation.Paths;
using NzbDrone.Core.Validation;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.SignalR;
using Radarr.Http;

namespace Radarr.Api.V2.Calendar
{
    public class CalendarModule : MovieModule
    {
        public CalendarModule(IBroadcastSignalRMessage signalR,
                            IMovieService moviesService,
                            IMapCoversToLocal coverMapper,
                            RootFolderValidator rootFolderValidator,
                            MoviePathValidator moviesPathValidator,
                            MovieExistsValidator moviesExistsValidator,
                            MovieAncestorValidator moviesAncestorValidator,
                            ProfileExistsValidator profileExistsValidator)
            : base(signalR, moviesService, coverMapper, rootFolderValidator, moviesPathValidator, moviesExistsValidator, moviesAncestorValidator, profileExistsValidator)
        {
            GetResourceAll = GetCalendar;
        }

        private List<MovieResource> GetCalendar()
        {
            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(2);
            var includeUnmonitored = false;

            var queryStart = Request.Query.Start;
            var queryEnd = Request.Query.End;
            var queryIncludeUnmonitored = Request.Query.Unmonitored;

            if (queryStart.HasValue) start = DateTime.Parse(queryStart.Value);
            if (queryEnd.HasValue) end = DateTime.Parse(queryEnd.Value);
            if (queryIncludeUnmonitored.HasValue) includeUnmonitored = Convert.ToBoolean(queryIncludeUnmonitored.Value);

            var resources = _moviesService.GetMoviesBetweenDates(start, end, includeUnmonitored).Select(MapToResource);

            return resources.OrderBy(e => e.InCinemas).ToList();
        }
    }
}
