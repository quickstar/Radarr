
function getNewSeries(series, payload) {
  const {
    rootFolderPath,
    monitor,
    qualityProfileId,
    tags,
    searchForMissingEpisodes = false
  } = payload;

  const addOptions = {
    monitor,
    searchForMissingEpisodes
  };

  series.addOptions = addOptions;
  series.monitored = true;
  series.qualityProfileId = qualityProfileId;
  series.rootFolderPath = rootFolderPath;
  series.tags = tags;

  return series;
}

export default getNewSeries;
