import { saveAs } from 'file-saver';
import { CommandBar, ContextualMenuItemType, ICommandBarItemProps } from 'office-ui-fabric-react';
import * as React from 'react';
import { BodyDataPayload, RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import {
  createCSharpCodeFromDetail,
  createCSharpLinqPadFileFromDetail,
  createCurlFromDetail
} from '../../domain/RequestRecord';
import { copyTextToClipboard } from '../../utilities';
import { IContextualMenuItem } from '@fluentui/react';

export interface InspectorDetailCommandBarProps {
  endpointUrlBase: string;
  requestBody: BodyDataPayload | null;
  currentRecordDetail: RequestRecordDetailPayload | null;
}

export function InspectorDetailCommandBar(props: InspectorDetailCommandBarProps) {
  const commandBarItems: ICommandBarItemProps[] = [
    // { key: 'Replay', text: 'Replay', iconProps: { iconName: 'SendMirrored' } }
  ];


  function getCommandBarFarItems(): ICommandBarItemProps[] {
    const record = props.currentRecordDetail;
    if (record === null || record === undefined) {
      return [];
    }

    const disableRequestDownload =
      !props.requestBody || (props.requestBody.Body != null && props.requestBody.Body.length === 0);

    return [
      {
        key: 'Export',
        text: 'Export/Save',
        iconProps: { iconName: 'Download' },
        subMenuProps: {
          onItemClick: onContextualMenuItemClicked,
          items: [
            {
              key: 'section1',
              itemType: ContextualMenuItemType.Section,
              sectionProps: {
                topDivider: false,
                bottomDivider: true,
                title: 'Copy',
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
                  { key: 'SaveRequestAsCSharp', text: 'Save Request as C# (LINQPad)' },
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

  function onContextualMenuItemClicked(ev?: React.MouseEvent<HTMLElement> | React.KeyboardEvent<HTMLElement>, item?: IContextualMenuItem) {
    const selectedRecord = props.currentRecordDetail;
    if (selectedRecord == null || item == null) return;

    switch (item.key) {
      case 'SaveResponseBody':
        window.open(`${props.endpointUrlBase}/download/response?id=${selectedRecord.Id}`);
        break;
      case 'SaveRequestBody':
        window.open(`${props.endpointUrlBase}/download/request?id=${selectedRecord.Id}`);
        break;
      case 'SaveRequestAsCSharp':
        saveRequestAsCSharp();
        break;
      case 'CopyAsCSharp':
        copyAsCSharp();
        break;
      case 'CopyAsCurl':
        copyAsCurl();
        break;
    }
  };

  function saveRequestAsCSharp() {
    const selectedRecord = props.currentRecordDetail;
    if (selectedRecord == null) return;

    const linqFileContent = createCSharpLinqPadFileFromDetail(selectedRecord, props.requestBody);
    saveAs(new File([linqFileContent], `${selectedRecord.Id}.linq`, { type: 'application/octet-stream' }));
  }

  function copyAsCSharp() {
    const selectedRecord = props.currentRecordDetail;
    if (selectedRecord == null) return;

    const code = createCSharpCodeFromDetail(selectedRecord, props.requestBody, false);
    copyTextToClipboard(code);
  }

  function copyAsCurl() {
    const selectedRecord = props.currentRecordDetail;
    if (selectedRecord == null) return;

    const code = createCurlFromDetail(selectedRecord, props.requestBody);
    copyTextToClipboard(code);
  }

  return <CommandBar items={commandBarItems} farItems={getCommandBarFarItems()} />;
}
