import PropTypes from 'prop-types';
import React from 'react';
import { inputTypes } from 'Helpers/Props';
import FormInputGroup from 'Components/Form/FormInputGroup';
import VirtualTableRow from 'Components/Table/VirtualTableRow';
import VirtualTableRowCell from 'Components/Table/Cells/VirtualTableRowCell';
import VirtualTableSelectCell from 'Components/Table/Cells/VirtualTableSelectCell';
import ImportMovieSelectMovieConnector from './SelectMovie/ImportMovieSelectMovieConnector';
import styles from './ImportMovieRow.css';

function ImportMovieRow(props) {
  const {
    style,
    id,
    monitor,
    qualityProfileId,
    selectedSeries,
    isExistingSeries,
    isSelected,
    onSelectedChange,
    onInputChange
  } = props;

  return (
    <VirtualTableRow style={style}>
      <VirtualTableSelectCell
        inputClassName={styles.selectInput}
        id={id}
        isSelected={isSelected}
        isDisabled={!selectedSeries || isExistingSeries}
        onSelectedChange={onSelectedChange}
      />

      <VirtualTableRowCell className={styles.folder}>
        {id}
      </VirtualTableRowCell>

      <VirtualTableRowCell className={styles.monitor}>
        <FormInputGroup
          type={inputTypes.MONITOR_EPISODES_SELECT}
          name="monitor"
          value={monitor}
          onChange={onInputChange}
        />
      </VirtualTableRowCell>

      <VirtualTableRowCell className={styles.qualityProfile}>
        <FormInputGroup
          type={inputTypes.QUALITY_PROFILE_SELECT}
          name="qualityProfileId"
          value={qualityProfileId}
          onChange={onInputChange}
        />
      </VirtualTableRowCell>

      <VirtualTableRowCell className={styles.series}>
        <ImportMovieSelectMovieConnector
          id={id}
          isExistingSeries={isExistingSeries}
        />
      </VirtualTableRowCell>
    </VirtualTableRow>
  );
}

ImportMovieRow.propTypes = {
  style: PropTypes.object.isRequired,
  id: PropTypes.string.isRequired,
  monitor: PropTypes.string.isRequired,
  qualityProfileId: PropTypes.number.isRequired,
  selectedSeries: PropTypes.object,
  isExistingSeries: PropTypes.bool.isRequired,
  items: PropTypes.arrayOf(PropTypes.object).isRequired,
  queued: PropTypes.bool.isRequired,
  isSelected: PropTypes.bool,
  onSelectedChange: PropTypes.func.isRequired,
  onInputChange: PropTypes.func.isRequired
};

ImportMovieRow.defaultsProps = {
  items: []
};

export default ImportMovieRow;
