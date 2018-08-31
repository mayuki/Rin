declare module 'react-inspector' {
  export interface ObjectInspectorProps {
    data: any;
    name?: string;
    expandLevel?: number;
    expandPaths?: string | string[];
    showNonenumerable?: boolean;
    sortObjectKeys?: boolean | ((a: any, b: any) => boolean);
    nodeRenderer?: (
      args: { depth: number; name: string; data: any; isNonenumerable: boolean; expanded: boolean }
    ) => any;
  }
  export class ObjectInspector extends React.Component<ObjectInspectorProps, {}> {}
}
