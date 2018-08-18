import * as monacoEditor from 'monaco-editor';
import { Toggle } from 'office-ui-fabric-react';
import * as React from 'react';
import MonacoEditor from 'react-monaco-editor';
import {
  RequestRecordDetailPayload,
  TimelineData,
  TimelineDataScope,
  TimelineEventCategory
} from '../../api/IRinCoreHub';
import './InspectorDetail.TraceView.css';

export interface IInspectorDetailTraceViewProps {
  record: RequestRecordDetailPayload;
  enableWordWrap: boolean;
  toggleWordWrap: (value: boolean) => void;
}

export class InspectorDetailTraceView extends React.Component<IInspectorDetailTraceViewProps, {}> {
  render() {
    return (
      <div className="inspectorDetailTraceView">
        <div className="inspectorDetailTraceView_Commands">
          <Toggle label="Enable WordWrap" checked={this.props.enableWordWrap} onChanged={this.props.toggleWordWrap} />
        </div>
        <TraceTextView
          body={traceAsText(collectTraces(this.props.record.Timeline))}
          enableWordWrap={this.props.enableWordWrap}
        />
      </div>
    );
  }
}

class TraceTextView extends React.Component<{ body: string; enableWordWrap: boolean }, {}> {
  private unsubscribe: () => void;
  private editor: monacoEditor.editor.IStandaloneCodeEditor;

  componentDidMount() {
    const listener = () => {
      this.editor.layout({ width: 0, height: 0 });
    };

    window.addEventListener('resize', listener);
    this.unsubscribe = () => window.removeEventListener('resize', listener);

    // force re-layout
    this.editor.layout({ width: 0, height: 0 });
  }

  componentWillUnmount() {
    this.unsubscribe();
  }

  componentDidUpdate() {
    this.editor.updateOptions(this.monacoOptions);
  }

  editorDidMount = (editor: monacoEditor.editor.IStandaloneCodeEditor) => {
    this.editor = editor;
  };

  render() {
    return (
      <MonacoEditor
        width="100%"
        height="100%"
        options={{ ...this.monacoOptions, theme: 'rin-log' }}
        language={'text/x-rin-log'}
        value={this.props.body}
        editorDidMount={this.editorDidMount}
      />
    );
  }

  private get monacoOptions(): monacoEditor.editor.IEditorOptions {
    return {
      readOnly: true,
      automaticLayout: true,
      wordWrap: this.props.enableWordWrap ? 'on' : 'off'
    };
  }
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
    { token: 'error-token', foreground: 'a80000' },
    { token: 'warn-token', foreground: 'e0ad06' },
    { token: 'info-token', foreground: 'aaaaaa' },
    { token: 'debug-token', foreground: 'cccccc' }
  ],
  inherit: true
});
monacoEditor.languages.register({ id: 'text/x-rin-log' });
monacoEditor.languages.setMonarchTokensProvider('text/x-rin-log', {
  defaultToken: '',
  tokenPostfix: '',

  tokenizer: {
    root: [
      {
        regex: new RegExp(/^\[[^\]]+\] Error:.*/),
        action: { token: 'error-token' }
      },
      {
        regex: new RegExp(/^\[[^\]]+\] Warning:.*/),
        action: { token: 'warn-token' }
      },
      {
        regex: new RegExp(/^\[[^\]]+\] Information:.*/),
        action: { token: 'info-token' }
      },
      {
        regex: new RegExp(/^\[[^\]]+\] (Debug|Trace):.*/),
        action: { token: 'debug-token' }
      }
    ]
  }
});
