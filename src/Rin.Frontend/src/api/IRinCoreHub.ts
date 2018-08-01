export interface IRinCoreHub {
  GetRecordingList(): Promise<RequestEventPayload[]>;
  GetDetailById(id: string): Promise<RequestRecordDetailPayload>;
  GetRequestBody(id: string): Promise<any>;
  GetResponseBody(id: string): Promise<any>;
}

export interface RequestEventPayload {
  Id: string;
  IsCompleted: boolean;
  Method: string;
  Path: string;
  ResponseStatusCode: number;
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
  Traces: { DateTime: string; LogLevel: LogLevel; Message: string }[];
}

export interface TraceLogRecord {
  DateTime: string;
  LogLevel: LogLevel;
  Message: string;
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
