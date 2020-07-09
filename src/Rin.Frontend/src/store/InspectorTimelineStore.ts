import { action, observable } from 'mobx';
import { TimelineData } from '../api/IRinCoreHub';

export class InspectorTimelineStore {
  @observable
  isCalloutVisible = false;

  @observable
  calloutTarget!: HTMLElement;

  @observable
  calloutTimelineData!: TimelineData | null;

  @observable
  isTraceVisibleInTimeline = true;

  @action.bound
  showCallout(timelineData: TimelineData, target: HTMLElement) {
    this.calloutTimelineData = timelineData;
    this.calloutTarget = target;
    this.isCalloutVisible = true;
  }

  @action.bound
  dismissCallout() {
    this.isCalloutVisible = false;
  }

  @action.bound
  toggleTraceVisibility(visible: boolean) {
    this.isTraceVisibleInTimeline = visible;
    window.localStorage['Rin.Inspector.Timeline.IsTraceVisibleInTimeline'] = JSON.stringify(visible);
  }

  @action.bound
  ready() {
    this.isTraceVisibleInTimeline = JSON.parse(
      window.localStorage['Rin.Inspector.Timeline.IsTraceVisibleInTimeline'] || 'true'
    );
  }
}

export const inspectorTimelineStore = new InspectorTimelineStore();
