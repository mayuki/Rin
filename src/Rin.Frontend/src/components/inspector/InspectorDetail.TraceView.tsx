import * as monacoEditor from 'monaco-editor';
import { Toggle } from 'office-ui-fabric-react';
import React, { useEffect, useState } from 'react';
import MonacoEditor from 'react-monaco-editor';
import {
  RequestRecordDetailPayload,
  TimelineData,
  TimelineDataScope,
  TimelineEventCategory
} from '../../api/IRinCoreHub';
import * as styles from './InspectorDetail.TraceView.css';

export interface IInspectorDetailTraceViewProps {
  record: RequestRecordDetailPayload;
  enableWordWrap: boolean;
  toggleWordWrap: (value: boolean) => void;
}

export function InspectorDetailTraceView(props: IInspectorDetailTraceViewProps) {
  return (
    <div className={styles.inspectorDetailTraceView}>
      <div className={styles.inspectorDetailTraceView_Commands}>
        <Toggle
          label="Enable WordWrap"
          inlineLabel={true}
          checked={props.enableWordWrap}
          onChanged={props.toggleWordWrap}
        />
      </div>
      <TraceTextView
        body={traceAsText(collectTraces(props.record.Timeline))}
        enableWordWrap={props.enableWordWrap}
      />
    </div>
  );
}

export function TraceTextView(props: { body: string; enableWordWrap: boolean }) {
  const [editor, setEditor] = useState<monacoEditor.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    const listener = () => {
      editor?.layout({ width: 0, height: 0 });
    };

    window.addEventListener('resize', listener);

    // force re-layout
    editor?.layout({ width: 0, height: 0 });

    return () => window.removeEventListener('resize', listener);
  }, [] /* once */);

  const monacoOptions = {
    readOnly: true,
    automaticLayout: true,
    wordWrap: props.enableWordWrap ? 'on' : 'off' as 'on' | 'off'
  };

  return <MonacoEditor
    width="100%"
    height="100%"
    options={monacoOptions}
    language={'text/x-rin-log'}
    value={props.body}
    editorDidMount={editor => setEditor(editor)}
    theme="rin-log"
  />;
}

function traceAsText(timelineDataArray: TimelineData[]) {
  return timelineDataArray.map(x => `[${x.Timestamp}] ${x.Name}: ${x.Data}`).join('\r\n');
}

function collectTraces(data: TimelineDataScope): TimelineData[] {
  return data.Children.reduce(
    (r, v) => {
      if (v.Category === TimelineEventCategory.Trace) {
        r.push(v);
      }

      if (v.EventType === 'TimelineScope') {
        return r.concat(collectTraces(v));
      } else {
        return r;
      }
    },
    [] as TimelineData[]
  );
}

monacoEditor.editor.defineTheme('rin-log', {
  base: 'vs-dark',
  colors: {},
  rules: [
    { token: 'error-token', foreground: 'da0000' },
    { token: 'warn-token', foreground: 'e0ad06' },
    { token: 'info-token', foreground: 'cccccc' },
    { token: 'debug-token', foreground: 'aaaaaa' }
  ],
  inherit: true
});
monacoEditor.languages.register({ id: 'text/x-rin-log' });
monacoEditor.languages.setMonarchTokensProvider('text/x-rin-log', {
  defaultToken: '',
  tokenPostfix: '',

  tokenizer: {
    root: [
      { regex: /^\[[^\]]+\] (Critical|Error):/, action: { token: 'error-token', next: '@errorToken' } },
      { regex: /^\[[^\]]+\] (Warning):/, action: { token: 'warn-token', next: '@warnToken' } },
      { regex: /^\[[^\]]+\] (Information):/, action: { token: 'info-token', next: '@infoToken' } },
      { regex: /^\[[^\]]+\] (Debug|Trace):/, action: { token: 'debug-token', next: '@debugToken' } }
    ],

    errorToken: [
      { regex: /^\[/, action: { token: '', next: '@pop', goBack: 1 } },
      { regex: /.+/, action: { token: 'error-token' } }
    ],
    warnToken: [
      { regex: /^\[/, action: { token: '', next: '@pop', goBack: 1 } },
      { regex: /.+/, action: { token: 'warn-token' } }
    ],
    infoToken: [
      { regex: /^\[/, action: { token: '', next: '@pop', goBack: 1 } },
      { regex: /.+/, action: { token: 'info-token' } }
    ],
    debugToken: [
      { regex: /^\[/, action: { token: '', next: '@pop', goBack: 1 } },
      { regex: /.+/, action: { token: 'debug-token' } }
    ]
  }
});
