import PropTypes from 'prop-types';
import React from 'react';
import { kinds } from 'Helpers/Props';
import Label from 'Components/Label';
import styles from './ImportMovieTitle.css';

function ImportMovieTitle(props) {
  const {
    title,
    year,
    network,
    isExistingSeries
  } = props;

  return (
    <div className={styles.titleContainer}>
      <div className={styles.title}>
        {title}

        {
          !title.contains(year) &&
            <span className={styles.year}>({year})</span>
        }
      </div>

      {
        !!network &&
          <Label>{network}</Label>
      }

      {
        isExistingSeries &&
          <Label
            kind={kinds.WARNING}
          >
            Existing
          </Label>
      }
    </div>
  );
}

ImportMovieTitle.propTypes = {
  title: PropTypes.string.isRequired,
  year: PropTypes.number.isRequired,
  network: PropTypes.string,
  isExistingSeries: PropTypes.bool.isRequired
};

export default ImportMovieTitle;
