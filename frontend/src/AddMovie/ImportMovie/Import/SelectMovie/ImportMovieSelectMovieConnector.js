import _ from 'lodash';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { queueLookupSeries, setImportMovieValue } from 'Store/Actions/importMovieActions';
import createImportMovieItemSelector from 'Store/Selectors/createImportMovieItemSelector';
import ImportMovieSelectMovie from './ImportMovieSelectMovie';

function createMapStateToProps() {
  return createSelector(
    (state) => state.importMovie.isLookingUpSeries,
    createImportMovieItemSelector(),
    (isLookingUpSeries, item) => {
      return {
        isLookingUpSeries,
        ...item
      };
    }
  );
}

const mapDispatchToProps = {
  queueLookupSeries,
  setImportMovieValue
};

class ImportMovieSelectMovieConnector extends Component {

  //
  // Listeners

  onSearchInputChange = (term) => {
    this.props.queueLookupSeries({
      name: this.props.id,
      term,
      topOfQueue: true
    });
  }

  onSeriesSelect = (tvdbId) => {
    const {
      id,
      items
    } = this.props;

    this.props.setImportMovieValue({
      id,
      selectedSeries: _.find(items, { tvdbId })
    });
  }

  //
  // Render

  render() {
    return (
      <ImportMovieSelectMovie
        {...this.props}
        onSearchInputChange={this.onSearchInputChange}
        onSeriesSelect={this.onSeriesSelect}
      />
    );
  }
}

ImportMovieSelectMovieConnector.propTypes = {
  id: PropTypes.string.isRequired,
  items: PropTypes.arrayOf(PropTypes.object),
  selectedSeries: PropTypes.object,
  isSelected: PropTypes.bool,
  queueLookupSeries: PropTypes.func.isRequired,
  setImportMovieValue: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(ImportMovieSelectMovieConnector);
