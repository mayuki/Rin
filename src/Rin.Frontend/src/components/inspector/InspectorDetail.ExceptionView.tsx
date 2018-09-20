import { FontClassNames } from '@uifabric/styling';
import * as React from 'react';
import { RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import * as styles from './InspectorDetail.ExceptionView.css';

export interface IInspectorDetailExceptionViewProps {
  record: RequestRecordDetailPayload;
}

export class InspectorDetailExceptionView extends React.Component<IInspectorDetailExceptionViewProps, {}> {
  render() {
    return (
      <div className={styles.inspectorDetail_ExceptionView}>
        <h2 className={FontClassNames.large}>
          {this.props.record.Exception.ClassName}: {this.props.record.Exception.Message}
        </h2>
        <pre>{this.props.record.Exception.StackTrace}</pre>
      </div>
    );
  }
}
