import _ from 'lodash';
import { createSelector } from 'reselect';
import createAllMoviesSelector from './createAllMoviesSelector';

function createImportMovieItemSelector() {
  return createSelector(
    (state, { id }) => id,
    (state) => state.addMovie,
    (state) => state.importMovie,
    createAllMoviesSelector(),
    (id, addMovie, importMovie, series) => {
      const item = _.find(importMovie.items, { id }) || {};
      const selectedSeries = item && item.selectedSeries;
      const isExistingSeries = !!selectedSeries && _.some(series, { tvdbId: selectedSeries.tvdbId });

      return {
        defaultMonitor: addMovie.defaults.monitor,
        defaultQualityProfileId: addMovie.defaults.qualityProfileId,
        ...item,
        isExistingSeries
      };
    }
  );
}

export default createImportMovieItemSelector;
