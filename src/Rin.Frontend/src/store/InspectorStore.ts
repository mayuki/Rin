import { action, computed, observable, runInAction } from 'mobx';
import { IHubClient } from '../api/hubClient';
import { IRinCoreHub, RequestEventPayload, RequestRecordDetailPayload } from '../api/IRinCoreHub';

export class InspectorStore {
  @observable currentDetailView: DetailViewType = DetailViewType.Request;
  @observable selectedId: string | null = null;
  @observable query: string = '';
  @observable requestBody: string | null = null;
  @observable responseBody: string | null = null;
  @observable currentRecordDetail: RequestRecordDetailPayload | null;

  @observable items: RequestEventPayload[] = [];

  private hubClient: IHubClient & IRinCoreHub;
  private requestEventQueue: { event: 'RequestBegin' | 'RequestEnd'; args: any }[] = [];
  private triggerRequestEventQueueTimerId?: number;

  @computed
  get selectedItem() {
    return this.items.find(x => x.Id === this.selectedId);
  }

  @computed
  get filteredItems() {
    if (this.query == null || this.query.length === 0) {
      return this.items;
    }

    const regex = new RegExp(this.query.replace(/[.*+?^=!:${}()|[\]\/\\]/g, '\\$&'), 'i');
    return this.items.filter(x => x.Path.match(regex));
  }

  @action.bound
  onFilterChange(newValue: string) {
    this.query = newValue;
  }

  @action.bound
  async onActiveItemChanged(item: RequestEventPayload) {
    this.selectedId = item.Id;

    await this.updateCurrentRecordAsync(item.Id);
  }

  @action.bound
  selectDetailView(view: DetailViewType) {
    this.currentDetailView = view;
  }

  @action.bound
  updateItems(records: RequestEventPayload[]) {
    this.items = records;
  }

  @action.bound
  async fetchItemsAsync() {
    const records = await this.hubClient.GetRecordingList();
    this.updateItems(records);
  }

  @action.bound
  async updateCurrentRecordAsync(itemId: string) {
    const record = await this.hubClient.GetDetailById(itemId);

    if (this.currentDetailView === DetailViewType.Exception && record.Exception === null) {
      this.selectDetailView(DetailViewType.Request);
    }

    runInAction(() => {
      this.requestBody = null;
      this.responseBody = null;
      this.currentRecordDetail = record;
    });

    const requestBody = await this.hubClient.GetRequestBody(itemId);
    const responseBody = await this.hubClient.GetResponseBody(itemId);
    runInAction(() => {
      this.requestBody = requestBody;
      this.responseBody = responseBody;
    });
  }

  ready(hubClient: IHubClient & IRinCoreHub) {
    this.hubClient = hubClient;
    this.hubClient.on('reconnecting', () => {
      runInAction(() => this.fetchItemsAsync());
    });

    this.hubClient.on('RequestBegin', args => {
      this.requestEventQueue.push({ event: 'RequestBegin', args });
      this.triggerRequestEventQueue();
    });
    this.hubClient.on('RequestEnd', args => {
      this.requestEventQueue.push({ event: 'RequestEnd', args });
      this.triggerRequestEventQueue();
    });

    this.fetchItemsAsync();
  }

  private triggerRequestEventQueue() {
    if (this.triggerRequestEventQueueTimerId !== undefined) {
      clearTimeout(this.triggerRequestEventQueueTimerId);
      this.triggerRequestEventQueueTimerId = undefined;
    }

    this.triggerRequestEventQueueTimerId = window.setTimeout(() => {
      const items = this.items.concat([]);
      this.requestEventQueue.forEach(x => {
        const item = x.args[0];
        if (x.event === 'RequestBegin') {
          items.unshift(item);
        } else if (x.event === 'RequestEnd') {
          const itemIndex = items.findIndex(y => y.Id === item.Id);
          items[itemIndex] = item;

          if (item.Id === this.selectedId) {
            this.updateCurrentRecordAsync(item.Id);
          }
        }
      });
      this.requestEventQueue = [];
      this.updateItems(items);
    }, 100);
  }
}

export enum DetailViewType {
  Request = 'Request',
  Response = 'Response',
  Timing = 'Timing',
  Trace = 'Trace',
  Exception = 'Exception'
}
