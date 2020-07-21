import { observer } from 'mobx-react';
import * as React from 'react';
import { inspectorStore } from '../../store/InspectorStore';
import * as styles from './Inspector.css';
import { InspectorEventsList } from './Inspector.InspectorEventsList';
import { InspectorDetail } from './InspectorDetail';

import SplitterLayout from 'react-splitter-layout';
import { RequestEventPayload } from '../../api/IRinCoreHub';

// Container Component
export const Inspector = observer(function Inspector() {
  const onActiveItemChanged = (item: RequestEventPayload) => {
    inspectorStore.selectDetail(item.Id);
  };
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
              onActiveItemChanged={onActiveItemChanged}
              onFilterChange={inspectorStore.onFilterChange}
              query={inspectorStore.query}
              selectedId={inspectorStore.selectedId || undefined}
            />
          </div>
          <div className={styles.rightPane}>
            <InspectorDetail />
          </div>
        </SplitterLayout>
      </div>
    </>
  );
});
