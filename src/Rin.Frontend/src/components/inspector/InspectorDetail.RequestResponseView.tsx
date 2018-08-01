import { observer } from 'mobx-react';
import {
  CheckboxVisibility,
  DetailsList,
  IColumn,
  IconButton,
  Pivot,
  PivotItem,
  SelectionMode
} from 'office-ui-fabric-react';
import * as React from 'react';
import { ObjectInspector } from 'react-inspector';
import MonacoEditor from 'react-monaco-editor';
import { RequestRecordDetailPayload } from '../../api/IRinCoreHub';
import { copyTextToClipboard, getContentType, getMonacoLanguage, isImage, isJson, isText } from '../../utilities';

export interface IInspectorRequestResponseViewProps {
  record: RequestRecordDetailPayload;
  generals: { key: string; value: string }[];
  headers: { [key: string]: string[] };
  bodyRaw: string | null;
}

@observer
export class InspectorDetailRequestResponseView extends React.Component<
  IInspectorRequestResponseViewProps,
  { bodyView: 'Tree' | 'Source' }
> {
  private readonly headersColumns: IColumn[] = [
    {
      key: 'Key',
      name: 'Header',
      fieldName: 'Key',
      minWidth: 0,
      maxWidth: 100,
      isResizable: true,
      onRender: (item: any) => (
        <span title={item.Key} style={{ color: '#000' }}>
          {item.Key}
        </span>
      )
    },
    {
      key: 'Value',
      name: 'Value',
      fieldName: 'Value',
      minWidth: 100,
      isResizable: true,
      onRender: (item: any) => (
        <>
          <span title={item.Value}>{item.Value}</span>
        </>
      )
    },
    {
      key: 'Command',
      name: '',
      fieldName: '',
      minWidth: 32,
      onRender: (item: any, index, column) => (
        <>
          <div className="inspectorRequestResponseHeadersListCell_Command">
            <IconButton
              iconProps={{ iconName: 'MoreVertical' }}
              menuProps={{
                items: [
                  { iconProps: { iconName: 'Copy' }, key: 'CopyHeader', text: 'Copy Header', item },
                  { iconProps: { iconName: 'Copy' }, key: 'CopyValue', text: 'Copy Value only', item }
                ],
                onItemClick: this.onHeadersCommandClicked
              }}
              split={false}
            />
          </div>
        </>
      )
    }
  ];

  private readonly generalsColumns: IColumn[] = [
    {
      key: 'Key',
      name: 'Name',
      fieldName: 'key',
      minWidth: 0,
      maxWidth: 100,
      isResizable: true,
      onRender: (item: any) => (
        <span title={item.key} style={{ color: '#000' }}>
          {item.key}
        </span>
      )
    },
    {
      key: 'Value',
      name: 'Value',
      fieldName: 'value',
      minWidth: 100,
      isResizable: true,
      onRender: (item: any) => (
        <>
          <span title={item.value}>{item.value}</span>
        </>
      )
    }
  ];

  constructor(props: IInspectorRequestResponseViewProps) {
    super(props);
    this.state = {
      bodyView: 'Source'
    };
  }

  componentWillReceiveProps() {
    const contentType = this.props.record.IsCompleted && getContentType(this.props.headers);
    if (!(contentType && isJson(contentType) && this.state.bodyView === 'Tree')) {
      this.setState({ bodyView: 'Source' });
    }
  }

  render() {
    const contentType = this.props.record.IsCompleted && getContentType(this.props.headers);
    const body = this.props.bodyRaw ? atob(this.props.bodyRaw) : '';
    const hasBody = !!this.props.bodyRaw;

    return (
      <div className="inspectorRequestResponseView">
        <div className="inspectorRequestResponseView_General">
          {this.props.generals != null &&
            this.props.generals.length > 0 && (
              <DetailsList
                compact={true}
                checkboxVisibility={CheckboxVisibility.hidden}
                selectionMode={SelectionMode.none}
                columns={this.generalsColumns}
                items={this.props.generals}
              />
            )}
        </div>
        <div className="inspectorRequestResponseView_Headers">
          <DetailsList
            compact={true}
            checkboxVisibility={CheckboxVisibility.hidden}
            selectionMode={SelectionMode.none}
            columns={this.headersColumns}
            items={Object.keys(this.props.headers).map(x => ({
              Key: x,
              Value: this.props.headers[x].join('\n')
            }))}
          />
        </div>
        <div className="inspectorRequestResponseView_Body">
          {hasBody &&
            contentType && (
              <>
                <Pivot selectedKey={this.state.bodyView} onLinkClick={this.onBodyPivotItemClicked}>
                  {isJson(contentType) ? <PivotItem itemKey="Tree" headerText="Tree" itemIcon="RowsChild" /> : <></>}
                  <PivotItem itemKey="Source" headerText="Source" itemIcon="Code" />
                </Pivot>
                {this.state.bodyView === 'Tree' && (
                  <div className="inspectorRequestResponseViewObjectInspector">
                    <ObjectInspector data={JSON.parse(body)} />
                  </div>
                )}
                {this.state.bodyView === 'Source' && (
                  <>
                    {isText(contentType) && (
                      <MonacoEditor
                        width="100%"
                        height="100%"
                        options={{ readOnly: true, automaticLayout: true }}
                        language={getMonacoLanguage(contentType)}
                        value={body}
                      />
                    )}
                    {isImage(contentType) && (
                      <ImagePreview contentType={contentType} bodyAsBase64={this.props.bodyRaw!} />
                    )}
                  </>
                )}
              </>
            )}
        </div>
      </div>
    );
  }

  private onBodyPivotItemClicked = (item: PivotItem) => {
    this.setState({ bodyView: item.props.itemKey as 'Tree' | 'Source' });
  };

  private onHeadersCommandClicked = (ev: any, item: any) => {
    if (item.key === 'CopyHeader') {
      copyTextToClipboard(`${item.item.Key}: ${item.item.Value}`);
    } else {
      copyTextToClipboard(item.item.Value);
    }
  };
}

class ImagePreview extends React.Component<
  { contentType: string; bodyAsBase64: string },
  { width: number; height: number; loaded: boolean }
> {
  private imagePreview = React.createRef<HTMLImageElement>();

  constructor(props: { contentType: string; bodyAsBase64: string }) {
    super(props);

    this.state = {
      width: 0,
      height: 0,
      loaded: false
    };
  }

  componentDidMount() {
    this.imagePreview.current!.addEventListener('load', this.onImageLoad);
  }

  componentWillUnmount() {
    this.imagePreview.current!.removeEventListener('load', this.onImageLoad);
  }

  componentWillReceiveProps() {
    this.setState({ loaded: false });
  }

  render() {
    return (
      <div className="inspectorRequestResponseViewImagePreview">
        <figure>
          <div className="inspectorRequestResponseViewImagePreview_Image">
            <img
              ref={this.imagePreview}
              src={'data:' + this.props.contentType + ';base64,' + this.props.bodyAsBase64}
            />
          </div>
          <figcaption>
            {this.state.loaded && (
              <>
                {this.state.width} x {this.state.height} |
              </>
            )}{' '}
            {this.props.contentType}
          </figcaption>
        </figure>
      </div>
    );
  }

  private onImageLoad = () => {
    const imageE = this.imagePreview.current!;
    this.setState({
      loaded: true,
      width: imageE.naturalWidth,
      height: imageE.naturalHeight
    });
  };
}
