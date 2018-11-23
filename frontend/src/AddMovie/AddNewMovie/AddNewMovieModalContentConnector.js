import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { setAddMovieDefault, addMovie } from 'Store/Actions/addMovieActions';
import createDimensionsSelector from 'Store/Selectors/createDimensionsSelector';
import selectSettings from 'Store/Selectors/selectSettings';
import AddNewMovieModalContent from './AddNewMovieModalContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.addMovie,
    createDimensionsSelector(),
    (addMovieState, dimensions) => {
      const {
        isAdding,
        addError,
        defaults
      } = addMovieState;

      const {
        settings,
        validationErrors,
        validationWarnings
      } = selectSettings(defaults, {}, addError);

      return {
        isAdding,
        addError,
        isSmallScreen: dimensions.isSmallScreen,
        validationErrors,
        validationWarnings,
        ...settings
      };
    }
  );
}

const mapDispatchToProps = {
  setAddMovieDefault,
  addMovie
};

class AddNewMovieModalContentConnector extends Component {

  //
  // Listeners

  onInputChange = ({ name, value }) => {
    this.props.setAddMovieDefault({ [name]: value });
  }

  onAddSeriesPress = (searchForMissingEpisodes) => {
    const {
      tvdbId,
      rootFolderPath,
      monitor,
      qualityProfileId,
      tags
    } = this.props;

    this.props.addMovie({
      tvdbId,
      rootFolderPath: rootFolderPath.value,
      monitor: monitor.value,
      qualityProfileId: qualityProfileId.value,
      tags: tags.value,
      searchForMissingEpisodes
    });
  }

  //
  // Render

  render() {
    return (
      <AddNewMovieModalContent
        {...this.props}
        onInputChange={this.onInputChange}
        onAddSeriesPress={this.onAddSeriesPress}
      />
    );
  }
}

AddNewMovieModalContentConnector.propTypes = {
  tvdbId: PropTypes.number.isRequired,
  rootFolderPath: PropTypes.object,
  monitor: PropTypes.object.isRequired,
  qualityProfileId: PropTypes.object,
  tags: PropTypes.object.isRequired,
  onModalClose: PropTypes.func.isRequired,
  setAddMovieDefault: PropTypes.func.isRequired,
  addMovie: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(AddNewMovieModalContentConnector);
