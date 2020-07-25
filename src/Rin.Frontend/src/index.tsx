import './configurePublicPath';

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { App } from './components/app/App';
import './index.css';
import '../node_modules/react-splitter-layout/lib/index.css';
import { initializeIcons } from '@fluentui/react';

initializeIcons();

ReactDOM.render(<App />, document.getElementById('root'));
