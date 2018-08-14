import {
  CheckboxVisibility,
  DetailsList,
  DetailsRow,
  IColumn,
  IDetailsRowProps,
  SelectionMode
} from 'office-ui-fabric-react';
import * as React from 'react';
import { RequestRecordDetailPayload, TimelineData, TimelineScopeCategory } from '../../api/IRinCoreHub';

export interface IInspectorDetailTraceViewProps {
  record: RequestRecordDetailPayload;
}

export class InspectorDetailTraceView extends React.Component<IInspectorDetailTraceViewProps, {}> {
  private readonly traceColumns: IColumn[] = [
    {
      key: 'Time',
      name: 'Time',
      fieldName: 'Timestamp',
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
      onRender: (item: TimelineData) => (
        <>
          <span>{item.Name}</span>
        </>
      )
    },
    {
      key: 'Message',
      name: 'Message',
      fieldName: 'Data',
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
          items={collectTraces(this.props.record.Timeline)}
          onRenderRow={this.onRenderRow}
        />
      </div>
    );
  }

  private onRenderRow(props: IDetailsRowProps) {
    const timelineData = props.item as TimelineData;
    const className = `inspectorDetailTraceView_Row inspectorDetailTraceView_Row-${timelineData.Name}`;
    return (
      <div className={className}>
        <DetailsRow {...props} />
      </div>
    );
  }
}

function collectTraces(data: TimelineData): TimelineData[] {
  return data.Children.reduce(
    (r, v) => {
      if (v.Category === TimelineScopeCategory.Trace) {
        r.push(v);
      }

      return r.concat(collectTraces(v));
    },
    [] as TimelineData[]
  );
}
