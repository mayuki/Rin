import { action } from 'mobx';

export class RinPageProfilerStore {
    @action.bind
    ready() {
        return true;
    }
}

export const rinPageProfilerStore = new RinPageProfilerStore();