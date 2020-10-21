import { hexy } from 'hexy';
import * as monacoEditor from 'monaco-editor';
import { Pivot, PivotItem } from 'office-ui-fabric-react';
import React, { useEffect, useState, useRef } from 'react';
import { ObjectInspector } from 'react-inspector';
import MonacoEditor from 'react-monaco-editor';
import SplitterLayout from 'react-splitter-layout';
import { BodyDataPayload, RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import {
  createKeyValuePairFromUrlEncoded,
  getContentType,
  getMonacoLanguage,
  isImage,
  isJson,
  isText,
  isWwwFormUrlencoded,
} from '../../utilities';
import { KeyValueDetailList } from '../shared/KeyValueDetailList';
import * as styles from './InspectorDetail.RequestResponseView.css';

export interface IInspectorRequestResponseViewProps {
  record: RequestRecordDetailPayload;
  generals: { key: string; value: string }[];
  headers: { [key: string]: string[] };
  trailers: { [key: string]: string[] } | null;
  body: BodyDataPayload | null;
  paneSize: number | null;
  onPaneSizeChange: (newSize: number) => void;
}

type PreviewType = 'Tree' | 'Source' | 'List' | 'Hex';

export function InspectorDetailRequestResponseView(props: IInspectorRequestResponseViewProps) {
  const [bodyView, setBodyView] = useState<PreviewType>('Source');
  const [paneToken, setPaneToken] = useState(0);

  const contentType = props.record.IsCompleted
    ? props.body != null && props.body.PresentationContentType !== ''
      ? props.body.PresentationContentType
      : getContentType(props.headers)
    : null;
  const isTransformed = props.body != null && props.body.PresentationContentType !== '';
  const hasBody = props.body != null && props.body.Body != null && props.body.Body.length > 0;
  const body =
    props.body != null && props.body.Body != null && props.body.Body.length > 0
      ? props.body.IsBase64Encoded
        ? atob(props.body.Body)
        : props.body.Body
      : '';

  const canPreview = (contentType: string) => {
    return isJson(contentType) || isWwwFormUrlencoded(contentType) || isText(contentType) || isImage(contentType);
  };

  const canPreviewForContentType = (contentType: string, type: PreviewType): boolean => {
    if (type == 'Hex') { 
      return true; // Hex dump always can be available for any content-type.
    }

    if (isJson(contentType) && (type == 'Tree')) {
      return true;
    }
    if (isWwwFormUrlencoded && (type == 'List')) {
      return true;
    }
    if ((isText(contentType) || isImage(contentType)) && (type == 'Source')) {
      return true;
    }

    return false;
  }

  const onBodyPivotItemClicked = (item?: PivotItem) => {
    if (item != null && item.props.itemKey != null) {
      setBodyView(item.props.itemKey as PreviewType);
    }
  };

  useEffect(() => {
    if (hasBody) {
      // Reset the preview type when content-type has been changed, and the current preview is not supported for it.
      if (contentType == null) {
        setBodyView('Hex');
      } else if (!canPreviewForContentType(contentType, bodyView)) {
        setBodyView(canPreview(contentType) ? 'Source' : 'Hex');
      }
    }
  }, [hasBody, contentType]);

  const trailers = props.trailers;
  return (
    <div className={styles.inspectorRequestResponseView}>
      <SplitterLayout
        vertical={true}
        percentage={true}
        secondaryInitialSize={props.paneSize || undefined}
        onSecondaryPaneSizeChange={(newSize) => {
          props.onPaneSizeChange(newSize);
          setPaneToken(paneToken + 1);
        }}
        primaryMinSize={10}
        secondaryMinSize={10}
      >
        <div>
          <div className={styles.inspectorRequestResponseView_General}>
            {props.generals != null && props.generals.length > 0 && (
              <KeyValueDetailList keyName="Name" valueName="Value" items={props.generals} />
            )}
          </div>
          <div className={styles.inspectorRequestResponseView_Headers}>
            <KeyValueDetailList
              keyName="Header"
              valueName="Value"
              items={Object.keys(props.headers).map((x) => ({
                key: x,
                value: props.headers[x].join('\n'),
              }))}
            />
          </div>
          {trailers != null && Object.keys(trailers).length > 0 && (
            <div className={styles.inspectorRequestResponseView_Headers}>
              <KeyValueDetailList
                keyName="Trailer"
                valueName="Value"
                items={Object.keys(trailers).map((x) => ({
                  key: x,
                  value: trailers[x].join('\n'),
                }))}
              />
            </div>
          )}
        </div>
        <div className={styles.inspectorRequestResponseView_Body}>
          {hasBody && contentType && (
            <>
              <Pivot selectedKey={bodyView} onLinkClick={onBodyPivotItemClicked}>
                {isJson(contentType) && <PivotItem itemKey="Tree" headerText="Tree" itemIcon="RowsChild" />}
                {isWwwFormUrlencoded(contentType) && <PivotItem itemKey="List" headerText="List" itemIcon="ViewList" />}
                {isText(contentType) && <PivotItem
                  itemKey="Source"
                  headerText={isTransformed ? `View as ${contentType}` : 'Source'}
                  itemIcon="Code"
                />}
                <PivotItem itemKey="Hex" headerText="Hex" itemIcon="ComplianceAudit" />
              </Pivot>
              {canPreview(contentType) && (<>
                {bodyView === 'List' && (
                  <div className={styles.inspectorRequestResponseViewKeyValueDetailList}>
                    <KeyValueDetailList keyName="Key" valueName="Value" items={createKeyValuePairFromUrlEncoded(body)} />
                  </div>
                )}
                {bodyView === 'Tree' && (
                  <div className={styles.inspectorRequestResponseViewObjectInspector}>
                    <ObjectInspector data={JSON.parse(body)} />
                  </div>
                )}
                {bodyView === 'Source' && (
                  <>
                    {isText(contentType) && (
                      <EditorPreview contentType={contentType} body={body} paneResizeToken={paneToken} />
                    )}
                    {isImage(contentType) && props.body != null && (
                      <ImagePreview contentType={contentType} bodyAsBase64={props.body.Body} />
                    )}
                  </>
                )}
              </>)}
              {bodyView === 'Hex' && props.body != null && (
                <HexDumpPreview
                  body={props.body.Body}
                  isBase64Encoded={props.body.IsBase64Encoded}
                  paneResizeToken={paneToken}
                />
              )}
            </>
          )}
        </div>
      </SplitterLayout>
    </div>
  );
}

function HexDumpPreview(props: { body: string; isBase64Encoded: boolean; paneResizeToken: number }) {
  function convertBase64StringToUint8Array(data: string) {
    const binary = atob(data);
    const array = new Array<number>(binary.length);
    for (let i = 0; i < binary.length; i++) {
      array[i] = binary.charCodeAt(i);
    }
    return array;
  }

  const body = props.isBase64Encoded ? convertBase64StringToUint8Array(props.body) : props.body;
  const hexed = hexy(body, { format: 'twos', caps: 'upper' }).trimEnd();

  return (
    <EditorPreview
      contentType="text/x-rin-hex-dump"
      language="text/x-rin-hex-dump"
      body={hexed}
      paneResizeToken={props.paneResizeToken}
      theme="rin-hex-dump"
    />
  );
}

monacoEditor.editor.defineTheme('rin-hex-dump', {
  base: 'vs',
  colors: {},
  rules: [{ token: 'address-token', foreground: 'aaaaaa' }],
  inherit: true,
});
monacoEditor.languages.register({ id: 'text/x-rin-hex-dump' });
monacoEditor.languages.setMonarchTokensProvider('text/x-rin-hex-dump', {
  tokenizer: {
    root: [[/^[0-9a-fA-F]+:/, 'address-token']],
  },
});

function EditorPreview(props: {
  contentType: string;
  body: string;
  paneResizeToken: number;
  theme?: string;
  language?: string;
}) {
  const [editor, setEditor] = useState<monacoEditor.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    const listener = () => {
      editor?.layout();
    };

    window.addEventListener('resize', listener);

    // force re-layout
    editor?.layout();

    return () => window.removeEventListener('resize', listener);
  }, [editor, props.paneResizeToken]);

  const monacoOptions: monacoEditor.editor.IEditorConstructionOptions = {
    readOnly: true,
    automaticLayout: true,
    wordWrap: 'on',
  };

  return (
    <div style={{ width: '100%', height: '100%', overflow: 'hidden' }}>
      <MonacoEditor
        width="100%"
        height="100%"
        options={monacoOptions}
        theme={props.theme ?? 'vs'}
        language={props.language ?? getMonacoLanguage(props.contentType)}
        value={props.body}
        editorDidMount={(editor) => setEditor(editor)}
      />
    </div>
  );
}

function ImagePreview(props: { contentType: string; bodyAsBase64: string }) {
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });
  const [loaded, setLoaded] = useState(false);
  const imagePreviewRef = useRef<HTMLImageElement>(null);

  useEffect(() => {
    if (imagePreviewRef.current != null) {
      const imageE = imagePreviewRef.current;
      const loaded = () => {
        setImageSize({ width: imageE.naturalWidth, height: imageE.naturalHeight });
        setLoaded(true);
      };

      imageE.addEventListener('load', loaded);
      return () => imageE.removeEventListener('load', loaded);
    }
  }, []);

  return (
    <div className={styles.inspectorRequestResponseViewImagePreview}>
      <figure>
        <div className={styles.inspectorRequestResponseViewImagePreview_Image}>
          <img ref={imagePreviewRef} src={'data:' + props.contentType + ';base64,' + props.bodyAsBase64} />
        </div>
        <figcaption>
          {loaded && (
            <>
              {imageSize.width} x {imageSize.height} |
            </>
          )}{' '}
          {props.contentType}
        </figcaption>
      </figure>
    </div>
  );
}
