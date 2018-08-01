import { action, observable, runInAction } from 'mobx';
import { createHubClient, IHubClient } from '../api/hubClient';
import { IRinCoreHub } from '../api/IRinCoreHub';
import { InspectorStore } from './InspectorStore';

export interface IAppStoreProps {
  appStore: AppStore;
}

export class AppStore {
  hubClient: IHubClient & IRinCoreHub;
  inspector = new InspectorStore();

  @observable viewMode: ViewMode = ViewMode.Inspector;
  @observable connected: boolean = false;

  endpointUrlBase: string;

  @action.bound
  ready() {
    const isDevelopment = process.env.NODE_ENV === 'development';
    const host = isDevelopment ? 'localhost:5000' : location.host;
    const protocol = location.protocol === 'http:' ? 'ws:' : 'wss:';
    const pathBase = document.querySelector('html')!.dataset.rinConfigPathBase || '/rin';
    const channelEndPoint = pathBase + '/chan';

    this.endpointUrlBase = pathBase;
    this.hubClient = createHubClient<IRinCoreHub>(`${protocol}//${host}${channelEndPoint}`);
    this.inspector.ready(this.hubClient);

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
