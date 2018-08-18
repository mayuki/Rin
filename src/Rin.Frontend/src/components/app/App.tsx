import * as mobx from 'mobx';
import { observer } from 'mobx-react';
import { Fabric, FontClassNames, Overlay, Spinner, SpinnerSize } from 'office-ui-fabric-react';
import * as React from 'react';
import { Helmet } from 'react-helmet';
import { appStore } from '../../store/AppStore';
import { Inspector } from '../inspector/Inspector';
import './App.css';

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
          <div className="applicationFrame">
            <header>
              <h1 className={FontClassNames.xLarge}>Rin</h1>
            </header>
            <div className="contentArea">
              <Inspector />
              {!appStore.connected && (
                <>
                  <Overlay className="connectingOverlay">
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

export default App;
