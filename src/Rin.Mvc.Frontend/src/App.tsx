import { observer } from 'mobx-react';
import * as React from 'react';
import { Fragment } from 'react';
import { TimelineData, TimelineDataScope } from './api/IRinCoreHub';
import './App.css';
import { rinOnPageInspectorStore } from './Store';

@observer
class App extends React.Component {
  public render() {
    const data = rinOnPageInspectorStore.data;

    return (
      <>
        {data != null && (
          <div className="rinOnViewInspector">
            <div className="hud">
              <label className="summary" htmlFor="rinOnViewInspectorDetailIsVisible">
                {data.Timeline.Duration}
                ms
              </label>
              <input className="isVisible" type="checkbox" id="rinOnViewInspectorDetailIsVisible" />
              <div className="detail">
                <div className="detail_path">{data.Path}</div>
                <div className="detail_timestamp">{new Date(data.Timeline.Timestamp).toString()}</div>
                <Timeline timelineData={data.Timeline} />
              </div>
            </div>
          </div>
        )}
      </>
    );
  }
}

class Timeline extends React.Component<{ timelineData: TimelineDataScope }> {
  render() {
    return (
      <>
        <div className="timeline">
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
