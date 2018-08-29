import { action, observable, runInAction } from 'mobx';
import { createHubClient, IHubClient } from './api/hubClient';
import { IRinCoreHub, RequestRecordDetailPayload } from './api/IRinCoreHub';

export interface RinOnPageInspectorConfig {
  Position?: 'Bottom' | 'Top';
  PathBase: string;
  RequestId: string;
}

export class RinOnPageInspectorStore {
  @observable
  data: RequestRecordDetailPayload;

  @observable
  position: 'Bottom' | 'Top';

  @observable
  subRequests: RequestRecordDetailPayload[] = [];

  @observable
  config: RinOnPageInspectorConfig;

  private hubClient: IRinCoreHub & IHubClient;

  @action.bound
  async ready(config: RinOnPageInspectorConfig) {
    this.config = config;
    this.hubClient = createHubClient<IRinCoreHub>(getChannelEndPoint(config));
    this.position = config.Position || 'Bottom';

    const detail = await this.hubClient.GetDetailById(config.RequestId);

    runInAction(() => (this.data = detail));
  }

  @action.bound
  async fetchSubRequestById(id: string) {
    const detail = await this.hubClient.GetDetailById(id);

    runInAction(() => {
      this.subRequests = this.subRequests.concat([detail]);
    });
  }
}

function getChannelEndPoint(config: RinOnPageInspectorConfig) {
  const isDevelopment = process.env.NODE_ENV === 'development';
  const host = isDevelopment
    ? location.search.match(/__rin__dev__host=([^&]+)/)
      ? location.search.match(/__rin__dev__host=([^&]+)/)![1]
      : 'localhost:5000'
    : location.host;
  const protocol = location.protocol === 'http:' ? 'ws:' : 'wss:';
  // const pathBase = isDevelopment ? '/' : config.PathBase || '/rin';
  const endPointBasePath = config.PathBase || '/rin';
  const channelEndPoint = `${protocol}//${host}${endPointBasePath}/chan`;
  // const urlBase = `${location.protocol}//${host}${endPointBasePath}`;

  return channelEndPoint;
}

export const rinOnPageInspectorStore = new RinOnPageInspectorStore();
