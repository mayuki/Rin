import * as React from 'react';
import * as ReactDOM from 'react-dom';
import App from './App';
import './index.css';
import { rinOnPageInspectorStore } from './Store';

const rootElement = document.getElementById('rinOnViewInspectorRoot') as HTMLElement;
const config = JSON.parse(rootElement.dataset.rinOnViewInspectorConfig || 'null');

rinOnPageInspectorStore.ready(config);

ReactDOM.render(<App />, rootElement);
