export interface IRinCoreHub {
  GetRecordingList(): Promise<RequestEventPayload[]>;
  GetDetailById(id: string): Promise<RequestRecordDetailPayload>;
  GetRequestBody(id: string): Promise<BodyDataPayload>;
  GetResponseBody(id: string): Promise<BodyDataPayload>;
}

export interface RequestEventPayload {
  Id: string;
  IsCompleted: boolean;
  RequestReceivedAt: string;
  Method: string;
  Path: string;
  ResponseStatusCode: number;
}

export interface BodyDataPayload {
  Body: string;
  IsBase64Encoded: boolean;
  PresentationContentType: string;
}

export interface RequestRecordDetailPayload {
  Id: string;
  IsCompleted: boolean;
  IsHttps: boolean;
  Host: string;
  Method: string;
  Path: string;
  QueryString: string;
  ResponseStatusCode: number;
  RemoteIpAddress: string;
  RequestHeaders: { [key: string]: string[] };
  ResponseHeaders: { [key: string]: string[] };
  RequestReceivedAt: string;
  ProcessingStartedAt: string;
  ProcessingCompletedAt: string;
  TransferringStartedAt: string;
  TransferringCompletedAt: string;
  Exception: any;
  Timeline: TimelineData;
}

export interface TimelineData {
  Timestamp: string;
  Category: TimelineScopeCategory | string;
  Name: string;
  Data: string | null;
  Duration: number;
  Children: TimelineData[];
}

export enum TimelineScopeCategory {
  Root = 'Rin.Timeline.Root',
  Method = 'Rin.Timeline.Method',
  Data = 'Rin.Timeline.Data',
  Trace = 'Rin.Timeline.Trace',
  AspNetCoreCommon = 'Rin.Timeline.AspNetCore.Common',
  AspNetCoreMvcView = 'Rin.Timeline.AspNetCore.Mvc.View',
  AspNetCoreMvcResult = 'Rin.Timeline.AspNetCore.Mvc.Result',
  AspNetCoreMvcAction = 'Rin.Timeline.AspNetCore.Mvc.Action'
}

// from Microsoft.Extensions.Logging.LogLevel
export enum LogLevel {
  Trace = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4,
  Critical = 5,
  None = 6
}
