import { observer } from 'mobx-react';
import React, { Fragment, useEffect } from 'react';
import { TimelineData, TimelineDataScope } from './api/IRinCoreHub';
import * as styles from './App.css';
import {
  SubRequestFailurePayload,
  SubRequestPayload,
  useRinInViewInspectorStore,
  RinInViewInspectorConfig,
} from './Store';

export const App = observer(function App(props: { config: RinInViewInspectorConfig }) {
  const inspectorStore = useRinInViewInspectorStore();
  const data = inspectorStore.data;

  useEffect(() => {
    inspectorStore.ready(props.config);
  }, []);

  return inspectorStore.isReady && data != null ? <MiniInspector /> : <></>;
});

export const MiniInspector = observer(function MiniInspector() {
  const inspectorStore = useRinInViewInspectorStore();
  const position = inspectorStore.position;
  const data = inspectorStore.data;
  const subRequests = inspectorStore.subRequests;
  const pathBase = inspectorStore.config.PathBase;

  return (
    <>
      {data != null && (
        <div className={styles.rinInViewInspector + ' ' + (position === 'Bottom' ? styles.positionBottom : '')}>
          <div className={styles.hud}>
            <label className={styles.summary} htmlFor="rinInViewInspectorDetailIsVisible">
              {data.Timeline.Duration}
              ms
              {subRequests.length > 0 && ` + ${subRequests.length}`}
            </label>
            <input className={styles.isVisible} type="checkbox" id="rinInViewInspectorDetailIsVisible" />
            <div className={styles.detail}>
              <h1 className={styles.detail_path}>{data.Path}</h1>
              <div className={styles.detail_timestamp}>{new Date(data.Timeline.Timestamp).toString()}</div>
              <Timeline timelineData={data.Timeline} />
              <div className={styles.openInRin}>
                <a href={`${pathBase}/Inspect/${data.Id}`} target="_blank">
                  Open in Rin
                </a>
              </div>
              {subRequests.length > 0 && <SubRequests subRequests={subRequests} pathBase={pathBase} />}
            </div>
          </div>
        </div>
      )}
    </>
  );
});

function SubRequests(props: { subRequests: SubRequestPayload[]; pathBase: string }) {
  return (
    <div className={styles.subRequests}>
      <h2>SubRequests</h2>
      <table>
        <tbody>
          {props.subRequests.map((x) =>
            x instanceof SubRequestFailurePayload ? (
              <tr key={`s-${x.Id}`} className={styles.subRequests_subRequestError}>
                <td>{x.Path}</td>
                <td>(Error: {x.ResponseStatusCode})</td>
              </tr>
            ) : (
              <tr key={`s-${x.Id}`}>
                <td>
                  <a href={`${props.pathBase}/Inspect/${x.Id}`} target="_blank">
                    {x.Path}
                  </a>
                </td>
                <td>
                  <a href={`${props.pathBase}/Inspect/${x.Id}`} target="_blank">
                    {x.Timeline.Duration}
                    ms
                  </a>
                </td>
              </tr>
            )
          )}
        </tbody>
      </table>
    </div>
  );
}

function Timeline(props: { timelineData: TimelineDataScope }) {
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
          <tbody>{renderTimelineTree(props.timelineData, 0, new Date(props.timelineData.Timestamp))}</tbody>
        </table>
      </div>
    </>
  );
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
  return timelineScope.Children.filter((x) => x.EventType === 'TimelineScope')
    .map((x) => x as TimelineDataScope)
    .map((x: TimelineDataScope, i: number) => (
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
