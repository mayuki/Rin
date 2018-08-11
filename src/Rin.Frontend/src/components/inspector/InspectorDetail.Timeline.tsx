import { Callout, DirectionalHint, FontClassNames, Icon } from 'office-ui-fabric-react';
import * as React from 'react';
import { TimelineData } from '../../api/IRinCoreHub';
import './InspectorDetail.Timeline.css';

export interface TimelineProps {
  data: TimelineData;
  isCalloutVisible: boolean;
  showCallout: (data: TimelineData, target: HTMLElement) => void;
  dismissCallout: () => void;
  calloutTimelineData: TimelineData | null;
  calloutTarget: HTMLElement;
}

export class Timeline extends React.Component<TimelineProps> {
  render() {
    const columns = [
      <div key="Column-Event" className="timeline_headerColumn">
        <span>Event</span>
      </div>
    ];
    for (let i = 0; i < 10; i++) {
      const t = (this.props.data.Duration / 10) * i;
      columns.push(
        <div key={'Column-' + i} className="timeline_headerColumn">
          <span>
            {t.toFixed(1)}
            ms
          </span>
        </div>
      );
    }

    return (
      <div className="timeline">
        <div className="timeline_header">{columns}</div>
        <div className="timeline_spans">
          <TimelineSpans data={this.props.data} onTimelineSpanClick={this.props.showCallout} />
        </div>
        {this.props.isCalloutVisible &&
          this.props.calloutTimelineData != null && (
            <Callout
              gapSpace={0}
              target={this.props.calloutTarget}
              setInitialFocus={true}
              hidden={!this.props.isCalloutVisible}
              onDismiss={this.props.dismissCallout}
              directionalHint={DirectionalHint.bottomCenter}
            >
              <div className="timelineCalloutContent">
                <h2 className={FontClassNames.large}>
                  {this.props.calloutTimelineData.Category.replace(/^Rin\.Timeline\./, '')}:{' '}
                  {this.props.calloutTimelineData.Name}
                </h2>
                {this.props.calloutTimelineData.Data && (
                  <pre className="timelineCalloutContent_data">{this.props.calloutTimelineData.Data}</pre>
                )}
                <div>
                  <Icon iconName="Timer" /> {this.props.calloutTimelineData.Duration}
                  ms
                </div>
              </div>
            </Callout>
          )}
      </div>
    );
  }
}

class TimelineSpans extends React.Component<{
  data: TimelineData;
  onTimelineSpanClick: (data: TimelineData, target: HTMLElement) => void;
}> {
  render() {
    const totalDuration = this.props.data.Duration;
    const originDate = new Date(this.props.data.Timestamp);
    return (
      <>
        {this.props.data.Children.map((x, i) => (
          <TimelineSpan
            key={'timelineSpan-' + i}
            data={x}
            totalDuration={totalDuration}
            originDate={originDate}
            onTimelineSpanClick={this.props.onTimelineSpanClick}
          />
        ))}
      </>
    );
  }
}

class TimelineSpan extends React.Component<{
  data: TimelineData;
  totalDuration: number;
  originDate: Date;
  onTimelineSpanClick: (data: TimelineData, target: HTMLElement) => void;
}> {
  private timelineSpanBarRef = React.createRef<HTMLDivElement>();

  render() {
    const elapsedMilliSecFromOrigin = new Date(this.props.data.Timestamp).valueOf() - this.props.originDate.valueOf();
    const width = (this.props.data.Duration / this.props.totalDuration) * 100;
    const left = 100 - ((this.props.totalDuration - elapsedMilliSecFromOrigin) / this.props.totalDuration) * 100;
    const label = this.props.data.Category.replace(/^Rin\.Timeline\.(AspNetCore\.)?/, '') + ': ' + this.props.data.Name;
    return (
      <>
        <div className="timelineSpan" data-rin-timeline-category={this.props.data.Category} onClick={this.onClick}>
          <div
            className="timelineSpan_name"
            title={label + (this.props.data.Data != null ? '\n' + this.props.data.Data : '')}
          >
            {label}
          </div>
          <div
            className="timelineSpan_bar"
            ref={this.timelineSpanBarRef}
            title={this.props.data.Duration + 'ms'}
            style={{ width: width + '%', marginLeft: left + '%' }}
          />
        </div>
        {this.props.data.Children.map((x, i) => (
          <TimelineSpan
            key={'timelineSpan-' + i}
            data={x}
            totalDuration={this.props.totalDuration}
            originDate={this.props.originDate}
            onTimelineSpanClick={this.props.onTimelineSpanClick}
          />
        ))}
      </>
    );
  }

  private onClick = () => {
    this.props.onTimelineSpanClick(this.props.data, this.timelineSpanBarRef.current!);
  };
}
