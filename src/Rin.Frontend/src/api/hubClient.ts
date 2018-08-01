import { EventEmitter } from 'events';

interface InvokeResponseQueue {
  [requestId: string]: (value: any) => void;
}

interface InvokeWaitingOperation {
  method: string;
  args: any[];
  opId: string;
}

export interface IHubClient {
  invoke: (method: string, ...args: any[]) => Promise<any>;
  on: (event: string, listener: ((...args: any[]) => void)) => void;
  off: (event: string, listener: ((...args: any[]) => void)) => void;
  dispose(): void;
}

function invoke(holder: SocketHolder, method: string, ...args: any[]): Promise<any> {
  const opId = new Date().valueOf() + '.' + Math.random();

  if (holder.socket!.readyState !== WebSocket.OPEN) {
    return new Promise(resolve => {
      holder.invokeResponseQueue[opId] = resolve;
      holder.invokeWaitingQueue.push({ method, args, opId });
    });
  }

  return new Promise(resolve => {
    holder.invokeResponseQueue[opId] = resolve;
    holder.socket!.send(JSON.stringify({ M: method, A: args, O: opId }));
  });
}

function prepare(holder: SocketHolder) {
  holder.socket = holder.socketFactory();
  holder.socket.addEventListener('open', () => {
    holder.eventEmitter.emit('connected');
    holder.connected = true;
    let invokeParam = holder.invokeWaitingQueue.shift();
    while (invokeParam !== undefined) {
      holder.socket!.send(JSON.stringify({ M: invokeParam.method, A: invokeParam.args, O: invokeParam.opId }));
      invokeParam = holder.invokeWaitingQueue.shift();
    }
  });

  holder.socket.addEventListener('message', e => {
    const data = JSON.parse(e.data);
    if (data.R != null) {
      if (holder.invokeResponseQueue[data.R] != null) {
        holder.invokeResponseQueue[data.R](data.V);

        delete holder.invokeResponseQueue[data.R];
      }
    } else if (data.M != null) {
      holder.eventEmitter.emit(data.M, data.A);
    }
  });

  holder.socket.addEventListener('close', () => {
    console.log('closed');
    if (holder.connected) {
      holder.eventEmitter.emit('disconnected');
    }
    holder.connected = false;

    setTimeout(() => {
      holder.eventEmitter.emit('reconnecting');
      console.log('reconnecting...');
      holder.socket = holder.socketFactory();
      prepare(holder);
    }, 1000);
  });
}

interface SocketHolder {
  pingTimer: number | null;
  connected: boolean;
  socket: WebSocket | null;
  socketFactory: () => WebSocket;
  invokeResponseQueue: InvokeResponseQueue;
  invokeWaitingQueue: InvokeWaitingOperation[];
  eventEmitter: EventEmitter;
}

function setPingTimer(holder: SocketHolder) {
  clearPingTimer(holder);

  holder.pingTimer = window.setInterval(() => {
    if (!holder.connected) {
      return;
    }
    invoke(holder, 'Ping');
  }, 5000);
}

function clearPingTimer(holder: SocketHolder) {
  if (holder.pingTimer != null) {
    clearInterval(holder.pingTimer);
    holder.pingTimer = null;
  }
}

export function createHubClient<THub>(url: string): IHubClient & THub {
  const invokeResponseQueue: InvokeResponseQueue = {};
  const invokeWaitingQueue: InvokeWaitingOperation[] = [];
  const eventEmitter = new EventEmitter();

  let isDisposed = false;
  const socketHolder: SocketHolder = {
    connected: false,
    pingTimer: null,
    socket: null,
    invokeResponseQueue,
    invokeWaitingQueue,
    eventEmitter,
    socketFactory: () => new WebSocket(url)
  };

  const client = {
    invoke: (method: string, ...args: any[]) => {
      if (isDisposed) {
        throw new Error('Object Disposed');
      }
      return invoke(socketHolder, method, ...args);
    },
    off: (event: string, listener: ((...args: any[]) => void)) => {
      eventEmitter.off(event, listener);
    },
    on: (event: string, listener: ((...args: any[]) => void)) => {
      eventEmitter.on(event, listener);
    },
    dispose: () => {
      clearPingTimer(socketHolder);
      isDisposed = true;
    }
  };

  prepare(socketHolder);

  setPingTimer(socketHolder);

  return new Proxy(client, {
    get: (target, name) => {
      if (!(name in target)) {
        return (...args: any[]) => target.invoke(name.toString(), ...args);
      }
      return target[name];
    }
  }) as IHubClient & THub;
}
