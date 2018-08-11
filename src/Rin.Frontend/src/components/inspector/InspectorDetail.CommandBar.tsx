import { CommandBar, ContextualMenuItemType, ICommandBarItemProps } from 'office-ui-fabric-react';
import * as React from 'react';
import { BodyDataPayload, RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import { createCSharpCodeFromDetail, createCurlFromDetail } from '../../domain/RequestRecord';
import { copyTextToClipboard } from '../../utilities';

export interface InspectorDetailCommandBarProps {
  endpointUrlBase: string;
  requestBody: BodyDataPayload | null;
  currentRecordDetail: RequestRecordDetailPayload | null;
}

export class InspectorDetailCommandBar extends React.Component<InspectorDetailCommandBarProps> {
  private readonly commandBarItems: ICommandBarItemProps[] = [
    // { key: 'Replay', text: 'Replay', iconProps: { iconName: 'SendMirrored' } }
  ];

  render() {
    return <CommandBar items={this.commandBarItems} farItems={this.getCommandBarFarItems()} />;
  }

  private getCommandBarFarItems(): ICommandBarItemProps[] {
    const record = this.props.currentRecordDetail;
    if (record === null || record === undefined) {
      return [];
    }

    const disableRequestDownload =
      !this.props.requestBody || (this.props.requestBody.Body != null && this.props.requestBody.Body.length === 0);

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
                items: [
                  { key: 'CopyAsCSharp', text: 'Copy Request as C# (LINQPad)' },
                  { key: 'CopyAsCurl', text: 'Copy Request as cURL' }
                ]
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
    const selectedRecord = this.props.currentRecordDetail!;
    switch (item.key) {
      case 'SaveResponseBody':
        window.open(`${this.props.endpointUrlBase}/download/response?id=${selectedRecord.Id}`);
        break;
      case 'SaveRequestBody':
        window.open(`${this.props.endpointUrlBase}/download/request?id=${selectedRecord.Id}`);
        break;
      case 'CopyAsCSharp':
        this.copyAsCSharp();
        break;
      case 'CopyAsCurl':
        this.copyAsCurl();
        break;
    }
  };

  private copyAsCSharp() {
    const selectedRecord = this.props.currentRecordDetail!;
    const code = createCSharpCodeFromDetail(selectedRecord, this.props.requestBody);
    copyTextToClipboard(code);
  }

  private copyAsCurl() {
    const selectedRecord = this.props.currentRecordDetail!;
    const code = createCurlFromDetail(selectedRecord, this.props.requestBody);
    copyTextToClipboard(code);
  }
}
