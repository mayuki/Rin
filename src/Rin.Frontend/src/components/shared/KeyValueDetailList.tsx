import { CheckboxVisibility, DetailsList, IColumn, IconButton, SelectionMode } from 'office-ui-fabric-react';
import * as React from 'react';
import { copyTextToClipboard } from '../../utilities';
import * as styles from './KeyValueDetailList.css';

export interface KeyValuePairLike {
  key: string;
  value: string;
}

export class KeyValueDetailList extends React.Component<{
  keyName: string;
  valueName: string;
  items: KeyValuePairLike[];
}> {
  private get headersColumns(): IColumn[] {
    return [
      {
        key: 'Column-Key',
        name: this.props.keyName,
        fieldName: 'key',
        minWidth: 0,
        maxWidth: 100,
        isResizable: true,
        onRender: (item: KeyValuePairLike) => (
          <span title={item.key} style={{ color: '#000' }}>
            {item.key}
          </span>
        )
      },
      {
        key: 'ColumnValue',
        name: this.props.valueName,
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
        onRender: (item: any, index, column) => (
          <>
            <div className={styles.keyValueDetailListCell_Command}>
              <IconButton
                iconProps={{ iconName: 'MoreVertical' }}
                menuProps={{
                  items: [
                    { iconProps: { iconName: 'Copy' }, key: 'CopyKeyValue', text: 'Copy', item },
                    { iconProps: { iconName: 'Copy' }, key: 'CopyValue', text: 'Copy Value only', item }
                  ],
                  onItemClick: this.onRowCommandClicked
                }}
                split={false}
              />
            </div>
          </>
        )
      }
    ];
  }

  render() {
    return (
      <DetailsList
        compact={true}
        checkboxVisibility={CheckboxVisibility.hidden}
        selectionMode={SelectionMode.none}
        columns={this.headersColumns}
        items={this.props.items}
      />
    );
  }

  private onRowCommandClicked = (ev: any, item: any) => {
    if (item.key === 'CopyKeyValue') {
      copyTextToClipboard(`${item.item.key}: ${item.item.value}`);
    } else {
      copyTextToClipboard(item.item.value);
    }
  };
}
