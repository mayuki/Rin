import { Callout, DirectionalHint, FontClassNames, Icon, Toggle } from 'office-ui-fabric-react';
import * as React from 'react';
import { TimelineData, TimelineDataScope, TimelineEventCategory } from '../../api/IRinCoreHub';
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

export class InspectorDetailTimelineView extends React.Component<InspectorDetailTimelineViewProps> {
  render() {
    const filter = this.props.isTraceVisibleInTimeline
      ? (_: TimelineData) => true
      : (data: TimelineData) => data.Category !== TimelineEventCategory.Trace;

    return (
      <div className={styles.inspectorDetailTimelineView}>
        <div className={styles.inspectorDetailTimelineView_Commands}>
          <Toggle
            inlineLabel={true}
            label="Show Traces"
            checked={this.props.isTraceVisibleInTimeline}
            onChanged={this.props.toggleTraceVisibility}
          />
        </div>
        <Timeline {...this.props} filter={filter} />
      </div>
    );
  }
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

class Timeline extends React.Component<TimelineProps> {
  render() {
    const beginTime = new Date(this.props.data.Timestamp).valueOf();
    const endTime = maxEndTime(this.props.data);
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

    return (
      <div className={styles.timeline}>
        <div className={styles.timeline_header}>{columns}</div>
        <div className={styles.timeline_spans}>
          <TimelineSpans
            data={this.props.data}
            totalDuration={totalDuration}
            filter={this.props.filter}
            onTimelineSpanClick={this.props.showCallout}
          />
        </div>
        {this.props.isCalloutVisible && this.props.calloutTimelineData != null && this.renderCallout()}
      </div>
    );
  }

  private renderCallout() {
    const data = this.props.calloutTimelineData!;
    const hasChildren = data.EventType === 'TimelineScope' && data.Children.length > 0;
    const childrenDuration = data.EventType === 'TimelineScope' ? calculateChildrenDuration(data) : 0;
    // TODO: a child scope may be longer than the scope. (e.g. ActionResult < Transferring)
    const selfDuration = data.EventType === 'TimelineScope' ? Math.max(0, data.Duration - childrenDuration) : 0;

    return (
      <>
        <Callout
          gapSpace={0}
          target={this.props.calloutTarget}
          setInitialFocus={true}
          hidden={!this.props.isCalloutVisible}
          onDismiss={this.props.dismissCallout}
          directionalHint={DirectionalHint.bottomCenter}
        >
          <div className={styles.timelineCalloutContent}>
            <h2 className={FontClassNames.large}>
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
}

function maxEndTime(data: TimelineDataScope): number {
  const childrenEndTimeMax = data.Children.filter(x => x.EventType === 'TimelineScope').reduce(
    (r, v: TimelineDataScope) => Math.max(maxEndTime(v), r),
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

class TimelineSpans extends React.Component<TimelineSpansProps> {
  render() {
    const originDate = new Date(this.props.data.Timestamp);
    return (
      <>
        {this.props.data.Children.filter(this.props.filter).map((x, i) => (
          <TimelineSpan
            key={'timelineSpan-' + i}
            data={x}
            totalDuration={this.props.totalDuration}
            originDate={originDate}
            onTimelineSpanClick={this.props.onTimelineSpanClick}
            filter={this.props.filter}
          />
        ))}
      </>
    );
  }
}

interface TimelineSpanProps {
  data: TimelineData;
  totalDuration: number;
  originDate: Date;
  onTimelineSpanClick: (data: TimelineData, target: HTMLElement) => void;
  filter: (data: TimelineData) => boolean;
}

class TimelineSpan extends React.Component<TimelineSpanProps> {
  private timelineSpanItemRef = React.createRef<HTMLDivElement>();

  render() {
    const elapsedMilliSecFromOrigin = new Date(this.props.data.Timestamp).valueOf() - this.props.originDate.valueOf();
    const width =
      this.props.data.EventType === 'TimelineScope' ? (this.props.data.Duration / this.props.totalDuration) * 100 : 0;
    const left = 100 - ((this.props.totalDuration - elapsedMilliSecFromOrigin) / this.props.totalDuration) * 100;
    const label = asLabel(this.props.data);
    return (
      <>
        <div
          className={styles.timelineSpan}
          data-rin-timeline-category={this.props.data.Category}
          data-rin-timeline-name={this.props.data.Name}
          onClick={this.onClick}
        >
          <div
            className={styles.timelineSpan_name}
            title={
              `${this.props.data.Category}: ${this.props.data.Name}` +
              (this.props.data.Data != null ? '\n' + this.props.data.Data : '')
            }
          >
            {label}
          </div>
          {this.props.data.EventType === 'TimelineScope' ? (
            <div
              className={styles.timelineSpan_bar}
              ref={this.timelineSpanItemRef}
              title={this.props.data.Duration + 'ms'}
              style={{ width: width + '%', marginLeft: left + '%' }}
            />
          ) : (
            <div
              className={styles.timelineSpan_point}
              ref={this.timelineSpanItemRef}
              style={{ marginLeft: 'calc(' + left + '% - 4px)' }}
            />
          )}
        </div>
        {this.props.data.EventType === 'TimelineScope' &&
          this.props.data.Children.filter(this.props.filter).map((x, i) => (
            <TimelineSpan
              key={'timelineSpan-' + i}
              data={x}
              totalDuration={this.props.totalDuration}
              originDate={this.props.originDate}
              onTimelineSpanClick={this.props.onTimelineSpanClick}
              filter={this.props.filter}
            />
          ))}
      </>
    );
  }

  private onClick = () => {
    this.props.onTimelineSpanClick(this.props.data, this.timelineSpanItemRef.current!);
  };
}

function asLabel(data: TimelineData): string {
  const category = data.Category.replace(/^.*\./, '');
  return data.Category === TimelineEventCategory.AspNetCoreMvcView
    ? `${category}: ${data.Data}`
    : `${category}: ${data.Name}`;
}
