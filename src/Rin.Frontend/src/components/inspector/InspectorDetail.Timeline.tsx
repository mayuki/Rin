import * as React from 'react';
import { TimelineData } from '../../api/IRinCoreHub';
import './InspectorDetail.Timeline.css';

export class Timeline extends React.Component<{ data: TimelineData }> {
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
          <TimelineSpans data={this.props.data} />
        </div>
      </div>
    );
  }
}

class TimelineSpans extends React.Component<{ data: TimelineData }> {
  render() {
    const totalDuration = this.props.data.Duration;
    const originDate = new Date(this.props.data.BeginTime);
    return (
      <>
        {this.props.data.Children.map((x, i) => (
          <TimelineSpan key={'timelineSpan-' + i} data={x} totalDuration={totalDuration} originDate={originDate} />
        ))}
      </>
    );
  }
}

class TimelineSpan extends React.Component<{ data: TimelineData; totalDuration: number; originDate: Date }> {
  render() {
    const elapsedMilliSecFromOrigin = new Date(this.props.data.BeginTime).valueOf() - this.props.originDate.valueOf();
    const width = (this.props.data.Duration / this.props.totalDuration) * 100;
    const left = 100 - ((this.props.totalDuration - elapsedMilliSecFromOrigin) / this.props.totalDuration) * 100;
    return (
      <>
        <div
          className="timelineSpan"
          data-rin-timeline-category={this.props.data.Category}
          title={this.props.data.Duration + 'ms'}
        >
          <div className="timelineSpan_name">
            {this.props.data.Category.replace('Rin.Timeline.', '')}: {this.props.data.Name}
          </div>
          <div className="timelineSpan_bar" style={{ width: width + '%', marginLeft: left + '%' }} />
        </div>
        {this.props.data.Children.map((x, i) => (
          <TimelineSpan
            key={'timelineSpan-' + i}
            data={x}
            totalDuration={this.props.totalDuration}
            originDate={this.props.originDate}
          />
        ))}
      </>
    );
  }
}
