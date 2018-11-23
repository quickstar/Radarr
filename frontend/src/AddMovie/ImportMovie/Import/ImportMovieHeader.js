import PropTypes from 'prop-types';
import React from 'react';
import { icons, tooltipPositions } from 'Helpers/Props';
import Icon from 'Components/Icon';
import Popover from 'Components/Tooltip/Popover';
import VirtualTableHeader from 'Components/Table/VirtualTableHeader';
import VirtualTableHeaderCell from 'Components/Table/VirtualTableHeaderCell';
import VirtualTableSelectAllHeaderCell from 'Components/Table/VirtualTableSelectAllHeaderCell';
import SeriesMonitoringOptionsPopoverContent from 'AddMovie/SeriesMonitoringOptionsPopoverContent';
import styles from './ImportMovieHeader.css';

function ImportMovieHeader(props) {
  const {
    allSelected,
    allUnselected,
    onSelectAllChange
  } = props;

  return (
    <VirtualTableHeader>
      <VirtualTableSelectAllHeaderCell
        allSelected={allSelected}
        allUnselected={allUnselected}
        onSelectAllChange={onSelectAllChange}
      />

      <VirtualTableHeaderCell
        className={styles.folder}
        name="folder"
      >
        Folder
      </VirtualTableHeaderCell>

      <VirtualTableHeaderCell
        className={styles.monitor}
        name="monitor"
      >
        Monitor

        <Popover
          anchor={
            <Icon
              className={styles.detailsIcon}
              name={icons.INFO}
            />
          }
          title="Monitoring Options"
          body={<SeriesMonitoringOptionsPopoverContent />}
          position={tooltipPositions.RIGHT}
        />
      </VirtualTableHeaderCell>

      <VirtualTableHeaderCell
        className={styles.qualityProfile}
        name="qualityProfileId"
      >
        Quality Profile
      </VirtualTableHeaderCell>

      <VirtualTableHeaderCell
        className={styles.movie}
        name="movie"
      >
        Movie
      </VirtualTableHeaderCell>
    </VirtualTableHeader>
  );
}

ImportMovieHeader.propTypes = {
  allSelected: PropTypes.bool.isRequired,
  allUnselected: PropTypes.bool.isRequired,
  onSelectAllChange: PropTypes.func.isRequired
};

export default ImportMovieHeader;
