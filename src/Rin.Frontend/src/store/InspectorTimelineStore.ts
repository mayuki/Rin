import { action, observable } from 'mobx';
import { TimelineData } from '../api/IRinCoreHub';

export class InspectorTimelineStore {
  @observable
  isCalloutVisible = false;

  @observable
  calloutTarget: HTMLElement;

  @observable
  calloutTimelineData: TimelineData | null;

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
}

export const inspectorTimelineStore = new InspectorTimelineStore();
