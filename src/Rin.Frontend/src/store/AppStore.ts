import { action, observable, runInAction } from 'mobx';
import { createHubClient, IHubClient } from '../api/hubClient';
import { IRinCoreHub } from '../api/IRinCoreHub';
import { inspectorStore } from './InspectorStore';
import { inspectorTimelineStore } from './InspectorTimelineStore';

export interface IAppStoreProps {
  appStore: AppStore;
}

export class AppStore {
  hubClient: IHubClient & IRinCoreHub;

  @observable
  viewMode: ViewMode = ViewMode.Inspector;
  @observable
  connected: boolean = false;

  endpointUrlBase: string;

  inspectorTimelineStore = inspectorTimelineStore;
  inspectorStore = inspectorStore;

  @action.bound
  ready() {
    const isDevelopment = process.env.NODE_ENV === 'development';
    const host = isDevelopment
      ? location.search.match(/__rin__dev__host=([^&]+)/)
        ? location.search.match(/__rin__dev__host=([^&]+)/)![1]
        : 'localhost:5000'
      : location.host;
    const protocol = location.protocol === 'http:' ? 'ws:' : 'wss:';
    const pathBase = document.querySelector('html')!.dataset.rinConfigPathBase || '/rin';
    const channelEndPoint = pathBase + '/chan';

    this.endpointUrlBase = `${location.protocol}//${host}${pathBase}`;
    this.hubClient = createHubClient<IRinCoreHub>(`${protocol}//${host}${channelEndPoint}`);

    inspectorStore.ready(this.hubClient);
    inspectorTimelineStore.ready();

    this.hubClient.on('connected', () => {
      runInAction(() => (this.connected = true));
    });
    this.hubClient.on('disconnected', () => {
      runInAction(() => (this.connected = false));
    });
  }
}

export enum ViewMode {
  Inspector
}

export const appStore = new AppStore();
