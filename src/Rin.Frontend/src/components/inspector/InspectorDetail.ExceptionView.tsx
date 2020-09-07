import { FontClassNames } from '@uifabric/styling';
import React from 'react';
import { RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import * as styles from './InspectorDetail.ExceptionView.css';

export interface IInspectorDetailExceptionViewProps {
  record: RequestRecordDetailPayload;
}

export function InspectorDetailExceptionView(props: IInspectorDetailExceptionViewProps) {
  return (
    <div className={styles.inspectorDetail_ExceptionView}>
      <h2 className={FontClassNames.large}>
        {props.record.Exception.ClassName}: {props.record.Exception.Message}
      </h2>
      <pre>{props.record.Exception.StackTrace}</pre>
    </div>
  );
}
