import { observer } from 'mobx-react';
import {
  CheckboxVisibility,
  DetailsList,
  DetailsRow,
  IColumn,
  IDetailsRowProps,
  SelectionMode
} from 'office-ui-fabric-react';
import * as React from 'react';
import { LogLevel, RequestRecordDetailPayload, TraceLogRecord } from '../../api/IRinCoreHub';

export interface IInspectorDetailTraceViewProps {
  record: RequestRecordDetailPayload;
}

@observer
export class InspectorDetailTraceView extends React.Component<IInspectorDetailTraceViewProps, {}> {
  private readonly traceColumns: IColumn[] = [
    {
      key: 'Time',
      name: 'Time',
      fieldName: 'DateTime',
      minWidth: 200,
      maxWidth: 200,
      isResizable: true
    },
    {
      key: 'LogLevel',
      name: 'LogLevel',
      fieldName: 'LogLevel',
      minWidth: 64,
      maxWidth: 72,
      isResizable: true,
      onRender: (item: TraceLogRecord) => (
        <>
          <span>{LogLevel[item.LogLevel]}</span>
        </>
      )
    },
    {
      key: 'Message',
      name: 'Message',
      fieldName: 'Message',
      minWidth: 100,
      isResizable: true
    }
  ];

  render() {
    return (
      <div className="inspectorDetail_TraceView">
        <DetailsList
          compact={true}
          checkboxVisibility={CheckboxVisibility.hidden}
          selectionMode={SelectionMode.none}
          columns={this.traceColumns}
          items={this.props.record.Traces}
          onRenderRow={this.onRenderRow}
        />
      </div>
    );
  }

  private onRenderRow(props: IDetailsRowProps) {
    const traceLogRecord = props.item as TraceLogRecord;
    const className = `inspectorDetailTraceView_Row inspectorDetailTraceView_Row-${LogLevel[traceLogRecord.LogLevel]}`;
    return (
      <div className={className}>
        <DetailsRow {...props} />
      </div>
    );
  }
}
