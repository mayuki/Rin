import * as React from 'react';
import { Switch, Route, BrowserRouter, useParams } from 'react-router-dom';
import {
  Customizer,
  ICustomizations,
  FontClassNames,
  createTheme,
  Overlay,
  Spinner,
  SpinnerSize,
} from '@fluentui/react';
import { Helmet } from 'react-helmet';
import * as styles from './App.css';
import { inspectorStore } from '../../store/InspectorStore';
import { appStore } from '../../store/AppStore';
import { Inspector } from '../inspector/Inspector';
import * as mobx from 'mobx';
import { observer } from 'mobx-react';

export const RinTheme: ICustomizations = {
  settings: {
    theme: createTheme({
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
        white: '#fff',
      },
    }),
  },
  scopedSettings: {},
};

mobx.configure({
  enforceActions: true
});

@observer
export class App extends React.Component {
  public render() {
    return (
      <>
        <Customizer {...RinTheme}>
          <Helmet>
            <title>Rin</title>
          </Helmet>
          <div className={styles.applicationFrame}>
            <header>
              <h1 className={FontClassNames.xLarge}>Rin</h1>
            </header>
            <div className={styles.contentArea}>
              <BrowserRouter basename={appStore.serverInfo.PathBase}>
                <Switch>
                  <Route path="/" exact={true} component={InspectorWithRouteMatch} />
                  <Route path="/inspect/:id?/:section?" component={InspectorWithRouteMatch} />
                </Switch>
              </BrowserRouter>
              {!appStore.connected && (
                <>
                  <Overlay className={styles.connectingOverlay}>
                    <Spinner size={SpinnerSize.large} label="Connecting..." ariaLive="assertive" />
                  </Overlay>
                </>
              )}
            </div>
          </div>
        </Customizer>
      </>
    );
  }
}

function InspectorWithRouteMatch() {
  const { id, section } = useParams();
  if (id != null) {
    inspectorStore.selectDetail(id, section);
  }

  return <Inspector />;
}