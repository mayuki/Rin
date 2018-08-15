import { observer } from 'mobx-react';
import * as React from 'react';
import { inspectorStore } from '../../store/InspectorStore';
import './Inspector.css';
import { InspectorEventsList } from './Inspector.InspectorEventsList';
import { InspectorDetail } from './InspectorDetail';

import SplitterLayout from 'react-splitter-layout';

// Container Component
@observer
export class Inspector extends React.Component {
  public render() {
    return (
      <>
        <div className="inspectorFrame">
          <SplitterLayout
            secondaryMinSize={300}
            secondaryInitialSize={inspectorStore.leftPaneSize}
            primaryIndex={1}
            onSecondaryPaneSizeChange={inspectorStore.onUpdateLeftPaneSize}
          >
            <div className="leftPane">
              <InspectorEventsList
                filteredItems={inspectorStore.filteredItems}
                onActiveItemChanged={inspectorStore.onActiveItemChanged}
                onFilterChange={inspectorStore.onFilterChange}
                query={inspectorStore.query}
              />
            </div>
            <div className="rightPane">
              <InspectorDetail />
            </div>
          </SplitterLayout>
        </div>
      </>
    );
  }
}
