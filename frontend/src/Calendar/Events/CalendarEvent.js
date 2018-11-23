import moment from 'moment';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import classNames from 'classnames';
import { icons, kinds } from 'Helpers/Props';
import formatTime from 'Utilities/Date/formatTime';
import getStatusStyle from 'Calendar/getStatusStyle';
import Icon from 'Components/Icon';
import Link from 'Components/Link/Link';
import CalendarEventQueueDetails from './CalendarEventQueueDetails';
import styles from './CalendarEvent.css';

class CalendarEvent extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isDetailsModalOpen: false
    };
  }

  //
  // Listeners

  onPress = () => {
    this.setState({ isDetailsModalOpen: true }, () => {
      this.props.onEventModalOpenToggle(true);
    });
  }

  onDetailsModalClose = () => {
    this.setState({ isDetailsModalOpen: false }, () => {
      this.props.onEventModalOpenToggle(false);
    });
  }

  //
  // Render

  render() {
    const {
      // id,
      series,
      episodeFile,
      // title,
      seasonNumber,
      episodeNumber,
      airDateUtc,
      monitored,
      hasFile,
      grabbed,
      queueItem,
      showFinaleIcon,
      showSpecialIcon,
      showCutoffUnmetIcon,
      timeFormat,
      colorImpairedMode
    } = this.props;

    if (!series) {
      return null;
    }

    const startTime = moment(airDateUtc);
    const endTime = moment(airDateUtc).add(series.runtime, 'minutes');
    const isDownloading = !!(queueItem || grabbed);
    const isMonitored = series.monitored && monitored;
    const statusStyle = getStatusStyle(hasFile, isDownloading, startTime, endTime, isMonitored);
    const season = series.seasons.find((s) => s.seasonNumber === seasonNumber);
    const seasonStatistics = season.statistics || {};

    return (
      <div>
        <Link
          className={classNames(
            styles.event,
            styles[statusStyle],
            colorImpairedMode && 'colorImpaired'
          )}
          component="div"
          onPress={this.onPress}
        >
          <div className={styles.info}>
            <div className={styles.seriesTitle}>
              {series.title}
            </div>

            {
              !!queueItem &&
                <span className={styles.statusIcon}>
                  <CalendarEventQueueDetails
                    {...queueItem}
                  />
                </span>
            }

            {
              !queueItem && grabbed &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.DOWNLOADING}
                  title="Episode is downloading"
                />
            }

            {
              showCutoffUnmetIcon &&
              !!episodeFile &&
              episodeFile.qualityCutoffNotMet &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.MOVIE_FILE}
                  kind={kinds.WARNING}
                  title="Quality cutoff has not been met"
                />
            }

            {
              showCutoffUnmetIcon &&
              !!episodeFile &&
              episodeFile.languageCutoffNotMet &&
              !episodeFile.qualityCutoffNotMet &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.MOVIE_FILE}
                  kind={kinds.WARNING}
                  title="Language cutoff has not been met"
                />
            }

            {
              episodeNumber === 1 && seasonNumber > 0 &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.INFO}
                  kind={kinds.INFO}
                  title={seasonNumber === 1 ? 'Series premiere' : 'Season premiere'}
                />
            }

            {
              showFinaleIcon &&
              episodeNumber !== 1 &&
              seasonNumber > 0 &&
              episodeNumber === seasonStatistics.totalEpisodeCount &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.INFO}
                  kind={kinds.WARNING}
                  title={series.status === 'ended' ? 'Series finale' : 'Season finale'}
                />
            }

            {
              showSpecialIcon &&
              (episodeNumber === 0 || seasonNumber === 0) &&
                <Icon
                  className={styles.statusIcon}
                  name={icons.INFO}
                  kind={kinds.PINK}
                  title="Special"
                />
            }
          </div>

          <div>
            {formatTime(airDateUtc, timeFormat)} - {formatTime(endTime.toISOString(), timeFormat, { includeMinuteZero: true })}
          </div>
        </Link>

      </div>
    );
  }
}

CalendarEvent.propTypes = {
  id: PropTypes.number.isRequired,
  series: PropTypes.object.isRequired,
  episodeFile: PropTypes.object,
  title: PropTypes.string.isRequired,
  seasonNumber: PropTypes.number.isRequired,
  episodeNumber: PropTypes.number.isRequired,
  absoluteEpisodeNumber: PropTypes.number,
  airDateUtc: PropTypes.string.isRequired,
  monitored: PropTypes.bool.isRequired,
  hasFile: PropTypes.bool.isRequired,
  grabbed: PropTypes.bool,
  queueItem: PropTypes.object,
  showEpisodeInformation: PropTypes.bool.isRequired,
  showFinaleIcon: PropTypes.bool.isRequired,
  showSpecialIcon: PropTypes.bool.isRequired,
  showCutoffUnmetIcon: PropTypes.bool.isRequired,
  timeFormat: PropTypes.string.isRequired,
  colorImpairedMode: PropTypes.bool.isRequired,
  onEventModalOpenToggle: PropTypes.func.isRequired
};

export default CalendarEvent;
