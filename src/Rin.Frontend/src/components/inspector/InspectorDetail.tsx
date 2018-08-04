import { observer } from 'mobx-react';
import { Pivot, PivotItem } from 'office-ui-fabric-react/lib/Pivot';
import * as React from 'react';
import { AppStore } from '../../store/AppStore';
import { DetailViewType, InspectorStore } from '../../store/InspectorStore';
import { InspectorDetailCommandBar } from './InspectorDetail.CommandBar';
import './InspectorDetail.css';
import { InspectorDetailExceptionView } from './InspectorDetail.ExceptionView';
import { InspectorDetailRequestResponseView } from './InspectorDetail.RequestResponseView';
import { InspectorDetailTraceView } from './InspectorDetail.TraceView';

export interface IInspectorDetailProps {
  appStore: AppStore;
  inspectorStore: InspectorStore;
}

@observer
export class InspectorDetail extends React.Component<IInspectorDetailProps, {}> {
  constructor(props: IInspectorDetailProps) {
    super(props);
  }

  public render() {
    const selectedRecord = this.props.inspectorStore.currentRecordDetail;
    return (
      <div className="inspectorDetail">
        <div className="inspectorDetail_CommandBar">
          <InspectorDetailCommandBar appStore={this.props.appStore} inspectorStore={this.props.inspectorStore} />
        </div>
        {selectedRecord != null && (
          <>
            <div className="inspectorDetail_Pivot">
              <Pivot selectedKey={this.props.inspectorStore.currentDetailView} onLinkClick={this.onPivotItemClicked}>
                <PivotItem itemKey={DetailViewType.Request} headerText="Request" itemIcon="CloudUpload" />
                {selectedRecord.IsCompleted ? (
                  <PivotItem itemKey={DetailViewType.Response} headerText="Response" itemIcon="CloudDownload" />
                ) : (
                  <></>
                )}
                {/*<PivotItem itemKey={DetailViewType.Timings} headerText="Timings" itemIcon="TimelineProgress" />*/}
                <PivotItem itemKey={DetailViewType.Trace} headerText="Trace" itemIcon="TimeEntry" />
                {selectedRecord.Exception ? (
                  <PivotItem itemKey={DetailViewType.Exception} headerText="Exception" itemIcon="StatusErrorFull" />
                ) : (
                  <></>
                )}
              </Pivot>
            </div>
            <div className="inspectorDetail_DetailView">
              {this.props.inspectorStore.currentDetailView === DetailViewType.Request && this.renderRequestView()}
              {this.props.inspectorStore.currentDetailView === DetailViewType.Response && this.renderResponseView()}
              {this.props.inspectorStore.currentDetailView === DetailViewType.Trace && (
                <InspectorDetailTraceView record={selectedRecord} />
              )}
              {this.props.inspectorStore.currentDetailView === DetailViewType.Exception && (
                <InspectorDetailExceptionView record={selectedRecord} />
              )}
            </div>
          </>
        )}
      </div>
    );
  }

  public renderResponseView() {
    const selectedRecord = this.props.inspectorStore.currentRecordDetail!;
    const headers = selectedRecord.ResponseHeaders;

    return (
      <InspectorDetailRequestResponseView
        record={selectedRecord}
        generals={[{ key: 'StatusCode', value: selectedRecord.ResponseStatusCode + '' }]}
        headers={headers}
        body={this.props.inspectorStore.responseBody}
      />
    );
  }

  public renderRequestView() {
    const selectedRecord = this.props.inspectorStore.currentRecordDetail!;
    const headers = { ...selectedRecord.RequestHeaders };

    const url =
      (selectedRecord.IsHttps ? 'https' : 'http') +
      '://' +
      selectedRecord.Host +
      selectedRecord.Path +
      (selectedRecord.QueryString != null && selectedRecord.QueryString !== '' ? selectedRecord.QueryString : '');

    return (
      <InspectorDetailRequestResponseView
        record={selectedRecord}
        generals={[
          { key: 'Method', value: selectedRecord.Method },
          { key: 'Url', value: url },
          { key: 'Remote Address', value: selectedRecord.RemoteIpAddress }
        ]}
        headers={headers}
        body={this.props.inspectorStore.requestBody}
      />
    );
  }

  private onPivotItemClicked = (item: PivotItem) => {
    this.props.inspectorStore.selectDetailView(item.props.itemKey as DetailViewType);
  };
}
