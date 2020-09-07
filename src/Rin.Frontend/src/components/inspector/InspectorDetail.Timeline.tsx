import { Callout, DirectionalHint, FontClassNames, Icon, Toggle } from 'office-ui-fabric-react';
import React, { useRef } from 'react';
import { TimelineData, TimelineDataScope, TimelineEventCategory, TimelineDataStamp } from '../../api/IRinCoreHub';
import { calculateChildrenDuration } from '../../domain/RequestRecord';
import * as styles from './InspectorDetail.Timeline.css';

export interface InspectorDetailTimelineViewProps {
  data: TimelineDataScope;
  isCalloutVisible: boolean;
  isTraceVisibleInTimeline: boolean;
  showCallout: (data: TimelineData, target: HTMLElement) => void;
  dismissCallout: () => void;
  calloutTimelineData: TimelineData | null;
  calloutTarget: HTMLElement;
  toggleTraceVisibility: (visible: boolean) => void;
}

export function InspectorDetailTimelineView(props: InspectorDetailTimelineViewProps) {
  const filter = props.isTraceVisibleInTimeline
    ? (_: TimelineData) => true
    : (data: TimelineData) => data.Category !== TimelineEventCategory.Trace;

  return (
    <div className={styles.inspectorDetailTimelineView}>
      <div className={styles.inspectorDetailTimelineView_Commands}>
        <Toggle
          inlineLabel={true}
          label="Show Traces"
          checked={props.isTraceVisibleInTimeline}
          onChanged={props.toggleTraceVisibility}
        />
      </div>
      <Timeline {...props} filter={filter} />
    </div>
  );
}

export interface TimelineProps {
  data: TimelineDataScope;
  isCalloutVisible: boolean;
  showCallout: (data: TimelineData, target: HTMLElement) => void;
  dismissCallout: () => void;
  calloutTimelineData: TimelineData | null;
  calloutTarget: HTMLElement;
  filter: (data: TimelineData) => boolean;
}

export function Timeline(props: TimelineProps) {
  const beginTime = new Date(props.data.Timestamp).valueOf();
  const endTime = maxEndTime(props.data);
  const totalDuration = endTime - beginTime;

  const columns = [
    <div key="Column-Event" className={styles.timeline_headerColumn}>
      <span>Event</span>
    </div>
  ];
  for (let i = 0; i < 10; i++) {
    const t = (totalDuration / 10) * i;
    columns.push(
      <div key={'Column-' + i} className={styles.timeline_headerColumn}>
        <span>
          {t.toFixed(1)}
          ms
        </span>
      </div>
    );
  }

  function renderCallout(data: TimelineDataScope | TimelineDataStamp) {
    const hasChildren = data.EventType === 'TimelineScope' && data.Children.length > 0;
    const childrenDuration = data.EventType === 'TimelineScope' ? calculateChildrenDuration(data) : 0;
    // TODO: a child scope may be longer than the scope. (e.g. ActionResult < Transferring)
    const selfDuration = data.EventType === 'TimelineScope' ? Math.max(0, data.Duration - childrenDuration) : 0;

    return (
      <>
        <Callout
          gapSpace={0}
          target={props.calloutTarget}
          setInitialFocus={true}
          hidden={!props.isCalloutVisible}
          onDismiss={props.dismissCallout}
          directionalHint={DirectionalHint.bottomCenter}
        >
          <div className={styles.timelineCalloutContent}>
            <h2 className={FontClassNames.medium}>
              {data.Category.replace(/^Rin\.Timeline\./, '')}: {data.Name}
            </h2>
            {data.Data && <pre className={styles.timelineCalloutContent_data}>{data.Data}</pre>}
            {data.EventType === 'TimelineScope' && (
              <div>
                <Icon iconName="Timer" />{' '}
                {hasChildren
                  ? `${data.Duration}ms (Self ${selfDuration}ms + Children ${childrenDuration}ms)`
                  : `${data.Duration}ms`}
              </div>
            )}
          </div>
        </Callout>
      </>
    );
  }

  return (
    <div className={styles.timeline}>
      <div className={styles.timeline_header}>{columns}</div>
      <div className={styles.timeline_spans}>
        <TimelineSpans
          data={props.data}
          totalDuration={totalDuration}
          filter={props.filter}
          onTimelineSpanClick={props.showCallout}
        />
      </div>
      {props.isCalloutVisible && props.calloutTimelineData != null && renderCallout(props.calloutTimelineData)}
    </div>
  );
}

function maxEndTime(data: TimelineDataScope): number {
  const childrenEndTimeMax = data.Children.filter(x => x.EventType === 'TimelineScope').reduce(
    (r, v) => Math.max(maxEndTime(v as TimelineDataScope), r),
    0
  );

  const endTime = new Date(data.Timestamp).valueOf() + data.Duration;
  return Math.max(childrenEndTimeMax, endTime);
}

interface TimelineSpansProps {
  data: TimelineDataScope;
  totalDuration: number;
  onTimelineSpanClick: (data: TimelineData, target: HTMLElement) => void;
  filter: (data: TimelineData) => boolean;
}

function TimelineSpans(props: TimelineSpansProps) {
  const originDate = new Date(props.data.Timestamp);
  return (
    <>
      {props.data.Children.filter(props.filter).map((x, i) => (
        <TimelineSpan
          key={'timelineSpan-' + i}
          data={x}
          totalDuration={props.totalDuration}
          originDate={originDate}
          onTimelineSpanClick={props.onTimelineSpanClick}
          filter={props.filter}
        />
      ))}
    </>
  );
}

interface TimelineSpanProps {
  data: TimelineData;
  totalDuration: number;
  originDate: Date;
  onTimelineSpanClick: (data: TimelineData, target: HTMLElement) => void;
  filter: (data: TimelineData) => boolean;
}

function TimelineSpan(props: TimelineSpanProps) {
  const elapsedMilliSecFromOrigin = new Date(props.data.Timestamp).valueOf() - props.originDate.valueOf();
  const width =
    props.data.EventType === 'TimelineScope' ? (props.data.Duration / props.totalDuration) * 100 : 0;
  const left = 100 - ((props.totalDuration - elapsedMilliSecFromOrigin) / props.totalDuration) * 100;
  const label = asLabel(props.data);

  const timelineSpanItemRef = useRef<HTMLDivElement>(null);
  const onClick = () => {
    if (timelineSpanItemRef.current != null) {
      props.onTimelineSpanClick(props.data, timelineSpanItemRef.current);
    }
  }

  return (
    <>
      <div
        className={styles.timelineSpan}
        data-rin-timeline-category={props.data.Category}
        data-rin-timeline-name={props.data.Name}
        onClick={onClick}
      >
        <div
          className={styles.timelineSpan_name}
          title={
            `${props.data.Category}: ${props.data.Name}` +
            (props.data.Data != null ? '\n' + props.data.Data : '')
          }
        >
          {label}
        </div>
        {props.data.EventType === 'TimelineScope' ? (
          <div
            className={styles.timelineSpan_bar}
            ref={timelineSpanItemRef}
            title={props.data.Duration + 'ms'}
            style={{ width: width + '%', marginLeft: left + '%' }}
          />
        ) : (
          <div
            className={styles.timelineSpan_point}
            ref={timelineSpanItemRef}
            style={{ marginLeft: 'calc(' + left + '% - 4px)' }}
          />
        )}
      </div>
      {props.data.EventType === 'TimelineScope' &&
        props.data.Children.filter(props.filter).map((x, i) => (
          <TimelineSpan
            key={'timelineSpan-' + i}
            data={x}
            totalDuration={props.totalDuration}
            originDate={props.originDate}
            onTimelineSpanClick={props.onTimelineSpanClick}
            filter={props.filter}
          />
        ))}
    </>
  );
}

function asLabel(data: TimelineData): string {
  const category = data.Category.replace(/^.*\./, '');
  return data.Category === TimelineEventCategory.AspNetCoreMvcView
    ? `${category}: ${data.Data}`
    : `${category}: ${data.Name}`;
}
