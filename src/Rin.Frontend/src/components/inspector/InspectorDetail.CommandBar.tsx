import { CommandBar, ContextualMenuItemType, ICommandBarItemProps } from 'office-ui-fabric-react';
import * as React from 'react';
import { createCSharpCodeFromDetail } from '../../domain/RequestRecord';
import { AppStore } from '../../store/AppStore';
import { InspectorStore } from '../../store/InspectorStore';
import { copyTextToClipboard } from '../../utilities';

export class InspectorDetailCommandBar extends React.Component<
  { appStore: AppStore; inspectorStore: InspectorStore },
  {}
> {
  private readonly commandBarItems: ICommandBarItemProps[] = [
    // { key: 'Replay', text: 'Replay', iconProps: { iconName: 'SendMirrored' } }
  ];

  render() {
    return <CommandBar items={this.commandBarItems} farItems={this.getCommandBarFarItems()} />;
  }

  private getCommandBarFarItems(): ICommandBarItemProps[] {
    const record = this.props.inspectorStore.currentRecordDetail;
    if (record === null || record === undefined) {
      return [];
    }

    const disableRequestDownload =
      !this.props.inspectorStore.requestBody ||
      (this.props.inspectorStore.requestBody.Body != null && this.props.inspectorStore.requestBody.Body.length === 0);

    return [
      {
        key: 'Export',
        text: 'Export/Save',
        iconProps: { iconName: 'Download' },
        subMenuProps: {
          onItemClick: (ev, item) => this.onContextualMenuItemClicked(ev, item),
          items: [
            {
              key: 'section1',
              itemType: ContextualMenuItemType.Section,
              sectionProps: {
                topDivider: false,
                bottomDivider: true,
                title: 'Export',
                items: [{ key: 'CopyAsCSharp', text: 'Copy Request as C# (LINQPad)' }]
              }
            },
            {
              key: 'section2',
              itemType: ContextualMenuItemType.Section,
              sectionProps: {
                topDivider: false,
                bottomDivider: true,
                title: 'Save',
                items: [
                  { key: 'SaveResponseBody', text: 'Save Response body' },
                  { key: 'SaveRequestBody', text: 'Save Request body', disabled: disableRequestDownload }
                ]
              }
            }
          ]
        }
      }
    ];
  }

  private onContextualMenuItemClicked = (ev: any, item: any) => {
    const selectedRecord = this.props.inspectorStore.currentRecordDetail!;
    switch (item.key) {
      case 'SaveResponseBody':
        window.open(`${this.props.appStore.endpointUrlBase}/download/response?id=${selectedRecord.Id}`);
        break;
      case 'SaveRequestBody':
        window.open(`${this.props.appStore.endpointUrlBase}/download/request?id=${selectedRecord.Id}`);
        break;
      case 'CopyAsCSharp':
        this.copyAsCSharp();
        break;
    }
  };

  private copyAsCSharp() {
    const selectedRecord = this.props.inspectorStore.currentRecordDetail!;
    const code = createCSharpCodeFromDetail(selectedRecord);
    copyTextToClipboard(code);
  }
}
