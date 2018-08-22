import * as mobx from 'mobx';
import { observer } from 'mobx-react';
import { Fabric, FontClassNames, Overlay, Spinner, SpinnerSize } from 'office-ui-fabric-react';
import * as React from 'react';
import { Helmet } from 'react-helmet';
import { match as Match, Route, RouteComponentProps, Switch } from 'react-router';
import { BrowserRouter as Router } from 'react-router-dom';
import { appStore } from '../../store/AppStore';
import { inspectorStore } from '../../store/InspectorStore';
import { Inspector } from '../inspector/Inspector';
import * as styles from './App.css';

mobx.configure({
  enforceActions: true
});

@observer
class App extends React.Component {
  constructor(props: {}) {
    super(props);

    appStore.ready();
  }

  public render() {
    return (
      <>
        <Helmet>{appStore.serverInfo.Host !== '' && <title>Rin: {appStore.serverInfo.Host}</title>}</Helmet>
        <Fabric>
          <div className={styles.applicationFrame}>
            <header>
              <h1 className={FontClassNames.xLarge}>Rin</h1>
            </header>
            <div className={styles.contentArea}>
              <Router>
                <Switch>
                  <Route path="/" exact={true} component={InspectorWithRouteMatch} />
                  <Route path="/inspect/:id?/:section?" component={InspectorWithRouteMatch} />
                </Switch>
              </Router>
              {!appStore.connected && (
                <>
                  <Overlay className={styles.connectingOverlay}>
                    <Spinner size={SpinnerSize.large} label="Connecting..." ariaLive="assertive" />
                  </Overlay>
                </>
              )}
            </div>
          </div>
        </Fabric>
      </>
    );
  }
}

function syncWithRoute<P>(WrappedComponent: React.ComponentType<P>, onMatched: ((match: Match<any>) => void)) {
  return class extends React.Component<RouteComponentProps<any> & P> {
    componentDidMount() {
      onMatched(this.props.match);
    }
    componentDidUpdate() {
      onMatched(this.props.match);
    }
    render() {
      return <WrappedComponent {...this.props} />;
    }
  };
}

const InspectorWithRouteMatch = syncWithRoute(Inspector, match => {
  if (match.params.id != null) {
    inspectorStore.selectDetail(match.params.id, match.params.section, true);
  }
});

export default App;
