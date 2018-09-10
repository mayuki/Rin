import { action, observable, runInAction } from 'mobx';
import { RequestRecordDetailPayload } from './api/IRinCoreHub';

export interface RinInViewInspectorConfig {
  Position?: 'Bottom' | 'Top';
  PathBase: string;
  RequestId: string;
}

export class SubRequestFailurePayload {
  Id: string = new Date().valueOf().toString() + '.' + Math.random();
  constructor(public Path: string, public ResponseStatusCode: number) {}
}

export type SubRequestPayload = RequestRecordDetailPayload | SubRequestFailurePayload;

export class RinInViewInspectorStore {
  @observable
  data: RequestRecordDetailPayload;

  @observable
  position: 'Bottom' | 'Top';

  @observable
  subRequests: SubRequestPayload[] = [];

  @observable
  config: RinInViewInspectorConfig;

  private apiEndPointBase: string;

  @action.bound
  async ready(config: RinInViewInspectorConfig) {
    this.config = config;
    this.apiEndPointBase = getApiEndPointBase(config);
    this.position = config.Position || 'Bottom';

    const detail = await this.getDetailByIdAsync(config.RequestId);

    runInAction(() => (this.data = detail));
  }

  @action.bound
  async fetchSubRequestById(id: string) {
    const detail = await this.getDetailByIdAsync(id);

    runInAction(() => {
      this.subRequests = this.subRequests.concat([detail]);
    });
  }

  @action.bound
  addFailureSubRequest(url: string, status: number) {
    this.subRequests = this.subRequests.concat([new SubRequestFailurePayload(url, status)]);
  }

  private async getDetailByIdAsync(id: string): Promise<RequestRecordDetailPayload> {
    return fetch(`${this.apiEndPointBase}/GetDetailById?id=${id}`).then(x => x.json());
  }
}

function getApiEndPointBase(config: RinInViewInspectorConfig) {
  const isDevelopment = process.env.NODE_ENV === 'development';
  const host = isDevelopment
    ? location.search.match(/__rin__dev__host=([^&]+)/)
      ? location.search.match(/__rin__dev__host=([^&]+)/)![1]
      : 'localhost:5000'
    : location.host;
  // const protocol = location.protocol === 'http:' ? 'ws:' : 'wss:';
  // const pathBase = isDevelopment ? '/' : config.PathBase || '/rin';
  const endPointBasePath = config.PathBase || '/rin';
  // const channelEndPoint = `${protocol}//${host}${endPointBasePath}/chan`;
  const urlBase = `${location.protocol}//${host}${endPointBasePath}/api`;

  return urlBase;
}

export const rinInViewInspectorStore = new RinInViewInspectorStore();
