import { observer } from 'mobx-react';
import { Icon, SearchBox } from 'office-ui-fabric-react';
import { CheckboxVisibility, DetailsList, IColumn } from 'office-ui-fabric-react/lib/DetailsList';
import * as React from 'react';
import { AppStore } from '../../store/AppStore';
import { InspectorStore } from '../../store/InspectorStore';
import './Inspector.css';
import { InspectorDetail } from './InspectorDetail';

@observer
export class Inspector extends React.Component<{ appStore: AppStore; inspectorStore: InspectorStore }, {}> {
  private readonly columns: IColumn[] = [
    {
      key: 'icon',
      name: '',
      isIconOnly: true,
      minWidth: 16,
      maxWidth: 16,
      onRender: (item: any) => {
        return (
          <Icon
            iconName={
              item.Path.match(/\.(jpg|png|svg)/)
                ? 'PictureCenter'
                : item.Path.match(/\.(js|vbs)/)
                  ? 'Script'
                  : item.Path.match(/\.(css)/)
                    ? 'FileCSS'
                    : item.Path.match(/\.html?/)
                      ? 'FileHTML'
                      : 'TextDocument'
            }
          />
        );
      }
    },
    {
      key: 'Path',
      name: 'Path',
      fieldName: 'Path',
      minWidth: 100,
      isResizable: true,
      onRender: (item: any) => (
        <span
          title={item.Path}
          style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '#000' }}
        >
          {item.Path}
        </span>
      )
    },
    {
      key: 'ResponseStatusCode',
      name: 'StatusCode',
      fieldName: 'ResponseStatusCode',
      minWidth: 64,
      isResizable: true,
      onRender: (item: any) =>
        item.ResponseStatusCode === 0 ? (
          <span>-</span>
        ) : (
          <span style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '' }}>
            {item.ResponseStatusCode}
          </span>
        )
    }
  ];

  public render() {
    return (
      <>
        <div className="inspectorFrame">
          <div className="leftPane">
            <SearchBox
              placeholder="Filter"
              underlined={true}
              onChange={this.props.inspectorStore.onFilterChange}
              value={this.props.inspectorStore.query}
            />
            <DetailsList
              compact={true}
              checkboxVisibility={CheckboxVisibility.hidden}
              columns={this.columns}
              items={this.props.inspectorStore.filteredItems}
              onActiveItemChanged={this.props.inspectorStore.onActiveItemChanged}
              selectionPreservedOnEmptyClick={true}
            />
          </div>
          <div className="rightPane">
            <InspectorDetail appStore={this.props.appStore} inspectorStore={this.props.inspectorStore} />
          </div>
        </div>
      </>
    );
  }
}
