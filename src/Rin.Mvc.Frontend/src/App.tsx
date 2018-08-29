import { observer } from 'mobx-react';
import * as React from 'react';
import { Fragment } from 'react';
import { RequestRecordDetailPayload, TimelineData, TimelineDataScope } from './api/IRinCoreHub';
import * as styles from './App.css';
import { rinOnPageInspectorStore } from './Store';

@observer
class App extends React.Component {
  public render() {
    const data = rinOnPageInspectorStore.data;
    const subRequests = rinOnPageInspectorStore.subRequests;
    return (
      <>
        {data != null && (
          <div
            className={
              styles.rinOnViewInspector +
              ' ' +
              (rinOnPageInspectorStore.position === 'Bottom' ? styles.positionBottom : '')
            }
          >
            <div className={styles.hud}>
              <label className={styles.summary} htmlFor="rinOnViewInspectorDetailIsVisible">
                {data.Timeline.Duration}
                ms
                {subRequests.length > 0 && ` + ${subRequests.length}`}
              </label>
              <input className={styles.isVisible} type="checkbox" id="rinOnViewInspectorDetailIsVisible" />
              <div className={styles.detail}>
                <h1 className={styles.detail_path}>{data.Path}</h1>
                <div className={styles.detail_timestamp}>{new Date(data.Timeline.Timestamp).toString()}</div>
                <Timeline timelineData={data.Timeline} />
                <div className={styles.openInRin}>
                  <a href={`${rinOnPageInspectorStore.config.PathBase}/Inspect/${data.Id}`} target="_blank">
                    Open in Rin
                  </a>
                </div>
                {subRequests.length > 0 && <SubRequests subRequests={subRequests} />}
              </div>
            </div>
          </div>
        )}
      </>
    );
  }
}

class SubRequests extends React.Component<{ subRequests: RequestRecordDetailPayload[] }> {
  render() {
    return (
      <div className={styles.subRequests}>
        <h2>SubRequests</h2>
        <table>
          <tbody>
            {this.props.subRequests.map(x => (
              <tr key={`s-${x.Id}`}>
                <td>
                  <a href={`${rinOnPageInspectorStore.config.PathBase}/Inspect/${x.Id}`} target="_blank">
                    {x.Path}
                  </a>
                </td>
                <td>
                  <a href={`${rinOnPageInspectorStore.config.PathBase}/Inspect/${x.Id}`} target="_blank">
                    {x.Timeline.Duration}
                    ms
                  </a>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }
}

class Timeline extends React.Component<{ timelineData: TimelineDataScope }> {
  render() {
    return (
      <>
        <div className={styles.timeline}>
          <table>
            <thead>
              <tr>
                <th>{''}</th>
                <th>Duration (ms)</th>
                <th>From start (ms)</th>
              </tr>
            </thead>
            <tbody>{renderTimelineTree(this.props.timelineData, 0, new Date(this.props.timelineData.Timestamp))}</tbody>
          </table>
        </div>
      </>
    );
  }
}

// https://stackoverflow.com/a/2901298
function numberWithCommas(value: number) {
  return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function prettyPrintTimelineDataName(data: TimelineData) {
  return data.Category === 'Rin.Timeline.AspNetCore.Mvc.View' && data.Name === 'Page'
    ? `View: ${data.Data}`
    : data.Category === 'Rin.Timeline.AspNetCore.Mvc.Action' && data.Name === 'ActionMethod'
      ? `Action: ${data.Data}`
      : data.Name;
}

function renderTimelineTree(timelineScope: TimelineDataScope, depth: number, beginTime: Date): JSX.Element[] {
  return timelineScope.Children.filter(x => x.EventType === 'TimelineScope').map((x: TimelineDataScope, i: number) => (
    <Fragment key={`${depth}_${i}`}>
      <tr>
        <td>
          <span style={{ marginLeft: depth + 'em' }}>{prettyPrintTimelineDataName(x)}</span>
        </td>
        <td>{numberWithCommas(x.Duration)}</td>
        <td>
          <span>+</span>
          {numberWithCommas(new Date(x.Timestamp).valueOf() - beginTime.valueOf())}
        </td>
      </tr>
      {renderTimelineTree(x, depth + 1, beginTime)}
    </Fragment>
  ));
}

export default App;
