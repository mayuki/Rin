declare module 'react-splitter-layout' {
  export interface SplitterLayoutProps {
    customClassName?: string;
    vertical?: boolean;
    percentage?: boolean;
    primaryIndex?: number;
    primaryMinSize?: number;
    secondaryInitialSize?: number;
    secondaryMinSize?: number;
    onDragStart?: Function;
    onDragEnd?: Function;
    onSecondaryPaneSizeChange?: (secondaryPaneSize: number) => void;
    children?: React.ReactNode[];
  }
  export default class SplitterLayout extends React.Component<SplitterLayoutProps> {}
}
