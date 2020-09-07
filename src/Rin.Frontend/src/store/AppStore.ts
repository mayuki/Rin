import { action, observable, runInAction } from 'mobx';
import { createHubClient, IHubClient } from '../api/hubClient';
import { IRinCoreHub } from '../api/IRinCoreHub';
import { useContext, createContext } from 'react';

export interface IAppStoreProps {
  appStore: AppStore;
}

export class AppStore {
  hubClient!: IHubClient & IRinCoreHub;

  @observable
  isReady = false;

  @observable
  viewMode: ViewMode = ViewMode.Inspector;

  @observable
  connected = false;

  @observable
  serverInfo = {
    Version: '',
    BuildDate: '',
    FeatureFlags: [] as string[],
    Host: '',
    EndPointBase: '/rin',
    PathBase: '/rin',
  };

  endpointUrlBase!: string;

  @action.bound
  async updateServerInfoAsync() {
    const prevVersion = this.serverInfo.Version;
    const prevBuildDate = this.serverInfo.BuildDate;

    const payload = await this.hubClient.GetServerInfo();
    runInAction(() => (this.serverInfo = { ...this.serverInfo, ...payload }));

    console.log(
      `Rin Server: Version=${this.serverInfo.Version}; BuildDate=${
        this.serverInfo.BuildDate
      }; FeatureFlags=${this.serverInfo.FeatureFlags.join(',')}`
    );

    // Reload when version of the server is changed.
    if (
      (prevVersion !== '' && prevVersion !== this.serverInfo.Version) ||
      (prevBuildDate !== '' && prevBuildDate !== this.serverInfo.BuildDate)
    ) {
      console.log(`Rin server was updated. Reloading...`);
      window.location.reload();
    }
  }

  @action.bound
  ready() {
    const isDevelopment = process.env.NODE_ENV === 'development';
    const host = isDevelopment
      ? location.search.match(/__rin__dev__host=([^&]+)/)
        ? location.search.match(/__rin__dev__host=([^&]+)/)![1]
        : 'localhost:5000'
      : location.host;
    const protocol = location.protocol === 'http:' ? 'ws:' : 'wss:';
    const pathBase = isDevelopment ? '/' : document.querySelector('html')!.dataset.rinConfigPathBase || '/rin';
    const endPointBasePath = document.querySelector('html')!.dataset.rinConfigPathBase || '/rin';
    const channelEndPoint = endPointBasePath + '/chan';

    this.endpointUrlBase = `${location.protocol}//${host}${endPointBasePath}`;
    this.hubClient = createHubClient<IRinCoreHub>(`${protocol}//${host}${channelEndPoint}`);

    this.updateServerInfoAsync();
    this.serverInfo = { ...this.serverInfo, Host: host, PathBase: pathBase, EndPointBase: endPointBasePath };

    this.hubClient.on('connected', () => {
      runInAction(() => (this.connected = true));
    });
    this.hubClient.on('disconnected', () => {
      runInAction(() => (this.connected = false));
    });
    this.hubClient.on('reconnecting', () => {
      this.updateServerInfoAsync();
    });

    this.isReady = true;
  }
}

export enum ViewMode {
  Inspector,
}

const appStoreContext = createContext(new AppStore());
export const useAppStore = () => useContext(appStoreContext);
