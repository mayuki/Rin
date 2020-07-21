import { CheckboxVisibility, DetailsList, IColumn, Icon, SearchBox } from 'office-ui-fabric-react';
import { Selection, SelectionMode } from 'office-ui-fabric-react/lib/Selection';
import React, { useEffect } from 'react';
import { RequestEventPayload } from '../../api/IRinCoreHub';
import * as styles from './Inspector.InspectorEventsList.css';

export interface InspectorEventsListProps {
  onFilterChange: (event?: React.ChangeEvent<HTMLInputElement>, newValue?: string) => void;
  query: string;
  selectedId: string | undefined;
  filteredItems: RequestEventPayload[];
  onActiveItemChanged: (item: RequestEventPayload) => void;
}

export function InspectorEventsList(props: InspectorEventsListProps) {
  const columns: IColumn[] = [
    {
      key: 'icon',
      name: '',
      isIconOnly: true,
      minWidth: 16,
      maxWidth: 16,
      onRender: (item: RequestEventPayload) => {
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
      onRender: (item: RequestEventPayload) => (
        <div
          title={item.Path}
          style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '#000' }}
        >
          <div className={styles.inspectorEventsItem_Path}>{item.Path}</div>
          <div className={styles.inspectorEventsItem_ReceivedAt}>
            {new Date(item.RequestReceivedAt).toLocaleString()}
          </div>
        </div>
      )
    },
    {
      key: 'ResponseStatusCode',
      name: 'StatusCode',
      fieldName: 'ResponseStatusCode',
      minWidth: 64,
      isResizable: true,
      onRender: (item: RequestEventPayload) =>
        item.ResponseStatusCode === 0 ? (
          <span>-</span>
        ) : (
          <span
            className={styles.inspectorEventsItem_ResponseStatusCode}
            style={{ color: item.ResponseStatusCode >= 400 && item.ResponseStatusCode <= 599 ? '#a80000' : '' }}
          >
            {item.ResponseStatusCode}
          </span>
        )
    }
  ];

  const selection = new Selection({
    getKey: (item: {Id: string}, index?: number) => item.Id
  });

  if (props.selectedId != null) {
    const selections = selection.getSelection();
    if (selections.length > 0 && (selections[0] as any).Id === props.selectedId) {
      return <></>;
    }
    selection.setAllSelected(false);
    selection.setKeySelected(props.selectedId, true, false);
  }

  return (
    <>
      <SearchBox
        placeholder="Filter"
        underlined={true}
        onChange={props.onFilterChange}
        value={props.query}
      />
      <DetailsList
        className={styles.inspectorEventsList}
        compact={true}
        checkboxVisibility={CheckboxVisibility.hidden}
        columns={columns}
        selection={selection}
        selectionMode={SelectionMode.single}
        items={props.filteredItems}
        onActiveItemChanged={props.onActiveItemChanged}
        selectionPreservedOnEmptyClick={true}
      />
    </>
  );
}
