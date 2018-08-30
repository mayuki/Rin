import { rinInViewInspectorStore } from '../Store';

// tslint:disable:only-arrow-functions
function installHook_fetch() {
  const fetchOriginal = window.fetch;
  window.fetch = function(...args) {
    return (fetchOriginal.apply(window, args) as Promise<Response>).then(x => {
      const requestId = x.headers.get('X-Rin-Request-Id');
      if (requestId != null && requestId !== '') {
        rinInViewInspectorStore.fetchSubRequestById(requestId);
      }
      return x;
    });
  };
}

function installHook_XHR() {
  const xhrSend = XMLHttpRequest.prototype.send;
  XMLHttpRequest.prototype.send = function(...args) {
    this.addEventListener('loadend', () => {
      const requestId = this.getResponseHeader('X-Rin-Request-Id');
      if (requestId != null && requestId !== '') {
        rinInViewInspectorStore.fetchSubRequestById(requestId);
      }
    });
    xhrSend.apply(this, args);
  };
}

let hookInstalled = false;
export function installHooks() {
  if (hookInstalled) {
    return;
  }

  installHook_fetch();
  installHook_XHR();

  hookInstalled = true;
}

installHooks();
// tslint:enable:only-arrow-functions
