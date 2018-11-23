import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { queueLookupSeries, setImportMovieValue } from 'Store/Actions/importMovieActions';
import createAllMoviesSelector from 'Store/Selectors/createAllMoviesSelector';
import ImportMovieTable from './ImportMovieTable';

function createMapStateToProps() {
  return createSelector(
    (state) => state.addMovie,
    (state) => state.importMovie,
    (state) => state.app.dimensions,
    createAllMoviesSelector(),
    (addMovie, importMovie, dimensions, allSeries) => {
      return {
        defaultMonitor: addMovie.defaults.monitor,
        defaultQualityProfileId: addMovie.defaults.qualityProfileId,
        items: importMovie.items,
        isSmallScreen: dimensions.isSmallScreen,
        allSeries
      };
    }
  );
}

function createMapDispatchToProps(dispatch, props) {
  return {
    onSeriesLookup(name, path) {
      dispatch(queueLookupSeries({
        name,
        path,
        term: name
      }));
    },

    onSetImportMovieValue(values) {
      dispatch(setImportMovieValue(values));
    }
  };
}

export default connect(createMapStateToProps, createMapDispatchToProps)(ImportMovieTable);
