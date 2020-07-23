import * as mobx from 'mobx';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { App } from './App';
import { installHooks } from './utilities/hooks';

if ((window as any).Proxy && (window as any).Symbol) {
  const scriptElement = document.querySelector('script[data-rin-in-view-inspector-config]') as HTMLElement;
  const config = JSON.parse(scriptElement.dataset.rinInViewInspectorConfig || 'null');

  mobx.configure({
    enforceActions: 'always',
  });

  installHooks();

  window.addEventListener('DOMContentLoaded', () => {
    const rootElement = document.createElement('div');
    rootElement.id = '__rinInViewInspectorRootGenerated__';
    document.body.appendChild(rootElement);
    ReactDOM.render(<App config={config} />, rootElement);
  });
} else {
  if (console != null) {
    console.warn(
      'In-View Rin Inspector requires "Proxy" and "Symbol" features. Most modern browsers implement that feature.'
    );
  }
}
