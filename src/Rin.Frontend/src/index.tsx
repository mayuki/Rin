// tslint:disable orderd-imports
import './configurePublicPath';
// tslint:enable orderd-imports

import { loadTheme } from '@uifabric/styling';
import { initializeIcons } from 'office-ui-fabric-react/lib/Icons';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import App from './components/app/App';
import './index.css';
import { appStore } from './store/AppStore';

// Initialize Office UI Fabric
initializeIcons(/* optional base url */);
loadTheme({
  palette: {
    themePrimary: '#9cc4be',
    themeLighterAlt: '#fbfdfc',
    themeLighter: '#eef6f4',
    themeLight: '#dfedeb',
    themeTertiary: '#c1dcd8',
    themeSecondary: '#a8cbc6',
    themeDarkAlt: '#8db1ab',
    themeDark: '#779591',
    themeDarker: '#586e6b',
    neutralLighterAlt: '#f8f8f8',
    neutralLighter: '#f4f4f4',
    neutralLight: '#eaeaea',
    neutralQuaternaryAlt: '#dadada',
    neutralQuaternary: '#d0d0d0',
    neutralTertiaryAlt: '#c8c8c8',
    neutralTertiary: '#c2c2c2',
    neutralSecondary: '#858585',
    neutralPrimaryAlt: '#4b4b4b',
    neutralPrimary: '#333',
    neutralDark: '#272727',
    black: '#1d1d1d',
    white: '#fff'
  }
});

(window as any).__appStore = appStore;

ReactDOM.render(<App />, document.getElementById('root') as HTMLElement);
