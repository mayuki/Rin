import { action, observable, runInAction } from 'mobx';
import { createHubClient, IHubClient } from './api/hubClient';
import { IRinCoreHub, RequestRecordDetailPayload } from './api/IRinCoreHub';

export interface RinOnPageInspectorConfig {
  Position?: 'Bottom' | 'Top';
  HubEndPoint: string;
  RequestId: string;
}

export class RinOnPageInspectorStore {
  @observable
  data: RequestRecordDetailPayload;

  @observable
  position: 'Bottom' | 'Top';

  private hubClient: IRinCoreHub & IHubClient;

  @action.bound
  async ready(config: RinOnPageInspectorConfig) {
    this.hubClient = createHubClient<IRinCoreHub>(config.HubEndPoint);
    this.position = config.Position || 'Bottom';
    const detail = await this.hubClient.GetDetailById(config.RequestId);

    runInAction(() => (this.data = detail));

    this.hubClient.dispose();
  }
}

export const rinOnPageInspectorStore = new RinOnPageInspectorStore();
