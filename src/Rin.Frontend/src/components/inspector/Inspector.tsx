import { observer } from 'mobx-react';
import * as React from 'react';
import { inspectorStore } from '../../store/InspectorStore';
import * as styles from './Inspector.css';
import { InspectorEventsList } from './Inspector.InspectorEventsList';
import { InspectorDetail } from './InspectorDetail';

import SplitterLayout from 'react-splitter-layout';
import { RequestEventPayload } from '../../api/IRinCoreHub';

// Container Component
@observer
export class Inspector extends React.Component {
  public render() {
    return (
      <>
        <div className={styles.inspectorFrame}>
          <SplitterLayout
            secondaryMinSize={300}
            secondaryInitialSize={inspectorStore.leftPaneSize}
            primaryIndex={1}
            onSecondaryPaneSizeChange={inspectorStore.onUpdateLeftPaneSize}
          >
            <div className={styles.leftPane}>
              <InspectorEventsList
                filteredItems={inspectorStore.filteredItems}
                onActiveItemChanged={this.onActiveItemChanged}
                onFilterChange={inspectorStore.onFilterChange}
                query={inspectorStore.query}
              />
            </div>
            <div className={styles.rightPane}>
              <InspectorDetail />
            </div>
          </SplitterLayout>
        </div>
      </>
    );
  }

  private onActiveItemChanged = (item: RequestEventPayload) => {
    inspectorStore.selectDetail(item.Id);
  };
}
