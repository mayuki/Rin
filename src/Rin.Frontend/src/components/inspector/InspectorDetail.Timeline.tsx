import { Callout, DirectionalHint, FontClassNames, Icon } from 'office-ui-fabric-react';
import * as React from 'react';
import { TimelineData } from '../../api/IRinCoreHub';
import './InspectorDetail.Timeline.css';

type OnSelectSpan = (target: HTMLElement, data: TimelineData) => void;

export class Timeline extends React.Component<
  { data: TimelineData },
  { isCalloutVisible: boolean; calloutTarget: HTMLElement | null; calloutData: TimelineData | null }
> {
  private calloutTarget = React.createRef<HTMLDivElement>();

  constructor(props: { data: TimelineData }) {
    super(props);
    this.state = { isCalloutVisible: false, calloutTarget: null, calloutData: null };
  }

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
        <div className="timeline_header" ref={this.calloutTarget}>
          {columns}
        </div>
        <div className="timeline_spans">
          <TimelineSpans data={this.props.data} onSelectSpan={this.onSelectSpan} />
        </div>
        {this.state.isCalloutVisible && (
          <Callout
            gapSpace={0}
            target={this.state.calloutTarget}
            setInitialFocus={true}
            hidden={!this.state.isCalloutVisible}
            onDismiss={this.onCalloutDismiss}
            directionalHint={DirectionalHint.bottomCenter}
          >
            <div className="timelineCalloutContent">
              <h2 className={FontClassNames.large}>
                {this.state.calloutData!.Category.replace(/^Rin\.Timeline\./, '')}: {this.state.calloutData!.Name}
              </h2>
              {this.state.calloutData!.Data && (
                <pre className="timelineCalloutContent_data">{this.state.calloutData!.Data}</pre>
              )}
              <div>
                <Icon iconName="Timer" /> {this.state.calloutData!.Duration}
                ms
              </div>
            </div>
          </Callout>
        )}
      </div>
    );
  }
  private onSelectSpan = (target: HTMLElement, data: TimelineData) => {
    this.setState({ isCalloutVisible: true, calloutTarget: target, calloutData: data });
  };
  private onCalloutDismiss = (): void => {
    this.setState({
      isCalloutVisible: false
    });
  };
}

class TimelineSpans extends React.Component<{ data: TimelineData; onSelectSpan: OnSelectSpan }> {
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
            onSelectSpan={this.props.onSelectSpan}
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
  onSelectSpan: OnSelectSpan;
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
            onSelectSpan={this.props.onSelectSpan}
          />
        ))}
      </>
    );
  }

  private onClick = () => {
    this.props.onSelectSpan(this.timelineSpanBarRef.current!, this.props.data);
  };
}
