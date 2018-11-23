import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { icons, kinds, inputTypes, tooltipPositions } from 'Helpers/Props';
import Icon from 'Components/Icon';
import SpinnerButton from 'Components/Link/SpinnerButton';
import Form from 'Components/Form/Form';
import FormGroup from 'Components/Form/FormGroup';
import FormLabel from 'Components/Form/FormLabel';
import FormInputGroup from 'Components/Form/FormInputGroup';
import CheckInput from 'Components/Form/CheckInput';
import ModalContent from 'Components/Modal/ModalContent';
import ModalHeader from 'Components/Modal/ModalHeader';
import ModalBody from 'Components/Modal/ModalBody';
import ModalFooter from 'Components/Modal/ModalFooter';
import Popover from 'Components/Tooltip/Popover';
import MoviePoster from 'Movie/MoviePoster';
import SeriesMonitoringOptionsPopoverContent from 'AddMovie/SeriesMonitoringOptionsPopoverContent';
import styles from './AddNewMovieModalContent.css';

class AddNewMovieModalContent extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      searchForMissingEpisodes: false
    };
  }

  //
  // Listeners

  onSearchForMissingEpisodesChange = ({ value }) => {
    this.setState({ searchForMissingEpisodes: value });
  }

  onQualityProfileIdChange = ({ value }) => {
    this.props.onInputChange({ name: 'qualityProfileId', value: parseInt(value) });
  }

  onAddSeriesPress = () => {
    this.props.onAddSeriesPress(this.state.searchForMissingEpisodes);
  }

  //
  // Render

  render() {
    const {
      title,
      year,
      overview,
      images,
      isAdding,
      rootFolderPath,
      monitor,
      qualityProfileId,
      tags,
      isSmallScreen,
      onModalClose,
      onInputChange
    } = this.props;

    return (
      <ModalContent onModalClose={onModalClose}>
        <ModalHeader>
          {title}

          {
            !title.contains(year) && !!year &&
              <span className={styles.year}>({year})</span>
          }
        </ModalHeader>

        <ModalBody>
          <div className={styles.container}>
            {
              !isSmallScreen &&
                <div className={styles.poster}>
                  <MoviePoster
                    className={styles.poster}
                    images={images}
                    size={250}
                  />
                </div>
            }

            <div className={styles.info}>
              <div className={styles.overview}>
                {overview}
              </div>

              <Form>
                <FormGroup>
                  <FormLabel>Root Folder</FormLabel>

                  <FormInputGroup
                    type={inputTypes.ROOT_FOLDER_SELECT}
                    name="rootFolderPath"
                    onChange={onInputChange}
                    {...rootFolderPath}
                  />
                </FormGroup>

                <FormGroup>
                  <FormLabel>
                    Monitor

                    <Popover
                      anchor={
                        <Icon
                          className={styles.labelIcon}
                          name={icons.INFO}
                        />
                      }
                      title="Monitoring Options"
                      body={<SeriesMonitoringOptionsPopoverContent />}
                      position={tooltipPositions.RIGHT}
                    />
                  </FormLabel>

                  <FormInputGroup
                    type={inputTypes.MONITOR_EPISODES_SELECT}
                    name="monitor"
                    onChange={onInputChange}
                    {...monitor}
                  />
                </FormGroup>

                <FormGroup>
                  <FormLabel>Quality Profile</FormLabel>

                  <FormInputGroup
                    type={inputTypes.QUALITY_PROFILE_SELECT}
                    name="qualityProfileId"
                    onChange={this.onQualityProfileIdChange}
                    {...qualityProfileId}
                  />
                </FormGroup>

                <FormGroup>
                  <FormLabel>Tags</FormLabel>

                  <FormInputGroup
                    type={inputTypes.TAG}
                    name="tags"
                    onChange={onInputChange}
                    {...tags}
                  />
                </FormGroup>
              </Form>
            </div>
          </div>
        </ModalBody>

        <ModalFooter className={styles.modalFooter}>
          <label className={styles.searchForMissingEpisodesLabelContainer}>
            <span className={styles.searchForMissingEpisodesLabel}>
              Start search for missing movie
            </span>

            <CheckInput
              containerClassName={styles.searchForMissingEpisodesContainer}
              className={styles.searchForMissingEpisodesInput}
              name="searchForMissingEpisodes"
              value={this.state.searchForMissingEpisodes}
              onChange={this.onSearchForMissingEpisodesChange}
            />
          </label>

          <SpinnerButton
            className={styles.addButton}
            kind={kinds.SUCCESS}
            isSpinning={isAdding}
            onPress={this.onAddSeriesPress}
          >
            Add {title}
          </SpinnerButton>
        </ModalFooter>
      </ModalContent>
    );
  }
}

AddNewMovieModalContent.propTypes = {
  title: PropTypes.string.isRequired,
  year: PropTypes.number.isRequired,
  overview: PropTypes.string,
  images: PropTypes.arrayOf(PropTypes.object).isRequired,
  isAdding: PropTypes.bool.isRequired,
  addError: PropTypes.object,
  rootFolderPath: PropTypes.object,
  monitor: PropTypes.object.isRequired,
  qualityProfileId: PropTypes.object,
  tags: PropTypes.object.isRequired,
  isSmallScreen: PropTypes.bool.isRequired,
  onModalClose: PropTypes.func.isRequired,
  onInputChange: PropTypes.func.isRequired,
  onAddSeriesPress: PropTypes.func.isRequired
};

export default AddNewMovieModalContent;
