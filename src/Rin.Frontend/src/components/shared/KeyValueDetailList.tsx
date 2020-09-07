import { CheckboxVisibility, DetailsList, IColumn, IconButton, SelectionMode, IContextualMenu } from 'office-ui-fabric-react';
import * as React from 'react';
import { copyTextToClipboard } from '../../utilities';
import * as styles from './KeyValueDetailList.css';
import { IContextualMenuItem } from '@fluentui/react';

export interface KeyValuePairLike {
  key: string;
  value: string;
}

export function KeyValueDetailList(props: {
  keyName: string;
  valueName: string;
  items: KeyValuePairLike[];
}) {

  const onRowCommandClicked = (ev: unknown, item: IContextualMenuItem | undefined) => {
    if (item == null) return;

    if (item.key === 'CopyKeyValue') {
      copyTextToClipboard(`${item.item.key}: ${item.item.value}`);
    } else {
      copyTextToClipboard(item.item.value);
    }
  };

  const headersColumns: IColumn[] = [
    {
      key: 'Column-Key',
      name: props.keyName,
      fieldName: 'key',
      minWidth: 100,
      maxWidth: 150,
      isResizable: true,
      onRender: (item: KeyValuePairLike) => (
        <span title={item.key} style={{ color: '#000' }}>
          {item.key}
        </span>
      )
    },
    {
      key: 'ColumnValue',
      name: props.valueName,
      fieldName: 'value',
      minWidth: 100,
      isResizable: true,
      onRender: (item: KeyValuePairLike) => (
        <>
          <span title={item.value}>{item.value}</span>
        </>
      )
    },
    {
      key: 'Command',
      name: '',
      fieldName: '',
      minWidth: 32,
      onRender: (item, index, column) => (
        <>
          <div className={styles.keyValueDetailListCell_Command}>
            <IconButton
              iconProps={{ iconName: 'MoreVertical' }}
              menuProps={{
                items: [
                  { iconProps: { iconName: 'Copy' }, key: 'CopyKeyValue', text: 'Copy', item },
                  { iconProps: { iconName: 'Copy' }, key: 'CopyValue', text: 'Copy Value only', item }
                ],
                onItemClick: onRowCommandClicked
              }}
              split={false}
            />
          </div>
        </>
      )
    }
  ];

  return (
    <DetailsList
      compact={true}
      checkboxVisibility={CheckboxVisibility.hidden}
      selectionMode={SelectionMode.none}
      columns={headersColumns}
      items={props.items}
    />
  );
}

