import { rinInViewInspectorStore } from '../Store';

// tslint:disable:only-arrow-functions
function installHook_fetch() {
  const fetchOriginal = window.fetch;
  window.fetch = function (...args) {
    return (fetchOriginal.apply(window, args) as Promise<Response>).then((x) => {
      if (new URL(x.url).origin != location.origin) return x;

      const requestId = x.headers.get('X-Rin-Request-Id');
      if (requestId != null && requestId !== '') {
        // WORKAROUND: The request on server-side may not be finished yet while "loadend" event on client-side.
        setTimeout(() => rinInViewInspectorStore.fetchSubRequestById(requestId), 100);
      } else if (x.status >= 400 && x.status <= 599) {
        rinInViewInspectorStore.addFailureSubRequest(x.url, x.status);
      }
      return x;
    });
  };
}

function installHook_XHR() {
  const xhrSend = XMLHttpRequest.prototype.send;
  XMLHttpRequest.prototype.send = function (...args) {
    this.addEventListener('loadend', () => {
      if (new URL(this.responseURL).origin != location.origin) return;

      const requestId = this.getResponseHeader('X-Rin-Request-Id');
      if (requestId != null && requestId !== '') {
        // WORKAROUND: The request on server-side may not be finished yet while "loadend" event on client-side.
        setTimeout(() => rinInViewInspectorStore.fetchSubRequestById(requestId), 100);
      } else if (this.status >= 400 && this.status <= 599) {
        rinInViewInspectorStore.addFailureSubRequest(this.responseURL, this.status);
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
